namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class SendMessageDto
    {
        public int ThreadId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string SenderId { get; set; } = default!;
        public string? AttachmentUrl { get; set; }
    }

}
