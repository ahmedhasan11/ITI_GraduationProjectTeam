using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Services
{
	public class ConsultationService:IConsultationService
	{
		private readonly ApplicationDbContext _context;

		public ConsultationService(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<bool> HasPaidForConsultationAsync(string patientId, string doctorId)
		{
			return await _context.DoctorPayments
				.AnyAsync(c => c.PatientId == patientId && c.DoctorId == doctorId);
		}
		public async Task<ConsultationPaymentDto> CreateConsultationPaymentAsync(string patientId, string doctorId)
		{
			// In a real app, you might get this from a configuration or database
			decimal consultationFee = 50.00m;

			var doctor = await _context.Users.FindAsync(doctorId);
			if (doctor == null) throw new ArgumentException("Doctor not found");

			return new ConsultationPaymentDto
			{
				Amount = consultationFee,
				DoctorName = doctor.FullName
				// SessionId and SessionUrl will be set by the PaymentController
			};
		}
		public async Task<bool> RecordSuccessfulConsultationAsync(string patientId, string doctorId, string paymentIntentId, decimal amount)
		{
			var consultation = new Consultation
			{
				PatientId = patientId,
				DoctorId = doctorId,
				StripePaymentIntentId = paymentIntentId,
				AmountPaid = amount,
				PaymentDate = DateTime.UtcNow
			};

			_context.DoctorPayments.Add(consultation);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
