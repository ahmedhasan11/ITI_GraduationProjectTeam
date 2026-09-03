using Healthcare.Application.DTOs;

public interface ICartService
{
    Task<List<CartItemDto>> GetCartItemsAsync(string? userId = null, string? sessionId = null);
    Task<int> AddToCartAsync(AddToCartDto dto);
    Task<bool> RemoveFromCartAsync(string? userId = null, string? sessionId = null, int cartItemId = 0);
    //Task<CheckoutDto> CheckoutAsync(string? userId = null, string? sessionId = null);
}
