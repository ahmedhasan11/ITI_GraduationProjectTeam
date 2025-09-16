using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Services
{
	public class OrderService : IOrderService
	{
		private readonly ApplicationDbContext _context;
		public OrderService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> ClearCartAfterPaymentAsync(int orderId)
		{
			var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == orderId);
			if (order==null)
			{
				return false;
			}
			List<CartItem> cartItems;

			if (order.PatientId != null)
			{
				cartItems = await _context.CartItems.Where(cart => cart.UserId == order.PatientId).ToListAsync();
			}
			else 
			{
				return false;
			}

			 _context.RemoveRange(cartItems);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<OrderDto> CreateOrderFromCartAsync(string? userId = null, string? sessionId = null)
		{
			//get allcart items from that specificuser usingsession idor userID
			List<CartItem> CartItems =await _context.CartItems.Include(c => c.Medicine)
				.Where(c => c.UserId != null && c.UserId == userId || c.SessionId != null && c.SessionId == sessionId)
				.ToListAsync();

			if (CartItems==null)
			{
				throw new InvalidOperationException("Cart is empty");
			}
			//create neworder-->put Cartitemsonit
			Order order = new Order()
			{
				PatientId = userId,
				CreatedAt = DateTime.UtcNow,
				Status = "Pending",
				Total = CartItems.Sum(c => c.Medicine.Price * c.Quantity),
				
				Items = CartItems.Select(c => new OrderItem()
				{
					MedicineId = c.MedicineId,
					Quantity = c.Quantity,
					UnitPrice = c.Medicine.Price
				}).ToList()
			};
			//Add order to Order table
			await _context.Orders.AddAsync(order);
			//delete cart
			// _context.CartItems.RemoveRange(CartItems);

			await _context.SaveChangesAsync();

			//return order details
			return new OrderDto()
			{
				OrderId = order.Id,
				CreatedAt = order.CreatedAt,
				PatientId = order.PatientId,
				Status = order.Status,
				Total = order.Total,
				Items = order.Items.Select(item => new OrderItemDto()
				{
					MedicineID=item.Id,
					MedicineName=item.Medicine.Name,
					Quantity=item.Quantity,
					UnitPrice=item.UnitPrice
				}).ToList()
			};
		}

		public async Task<bool> DeleteOrderAsync(int orderId)
		{
			var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == orderId);
			if (order==null)
			{
				return false;
			}
			
			_context.OrderItems.RemoveRange(order.Items);

			_context.Orders.Remove(order);

			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<OrderDetailsDto> GetOrderByIdAsync(int orderID)
		{
			Order order = await _context.Orders.Include(order => order.Items).ThenInclude(orderitem => orderitem.Medicine)
				.FirstOrDefaultAsync(order => order.Id == orderID);

			if (order==null)
			{
				return null;
			}

			OrderDetailsDto orderDetailsDto = new OrderDetailsDto()
			{
				Id = order.Id,
				CreatedAt = order.CreatedAt,
				Status = order.Status,
				Total = order.Total,
				Items =order.Items.Select(o=>new OrderItemDto()
				{
					MedicineID=o.MedicineId,
					MedicineName=o.Medicine.Name,
					Quantity=o.Quantity,
					UnitPrice=o.UnitPrice,

				}).ToList()
			};

			return orderDetailsDto;
		}

		public async Task<IEnumerable<OrderDto>> GetOrdersForUserAsync(string userId)
		{
			List<Order> Orders=await _context.Orders
				.Where(order => order.PatientId == userId)
				.Include(order => order.Items)
				.ThenInclude(orderitem=>orderitem.Medicine)
				.OrderByDescending(order=>order.CreatedAt)
				.ToListAsync();

			List<OrderDto> OrdersDto= Orders.Select(order => new OrderDto()
			{
				OrderId=order.Id,
				CreatedAt=order.CreatedAt,
				Status=order.Status,
				Total=order.Total,
				Items= order.Items.Select(item=>new OrderItemDto()
				{
					MedicineID=item.MedicineId,
					MedicineName=item.Medicine.Name,
					Quantity=item.Quantity,
					UnitPrice=item.Medicine.Price,

				}).ToList()
			}).ToList();

			return OrdersDto;


		}

		public async Task<bool> UpdateOrderStatusAsync(int orderId, string newstatus)
		{
			Order order= await _context.Orders.FirstOrDefaultAsync(order => order.Id == orderId);

			if (order==null)
			{
				return false;
			}

			order.Status = newstatus;
			_context.Orders.Update(order);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
