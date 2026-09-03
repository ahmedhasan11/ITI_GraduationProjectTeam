namespace Healthcare.Presentation.Models.ViewModels
{
    public class ChatRoomDto
    {
        public int ThreadId { get; set; }
        public List<ChatMessageViewDto> Messages { get; set; } = new();
    }

}
