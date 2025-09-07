namespace ITI_Hackathon.ServiceContracts.DTO
{
	public class OrderDto
	{
		public int OrderId { get; set; }

		public string PatientId { get; set; }

		public DateTime CreatedAt { get; set; } 

		public string Status { get; set; }

		public decimal Total { get; set; }
		public List<OrderItemDto> Items { get; set; } = new();
	}
}
