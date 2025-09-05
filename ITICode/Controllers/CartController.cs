using ITI_Hackathon.ServiceContracts.DTO;
using ITI_Hackathon.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

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

    private (string? userId, string? sessionId) GetCartOwner()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return (User.Identity.Name, null);

        var sessionId = GetSessionId();
        return (null, sessionId);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var (userId, sessionId) = GetCartOwner();
        var cart = await _cartService.GetCartItemsAsync(userId, sessionId);
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var (userId, sessionId) = GetCartOwner();
        dto.UserId = userId;
        dto.SessionId = sessionId;

        var totalItems = await _cartService.AddToCartAsync(dto);
        return Ok(new { success = true, totalItems });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var (userId, sessionId) = GetCartOwner();
        var success = await _cartService.RemoveFromCartAsync(userId, sessionId, id);
        return Ok(new { success });
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var (userId, sessionId) = GetCartOwner();
        var result = await _cartService.CheckoutAsync(userId, sessionId);
        return Ok(new { success = true, orderId = result.OrderId, total = result.Total });
    }
}
