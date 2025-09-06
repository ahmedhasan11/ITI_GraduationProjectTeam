using ITI_Hackathon.ServiceContracts.DTO;

namespace ITI_Hackathon.ServiceContracts
{
	public interface IConsultationService
	{
		Task<bool> HasPaidForConsultationAsync(string patientId, string doctorId);
		Task<ConsultationPaymentDto> CreateConsultationPaymentAsync(string patientId, string doctorId);
		Task<bool> RecordSuccessfulConsultationAsync(string patientId, string doctorId, string paymentIntentId, decimal amount);
	}
}
