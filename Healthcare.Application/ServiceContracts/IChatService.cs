using Healthcare.Application.DTOs;

namespace Healthcare.Application.ServiceContracts
{
    public interface IChatService
    {
        Task<List<ChatThreadDto>> GetDoctorThreadsAsync(string doctorId);
        Task<List<ChatThreadDto>> GetPatientThreadsAsync(string patientId);
        Task<List<ChatMessageDto>> GetMessagesAsync(int threadId);
        Task<ChatMessageDto> SendMessageAsync(SendMessageDto dto);
        Task<ChatThreadDto> GetOrCreateThreadAsync(string patientId, string doctorId);
    }
}
    