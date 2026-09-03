namespace Healthcare.Application.DTOs
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ThreadId { get; set; }
        public string SenderId { get; set; } = default!;
        public string Text { get; set; } = "";
        public string? AttachmentUrl { get; set; }
        public DateTime SentAt { get; set; }
    }

}
