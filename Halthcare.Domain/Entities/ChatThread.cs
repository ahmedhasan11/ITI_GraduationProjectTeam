

namespace Healthcare.Domain.Entities
{
    public class ChatThread
    {
        public Guid Id { get; set; }= Guid.NewGuid();  
		public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ChatMessage> Messages { get; set; } = new();
    }
}
