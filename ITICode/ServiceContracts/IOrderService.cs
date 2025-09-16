using ITI_Hackathon.ServiceContracts.DTO;

namespace ITI_Hackathon.ServiceContracts
{
	public interface IOrderService
	{
		Task<OrderDto> CreateOrderFromCartAsync(string? userId = null, string? sessionId = null);
		/// <summary>
		/// get details of specific order(if youwant to print its details)
		/// </summary>
		/// <param name="orderID"> usedto findthat specificorder</param>
		/// <returns></returns>
		Task<OrderDetailsDto> GetOrderByIdAsync(int orderID);
		///checksfor historyor orders for a specific user
		Task<IEnumerable<OrderDto>> GetOrdersForUserAsync(string userId);
		//changes order statusfrom pending -->paid-->shipped
		Task<bool> UpdateOrderStatusAsync(int orderId, string newstatus);
		Task<bool> ClearCartAfterPaymentAsync(int orderId);
		Task<bool> DeleteOrderAsync(int orderId);
	}
}
