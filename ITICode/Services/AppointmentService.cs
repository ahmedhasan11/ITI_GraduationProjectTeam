using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentDto> AddAppointmentAsync(AppointmentDto dto)
        {
            try
            {
                Console.WriteLine($"Adding appointment for doctor: {dto.DoctorId}");
                Console.WriteLine($"Date: {dto.AppointmentDate}, Time: {dto.StartTime} - {dto.EndTime}");

                var appointment = new Appointment
                {
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsBooked = false,
                    IsCompleted = false,
                    IsRated = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Appointment added successfully with ID: {appointment.Id}");

                dto.Id = appointment.Id;
                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding appointment: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<bool> BookAppointmentAsync(int appointmentId, string patientId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null || appointment.IsBooked) return false;

            appointment.PatientId = patientId;
            appointment.IsBooked = true;
            appointment.BookedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        //public async Task<bool> RateAppointmentAsync(int appointmentId, int rating, string comment, string patientId)
        //{
        //    var appointment = await _context.Appointments
        //        .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patientId);

        //    if (appointment == null || !appointment.IsCompleted || appointment.IsRated || rating < 1 || rating > 5)
        //        return false;

        //    var ratingEntity = new Rating
        //    {
        //        DoctorId = appointment.DoctorId,
        //        PatientId = patientId,
        //        Stars = rating,
        //        Comment = comment,
        //        AppointmentId = appointmentId,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.Ratings.Add(ratingEntity);
        //    appointment.IsRated = true;

        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<List<AppointmentDto>> GetAvailableAppointmentsAsync(string doctorId = null)
        {
            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => !a.IsBooked && a.AppointmentDate >= DateTime.Today);

            if (!string.IsNullOrEmpty(doctorId))
            {
                query = query.Where(a => a.DoctorId == doctorId);
            }

            return await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsBooked = a.IsBooked
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(string doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient != null ? a.Patient.FullName : null,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsBooked = a.IsBooked,
                    IsCompleted = a.IsCompleted,
                    IsRated = a.IsRated,
                    CreatedAt = a.CreatedAt,
                    BookedAt = a.BookedAt,
                    CompletedAt = a.CompletedAt
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(string patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    PatientId = a.PatientId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsBooked = a.IsBooked,
                    IsCompleted = a.IsCompleted,
                    IsRated = a.IsRated,
                    CreatedAt = a.CreatedAt,
                    BookedAt = a.BookedAt,
                    CompletedAt = a.CompletedAt
                })
                .ToListAsync();
        }

        //public async Task<bool> AddRatingAsync(CreateRatingDto dto, string patientId)
        //{
        //    try
        //    {
        //        var rating = new Rating
        //        {
        //            DoctorId = dto.DoctorId,
        //            PatientId = patientId,
        //            Stars = dto.Stars,
        //            Comment = dto.Comment,
        //            AppointmentId = dto.AppointmentId,
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        _context.Ratings.Add(rating);

        //        if (dto.AppointmentId.HasValue)
        //        {
        //            var appointment = await _context.Appointments.FindAsync(dto.AppointmentId.Value);
        //            if (appointment != null)
        //            {
        //                appointment.IsRated = true;
        //            }
        //        }

        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public async Task<List<RatingDto>> GetDoctorRatingsAsync(string doctorId)
        //{
        //    return await _context.Ratings
        //        .Include(r => r.Patient)
        //        .Where(r => r.DoctorId == doctorId)
        //        .OrderByDescending(r => r.CreatedAt)
        //        .Select(r => new RatingDto
        //        {
        //            Id = r.Id,
        //            DoctorId = r.DoctorId,
        //            PatientId = r.PatientId,
        //            PatientName = r.Patient.FullName,
        //            Stars = r.Stars,
        //            Comment = r.Comment,
        //            CreatedAt = r.CreatedAt,
        //            AppointmentId = r.AppointmentId
        //        })
        //        .ToListAsync();
        //}

        //public async Task<double> GetDoctorAverageRatingAsync(string doctorId)
        //{
        //    var ratings = await _context.Ratings
        //        .Where(r => r.DoctorId == doctorId)
        //        .ToListAsync();

        //    if (!ratings.Any()) return 0;

        //    return Math.Round(ratings.Average(r => r.Stars), 1);
        //}

        public async Task<bool> CompleteAppointmentAsync(int appointmentId, string doctorId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId);

            if (appointment == null || !appointment.IsBooked || appointment.IsCompleted)
                return false;

            appointment.IsCompleted = true;
            appointment.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId, string doctorId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId && !a.IsBooked);

            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
