using Healthcare.Application.DTOs;
namespace Healthcare.Presentation.Models.ViewModels
{
    public class ChatIndexViewModel
    {
        public List<ChatThreadDto> Threads { get; set; } = new();
        public ChatRoomDto? ActiveRoom { get; set; }
    }
}
