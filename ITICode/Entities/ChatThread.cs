using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_Hackathon.Entities
{
    public class ChatThread
    {
        public int Id { get; set; }

        public string PatientId { get; set; } = default!;
        public ApplicationUser Patient { get; set; } = default!;

        public string DoctorId { get; set; } = default!;
        public ApplicationUser Doctor { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ChatMessage> Messages { get; set; } = new();
    }
}
