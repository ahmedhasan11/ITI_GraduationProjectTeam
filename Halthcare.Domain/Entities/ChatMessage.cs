

namespace Healthcare.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ThreadId { get; set; }
        public Guid SenderId { get; set; } = default!;
        public string Text { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public ChatThread Thread { get; set; } = default!;
    }
}
