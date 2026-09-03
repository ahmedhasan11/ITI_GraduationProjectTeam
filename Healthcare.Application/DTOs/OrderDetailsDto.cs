namespace Healthcare.Application.DTOs
{
	public class OrderDetailsDto
	{
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Status { get; set; } = default!;
		public decimal Total { get; set; }

		public List<OrderItemDto> Items { get; set; } = new();
	}
}
