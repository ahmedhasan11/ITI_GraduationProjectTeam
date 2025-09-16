using System.Security.Claims;
using System.Threading.Tasks;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using ITI_Hackathon.Services;
using ITI_Hackathon.Stripe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

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
        public async Task<ActionResult> CreateCheckout()
        {
            var userId = User.Identity != null && User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
            var sessionId = userId == null ? Request.Cookies["GuestSessionId"] : null;

            if (userId == null && sessionId == null)
                return BadRequest("No cart owner found.");

            var order = await _orderservice.CreateOrderFromCartAsync(userId, sessionId);
            if (order == null || order.Items == null || !order.Items.Any())
                return BadRequest("Cart is empty.");

            decimal deliveryFee = 15m;

            var domain = "https://clickclinic.runasp.net";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}/Payment/Success?orderId={order.OrderId}",
                CancelUrl = $"{domain}/Payment/Cancel?orderId={order.OrderId}",
            };


            foreach (var item in order.Items)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(item.UnitPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.MedicineName
                        }
                    },
                    Quantity = item.Quantity
                });
            }

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = (long)(deliveryFee * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Delivery Fee"
                    }
                },
                Quantity = 1
            });

            var couponService = new CouponService();
            var coupon = couponService.Create(new CouponCreateOptions
            {
                Name = "Manual Discount",
                AmountOff = (long)(10 * 100), 
                Currency = "usd",
            });

            options.Discounts = new List<SessionDiscountOptions>
    {
        new SessionDiscountOptions { Coupon = coupon.Id }
    };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

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
			var domain = "https://clickclinic.runasp.net";
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
			return RedirectToAction("Index", "ChatPage", new { doctorId = doctorId });
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
