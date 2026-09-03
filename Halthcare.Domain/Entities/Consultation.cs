

namespace Healthcare.Domain.Entities
{
	public class Consultation
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid PatientId { get; set; }
		public Guid DoctorId { get; set; }
		public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
		// You can store the Stripe PaymentIntent ID for refunds/etc. later
		public string? StripePaymentIntentId { get; set; }
		// You can also store the amount paid
		public decimal AmountPaid { get; set; }

	}

}
