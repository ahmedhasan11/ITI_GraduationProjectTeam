namespace Healthcare.Application.DTOs
{
	public class ConsultationPaymentDto
	{
		public string SessionId { get; set; } = default!;
		public string SessionUrl { get; set; } = default!;
		public decimal Amount { get; set; }
		public string DoctorName { get; set; } = default!;
	}
}
