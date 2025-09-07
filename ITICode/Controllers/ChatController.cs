using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ------------------------------
    // Get or Create Thread
    // ------------------------------
    [HttpPost("GetOrCreateThread")]
    public async Task<IActionResult> GetOrCreateThread([FromBody] ThreadRequestDto dto)
    {
        var patient = await _userManager.FindByIdAsync(dto.PatientId);
        var doctor = await _userManager.FindByIdAsync(dto.DoctorId);

        if (patient == null || doctor == null)
            return Ok(new { success = false, message = "Invalid user IDs." });

        var thread = await _context.ChatThreads
            .Include(t => t.Messages)
            .Include(t => t.Patient)
            .Include(t => t.Doctor)
            .FirstOrDefaultAsync(t => t.PatientId == dto.PatientId && t.DoctorId == dto.DoctorId);

        if (thread == null)
        {
            thread = new ChatThread
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                CreatedAt = DateTime.UtcNow
            };
            _context.ChatThreads.Add(thread);
            await _context.SaveChangesAsync();

            thread = await _context.ChatThreads
                .Include(t => t.Patient)
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == thread.Id);
        }

        return Ok(new
        {
            success = true,
            thread = new ChatThreadDto
            {
                Id = thread.Id,
                PatientId = thread.PatientId,
                DoctorId = thread.DoctorId,
                PatientName = thread.Patient?.FullName ?? "Unknown",
                DoctorName = thread.Doctor?.FullName ?? "Unknown"
            }
        });
    }

    // ------------------------------
    // Send Message
    // ------------------------------
    [HttpPost("SendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var sender = await _userManager.FindByIdAsync(dto.SenderId);
        var thread = await _context.ChatThreads.FindAsync(dto.ThreadId);

        if (sender == null || thread == null)
            return BadRequest(new { success = false, message = "Invalid sender or thread." });

        var message = new ChatMessage
        {
            ThreadId = dto.ThreadId,
            SenderId = dto.SenderId,
            Text = dto.Text,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

        return Ok(new
        {
            success = true,
            message = new
            {
                id = message.Id,
                threadId = message.ThreadId,
                senderId = message.SenderId,
                senderName = message.Sender?.UserName,
                text = message.Text,
                sentAt = message.SentAt
            }
        });
    }

    // ------------------------------
    // Get Messages
    // ------------------------------
    [HttpGet("GetMessages/{threadId}")]
    public async Task<IActionResult> GetMessages(int threadId)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.ThreadId == threadId)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .Select(m => new
            {
                m.Id,
                m.SenderId,
                SenderName = m.Sender.UserName,
                m.Text,
                m.SentAt
            })
            .ToListAsync();

        return Ok(messages);
    }
}