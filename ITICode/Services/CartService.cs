using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItemDto>> GetCartItemsAsync(string? userId = null, string? sessionId = null)
        {
            var cart = await _context.CartItems
                .Include(c => c.Medicine)
                .Where(c => (userId != null && c.UserId == userId) || (sessionId != null && c.SessionId == sessionId))
                .ToListAsync();

            return cart.Select(c => new CartItemDto
            {
                Id = c.Id,
                MedicineId = c.MedicineId,
                MedicineName = c.Medicine.Name,
                UnitPrice = c.Medicine.Price,
                Quantity = c.Quantity
            }).ToList();
        }

        public async Task<int> AddToCartAsync(AddToCartDto dto)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.MedicineId == dto.MedicineId &&
                    ((dto.UserId != null && c.UserId == dto.UserId) ||
                     (dto.SessionId != null && c.SessionId == dto.SessionId)));

            if (cartItem != null)
            {
                cartItem.Quantity += dto.Quantity;
            }
            else
            {
                var medicine = await _context.Medicines.FindAsync(dto.MedicineId);
                if (medicine == null) throw new KeyNotFoundException("Medicine not found");

                cartItem = new CartItem
                {
                    MedicineId = dto.MedicineId,
                    Quantity = dto.Quantity,
                    UserId = dto.UserId,
                    SessionId = dto.SessionId
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            return await _context.CartItems
                .Where(c => (dto.UserId != null && c.UserId == dto.UserId) ||
                            (dto.SessionId != null && c.SessionId == dto.SessionId))
                .SumAsync(c => c.Quantity);
        }

        public async Task<bool> RemoveFromCartAsync(string? userId = null, string? sessionId = null, int cartItemId = 0)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId &&
                    ((userId != null && c.UserId == userId) || (sessionId != null && c.SessionId == sessionId)));

            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CheckoutDto> CheckoutAsync(string? userId = null, string? sessionId = null)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Medicine)
                .Where(c => (userId != null && c.UserId == userId) || (sessionId != null && c.SessionId == sessionId))
                .ToListAsync();

            if (!cartItems.Any()) throw new InvalidOperationException("Cart is empty");

            var order = new Order
            {
                PatientId = userId ?? "Guest", // use placeholder for guest
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Total = cartItems.Sum(c => c.Medicine.Price * c.Quantity),
                Items = cartItems.Select(c => new OrderItem
                {
                    MedicineId = c.MedicineId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Medicine.Price
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return new CheckoutDto
            {
                OrderId = order.Id,
                Total = order.Total
            };
        }
    }
}
