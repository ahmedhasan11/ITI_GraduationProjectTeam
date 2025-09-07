using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using ITI_Hackathon.Stripe;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using System.Threading.Tasks;
using System.Security.Claims;
using ITI_Hackathon.Services;
using Microsoft.AspNetCore.Authorization;

namespace ITI_Hackathon.Controllers
{
    public class PaymentController : Controller
    {
		private readonly StripeSettings _stripeconfiguration;
		private readonly IOrderService _orderservice;
		private readonly IConsultationService _consultationservice;
		public PaymentController(IOptions<StripeSettings> stripeconfiguration, IOrderService orderservice, IConsultationService consultationservice)
		{
			_stripeconfiguration = stripeconfiguration.Value;
			_orderservice = orderservice;
			_consultationservice = consultationservice;
		}

		[HttpPost]
		//Post:Payment/CreateCheckout
		public async Task<ActionResult> CreateCheckout()
		{
			var userId = User.Identity != null && User.Identity.IsAuthenticated	? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value: null;
			var sessionId = userId == null ? Request.Cookies["GuestSessionId"]:null;

			if (userId == null && sessionId == null)
			{
				return BadRequest("No cart owner found.");
			}
			var order = await _orderservice.CreateOrderFromCartAsync(userId, sessionId);
			if (order == null)
			{
				return BadRequest("Cart is empty or could not create order.");
			}

			//OrderDetailsDto order = await _orderservice.GetOrderByIdAsync(OrderID);

			if (order==null)
			{
				return NotFound("there is no order with that id");
			}

			var domain = "https://localhost:7101";

			//Checkout Session Options
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = order.Items.Select(orderitem=> new SessionLineItemOptions 
				{
					 PriceData= new SessionLineItemPriceDataOptions
					  {
						 UnitAmount = (long)(orderitem.UnitPrice * 100),
						 Currency="usd",
						 ProductData= new SessionLineItemPriceDataProductDataOptions
						 {
							 Name=orderitem.MedicineName
						 },
					  },
					 Quantity=orderitem.Quantity
				}).ToList(),
				Mode = "payment",
				SuccessUrl = $"{domain}/Payment/Success?orderId={order.OrderId}",
				CancelUrl = $"{domain}/Payment/Cancel?orderId={order.OrderId}",
			};

			//Start Session With Stripe
			var service = new SessionService();
			Session session = service.Create(options);

			return Redirect(session.Url);
		}

		//Get:Payment/Success
		[HttpGet]
		public async Task<IActionResult> Success(int orderID)
		{
			await _orderservice.UpdateOrderStatusAsync(orderID, "Paid");
			await _orderservice.ClearCartAfterPaymentAsync(orderID);


			//Order/GetOrderdetails/orderID
			return RedirectToAction("GetOrderDetails", "Order", new { orderId=orderID });
		}
		[HttpGet]
		//Get:Payment/Cancel
		public async Task<IActionResult> Cancel(int orderId)
		{
			await _orderservice.DeleteOrderAsync(orderId);
			await _orderservice.UpdateOrderStatusAsync(orderId, "Canceled");

			return RedirectToAction("GetOrderDetails", "Order", new { orderid = orderId });
		}


		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateConsultationCheckout(string doctorId)
		{
			var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier); //current patient
			if (patientId == null)
			{
				return RedirectToAction("Login", "Account");
			}
			bool hasPaid = await _consultationservice.HasPaidForConsultationAsync(patientId, doctorId);
			if (hasPaid)
			{
				// If already paid, redirect directly to chat
				return RedirectToAction("ChatWithDoctor", "Chat", new { doctorId = doctorId });
			}
			//create payment session
			var consultationInfo = await _consultationservice.CreateConsultationPaymentAsync(patientId, doctorId);
			var domain = "https://localhost:7101";
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = new List<SessionLineItemOptions>
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(consultationInfo.Amount * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = $"Consultation with Dr. {consultationInfo.DoctorName}",
								Description = "One-time consultation fee"
							},
						},
						Quantity = 1,
					},
				},
				Mode = "payment",
				SuccessUrl = $"{domain}/Payment/ConsultationSuccess?doctorId={doctorId}&sessionId={{CHECKOUT_SESSION_ID}}",
				CancelUrl = $"{domain}/Payment/ConsultationCancel?doctorId={doctorId}",
			};
			var service = new SessionService();
			Session session = service.Create(options);

			consultationInfo.SessionId = session.Id;
			consultationInfo.SessionUrl = session.Url;

			return Redirect(session.Url);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> ConsultationSuccess(string doctorId, string sessionId)
		{
			var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Retrieve the Stripe session to get payment details
			var service = new SessionService();
			var session = service.Get(sessionId);
			if (session == null)
			{
				TempData["Error"] = "Payment session not found.";
				return RedirectToAction("Index", "Home");
			}

			// Record the successful payment
			await _consultationservice.RecordSuccessfulConsultationAsync(
				patientId,
				doctorId,
				session.PaymentIntentId,
				(decimal)(session.AmountTotal / 100.0) // Convert from cents to dollars
			);

			// Redirect to chat
			return RedirectToAction("ChatWithDoctor", "Chat", new { doctorId = doctorId });
		}
		[Authorize]
		[HttpGet]
		public IActionResult ConsultationCancel(string doctorId)
		{
			// Optional: You might want to show a message
			TempData["Error"] = "Consultation payment was cancelled.";
			return RedirectToAction("Index", "Home", new { id = doctorId });
		}


	}
}
