using ITI_Hackathon.ServiceContracts.DTO;
namespace ITI_Hackathon.Models.ViewModels
{
    public class ChatIndexViewModel
    {
        public List<ServiceContracts.DTO.ChatThreadDto> Threads { get; set; } = new();
        public ChatRoomDto? ActiveRoom { get; set; }
    }
}
