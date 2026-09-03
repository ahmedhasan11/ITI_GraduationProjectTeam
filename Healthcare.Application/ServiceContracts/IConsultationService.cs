using Healthcare.Application.DTOs;

namespace Healthcare.Application.ServiceContracts
{
	public interface IConsultationService
	{
		Task<bool> HasPaidForConsultationAsync(string patientId, string doctorId);
		Task<ConsultationPaymentDto> CreateConsultationPaymentAsync(string patientId, string doctorId);
		Task<bool> RecordSuccessfulConsultationAsync(string patientId, string doctorId, string paymentIntentId, decimal amount);
	}
}
