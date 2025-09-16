using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatThreadDto>> GetDoctorThreadsAsync(string doctorId)
        {
            var threads = await _context.ChatThreads
                .Include(t => t.Patient)
                .Include(t => t.Messages) // include messages
                .Where(t => t.DoctorId == doctorId)
                .ToListAsync();

            return threads.Select(t => new ChatThreadDto
            {
                Id = t.Id,
                DoctorId = t.DoctorId,
                PatientId = t.PatientId,
                PatientName = t.Patient?.FullName ?? "Unknown",
                DoctorName = t.Doctor?.FullName ?? "Unknown",

                // last update time = last message or DateTime.MinValue if no messages
                UpdatedAt = t.Messages.Any()
                    ? t.Messages.Max(m => m.SentAt)
                    : DateTime.MinValue,

                // map messages to DTO
                Messages = t.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    ThreadId = m.ThreadId,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    AttachmentUrl = m.AttachmentUrl,
                    SentAt = m.SentAt
                }).ToList()
            }).ToList();
        }

        public async Task<List<ChatThreadDto>> GetPatientThreadsAsync(string patientId)
        {
            var threads = await _context.ChatThreads
                .Include(t => t.Doctor)
                .Include(t => t.Messages) // include messages
                .Where(t => t.PatientId == patientId)
                .ToListAsync();

            return threads.Select(t => new ChatThreadDto
            {
                Id = t.Id,
                DoctorId = t.DoctorId,
                PatientId = t.PatientId,
                PatientName = t.Patient?.FullName ?? "Unknown",
                DoctorName = t.Doctor?.FullName ?? "Unknown",

                UpdatedAt = t.Messages.Any()
                    ? t.Messages.Max(m => m.SentAt)
                    : DateTime.MinValue,

                Messages = t.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    ThreadId = m.ThreadId,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    AttachmentUrl = m.AttachmentUrl,
                    SentAt = m.SentAt
                }).ToList()
            }).ToList();
        }


        public async Task<List<ChatMessageDto>> GetMessagesAsync(int threadId)
        {
            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.ThreadId == threadId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                ThreadId = m.ThreadId,
                SenderId = m.SenderId,
                Text = m.Text,
                AttachmentUrl = m.AttachmentUrl,
                SentAt = m.SentAt
            }).ToList();
        }

        public async Task<ChatMessageDto> SendMessageAsync(SendMessageDto dto)
        {
            var message = new ChatMessage
            {
                ThreadId = dto.ThreadId,
                Text = dto.Text,
                SenderId = dto.SenderId!,
                AttachmentUrl = dto.AttachmentUrl,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

            return new ChatMessageDto
            {
                Id = message.Id,
                ThreadId = message.ThreadId,
                SenderId = message.SenderId,
                Text = message.Text,
                AttachmentUrl = message.AttachmentUrl,
                SentAt = message.SentAt
            };
        }

        public async Task<ChatThreadDto> GetOrCreateThreadAsync(string patientId, string doctorId)
        {
            var thread = await _context.ChatThreads
                .Include(t => t.Patient)
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.PatientId == patientId && t.DoctorId == doctorId);

            if (thread == null)
            {
                thread = new ChatThread
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ChatThreads.Add(thread);
                await _context.SaveChangesAsync();
                await _context.Entry(thread).Reference(t => t.Patient).LoadAsync();
                await _context.Entry(thread).Reference(t => t.Doctor).LoadAsync();
            }

            return new ChatThreadDto
            {
                Id = thread.Id,
                PatientId = thread.PatientId,
                DoctorId = thread.DoctorId,
                PatientName = thread.Patient?.FullName ?? "Unknown",
                DoctorName = thread.Doctor?.FullName ?? "Unknown"
            };
        }
    }
}
