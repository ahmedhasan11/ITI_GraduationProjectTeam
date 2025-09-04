using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_Hackathon.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Thread))]
        public int ThreadId { get; set; }

        [Required]
        [ForeignKey(nameof(Sender))]
        public string SenderId { get; set; } = default!;

        [Required(ErrorMessage = "Message text is required")]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        [Url]
        public string? AttachmentUrl { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public ChatThread Thread { get; set; } = default!;
        public ApplicationUser Sender { get; set; } = default!;
    }
}
