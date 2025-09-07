namespace ITI_Hackathon.Models.ViewModels
{
    public class ChatMessageViewDto
    {
        public string Text { get; set; } = "";
        public DateTime SentAt { get; set; }
        public bool IsMine { get; set; }
    }

}
