using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITI_Hackathon.Controllers
{
	public class OrderController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartservice;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(IOrderService orderservice,ApplicationDbContext context,ICartService cartservice,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _orderService = orderservice;
            _cartservice = cartservice;
            _userManager = userManager;
        }

		// POST: /Order/Create
		[HttpPost]
        public async Task<IActionResult> CreateOrderFromCart()
		{
			if (User.Identity?.IsAuthenticated != true)
			{
				var returnUrl = Url.Action("CreateOrderFromCart", "Order");
				return RedirectToAction("Login", "Account", new { ReturnUrl = returnUrl });
			}
			var userid = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
            var sessionid = Request.Cookies["GuestSessionId"];
           OrderDto orderDto= await _orderService.CreateOrderFromCartAsync(userid,sessionid);
            return View(orderDto);
        }

		// GET: /Order/MyOrders
		public async Task<IActionResult> GetOrderHistory()
		{
			if (!User.Identity?.IsAuthenticated ?? true)

			{

				return RedirectToAction("Login", "Account");

			}


			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))

			{

				return RedirectToAction("Login", "Account");

			}

			IEnumerable<OrderDto> Orderhistory = await _orderService.GetOrdersForUserAsync(userId);
			return View(Orderhistory);
		}

		// GET: /Order/Details/5
		public async Task<IActionResult> GetOrderDetails(int orderid)
		{
            if (orderid==0)
            {
                return NotFound("orderid not found");
            }
            OrderDetailsDto Orderdetails = await _orderService.GetOrderByIdAsync(orderid);
            if (Orderdetails==null)
            {
                return NotFound("there is no order with this ID");
            }
			return View(Orderdetails);
		}
		// POST: /Order/UpdateStatus/5
		public async Task<IActionResult> UpdateOrderStatus(int orderid,string newstatus)
		{
            bool isupdated = await _orderService.UpdateOrderStatusAsync(orderid,newstatus);
            if (isupdated==false)
            {
                return NotFound();
            }

            return RedirectToAction("GetOrderDetails", new { orderid});
		}


	}
}
