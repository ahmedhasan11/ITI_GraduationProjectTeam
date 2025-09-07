using System.Security.Claims;
using ITI_Hackathon.Entities;
using ITI_Hackathon.Models.ViewModels;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITI_Hackathon.Controllers
{
    [Authorize(Roles = "Doctor,Patient")]
    public class ChatPageController : Controller
    {
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatPageController(IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _chatService = chatService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? threadId)
        {
            var userId = _userManager.GetUserId(User);
            var isDoctor = User.IsInRole("Doctor");

            List<ChatThreadDto> threads = new List<ChatThreadDto>();

            if (isDoctor)
            {
                threads = await _chatService.GetDoctorThreadsAsync(userId);
            }
            else
            {
                threads = await _chatService.GetPatientThreadsAsync(userId);
            }

            ChatRoomDto? activeRoom = null;

            if (threadId.HasValue && threads.Any(t => t.Id == threadId.Value))
            {
                var messages = await _chatService.GetMessagesAsync(threadId.Value);
                activeRoom = new ChatRoomDto
                {
                    ThreadId = threadId.Value,
                    Messages = messages.Select(m => new ChatMessageViewDto
                    {
                        Text = m.Text,
                        SentAt = m.SentAt,
                        IsMine = m.SenderId == userId
                    }).ToList()
                };
            }
            else if (threads.Any())
            {
                threadId = threads.First().Id;
                var messages = await _chatService.GetMessagesAsync(threadId.Value);
                activeRoom = new ChatRoomDto
                {
                    ThreadId = threadId.Value,
                    Messages = messages.Select(m => new ChatMessageViewDto
                    {
                        Text = m.Text,
                        SentAt = m.SentAt,
                        IsMine = m.SenderId == userId
                    }).ToList()
                };
            }

            var model = new ChatIndexViewModel
            {
                Threads = threads,
                ActiveRoom = activeRoom
            };

            return View(model);
        }
    }
}