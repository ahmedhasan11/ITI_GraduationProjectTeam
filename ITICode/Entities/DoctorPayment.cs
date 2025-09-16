using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ITI_Hackathon.Entities
{
	public class Consultation
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[ForeignKey(nameof(Patient))]
		public string PatientId { get; set; } = default!;
		[Required]
		[ForeignKey(nameof(Doctor))]
		public string DoctorId { get; set; } = default!;
		public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

		// You can store the Stripe PaymentIntent ID for refunds/etc. later
		public string? StripePaymentIntentId { get; set; }

		// You can also store the amount paid
		[Column(TypeName = "decimal(18,2)")]
		public decimal AmountPaid { get; set; }

		// Navigation Properties
		public virtual ApplicationUser Patient { get; set; } = default!;
		public virtual ApplicationUser Doctor { get; set; } = default!;
	}

}
