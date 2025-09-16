using ITI_Hackathon.ServiceContracts.DTO;
using ITI_Hackathon.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class CartController : Controller
{
	private readonly ICartService _cartService;

	public CartController(ICartService cartService)
	{
		_cartService = cartService;
	}

	// Helper: generate or get session id for guest user
	private string GetSessionId()
	{
		if (Request.Cookies.ContainsKey("GuestSessionId"))
			return Request.Cookies["GuestSessionId"]!;

		var sessionId = Guid.NewGuid().ToString();
		Response.Cookies.Append("GuestSessionId", sessionId, new CookieOptions
		{
			Expires = DateTimeOffset.UtcNow.AddDays(7),
			HttpOnly = true,
			IsEssential = true
		});
		return sessionId;
	}

	// Helper: detect if cart belongs to logged user or guest
	private (string? userId, string? sessionId) GetCartOwner()
	{
		if (User.Identity != null && User.Identity.IsAuthenticated) 
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return (userId, null);
		}
		var sessionId = GetSessionId();
		return (null, sessionId);
	}
	[HttpGet]
	public async Task<IActionResult> GetCartCount()
	{
		var (userId, sessionId) = GetCartOwner();
		var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);

		return Json(new { count = cartItems.Sum(item => item.Quantity) });
	}
	[HttpGet]
    public async Task<IActionResult> GetCartItemsDropdown()
    {
        var (userId, sessionId) = GetCartOwner();
        var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);

        var total = cartItems.Sum(i => i.UnitPrice * i.Quantity);
        var count = cartItems.Sum(i => i.Quantity);

        ViewBag.Total = total;
        ViewBag.Count = count;

        return PartialView("_CartDropdownPartial", cartItems);
    }


    // GET: /Cart
    [HttpGet]
	public async Task<IActionResult> Index()
	{
		var (userId, sessionId) = GetCartOwner();
		var cart = await _cartService.GetCartItemsAsync(userId, sessionId);
		return View(cart); 
	}

	// POST: /Cart/Add
	[HttpPost]
	public async Task<IActionResult> Add([FromBody] AddToCartDto dto)
	{
		var (userId, sessionId) = GetCartOwner();
		dto.UserId = userId;
		dto.SessionId = sessionId;

		var totalItems = await _cartService.AddToCartAsync(dto);
		return Json(new { success = true, totalItems, message="added to cart successfully" });
	}

	// POST: /Cart/Remove/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Remove(int id)
	{
		var (userId, sessionId) = GetCartOwner();
		await _cartService.RemoveFromCartAsync(userId, sessionId, id);

		return RedirectToAction("Index");
	}
}
