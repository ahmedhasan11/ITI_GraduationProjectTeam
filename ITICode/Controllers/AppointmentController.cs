using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ITI_Hackathon.Data;

namespace ITI_Hackathon.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _service;
        private readonly UserManager<Entities.ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AppointmentController(IAppointmentService service,
                                  UserManager<Entities.ApplicationUser> userManager,
                                  ApplicationDbContext context)
        {
            _service = service;
            _userManager = userManager;
            _context = context;
        }

        // Doctor: Add new appointment
        // GET : /Doctor/Add
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AppointmentDto dto)
        {
            try
            {
                Console.WriteLine($"Received appointment data: {JsonSerializer.Serialize(dto)}");

                var doctorId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(doctorId))
                {
                    return BadRequest(new { success = false, message = "User not authenticated" });
                }

                dto.DoctorId = doctorId;

                if (dto.AppointmentDate == default)
                {
                    return BadRequest(new { success = false, message = "Appointment date is required" });
                }

                if (dto.StartTime == default || dto.EndTime == default)
                {
                    return BadRequest(new { success = false, message = "Start time and end time are required" });
                }

                if (dto.EndTime <= dto.StartTime)
                {
                    return BadRequest(new { success = false, message = "End time must be after start time" });
                }

                if (dto.AppointmentDate < DateTime.Today)
                {
                    return BadRequest(new { success = false, message = "Appointment date cannot be in the past" });
                }

                var result = await _service.AddAppointmentAsync(dto);

                return Ok(new { success = true, message = "Appointment added successfully", data = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add method: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "An error occurred", error = ex.Message });
            }
        }

        // Doctor: View all appointments with filtering and search
        // GET : /Doctor/MyAppointments
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyAppointments(string filter = "all", string searchTerm = "")
        {
            try
            {
                var doctorId = _userManager.GetUserId(User);
                var appointments = await _service.GetDoctorAppointmentsAsync(doctorId);

                if (!string.IsNullOrEmpty(filter) && filter != "all")
                {
                    appointments = filter switch
                    {
                        "available" => appointments.Where(a => !a.IsBooked).ToList(),
                        "booked" => appointments.Where(a => a.IsBooked && !a.IsCompleted).ToList(),
                        "completed" => appointments.Where(a => a.IsCompleted).ToList(),
                        _ => appointments
                    };
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    appointments = appointments
                        .Where(a =>
                            (a.PatientName != null && a.PatientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                            a.AppointmentDate.ToString("dd MMM yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            a.StartTime.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                }

                ViewBag.Filter = filter;
                ViewBag.SearchTerm = searchTerm;

                return View(appointments);
            }
            catch (Exception ex)
            {
                // Handle error
                TempData["ErrorMessage"] = "An error occurred while loading appointments.";
                return View(new List<AppointmentDto>());
            }
        }

        // Patient: View available appointments
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Available(string doctorId = null)
        {
            var appointments = await _service.GetAvailableAppointmentsAsync(doctorId);
            return View(appointments);
        }

        // Patient: Book appointment
        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> Book(int appointmentId)
        {
            var patientId = _userManager.GetUserId(User);
            bool success = await _service.BookAppointmentAsync(appointmentId, patientId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Appointment not available or already booked.";
                return RedirectToAction("Available");
            }

            TempData["SuccessMessage"] = "Appointment booked successfully!";
            return RedirectToAction("PatientAppointments");
        }

        // Patient: View their appointments
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> PatientAppointments()
        {
            var patientId = _userManager.GetUserId(User);
            var appointments = await _service.GetPatientAppointmentsAsync(patientId);
            return View(appointments);
        }

        // Doctor: Complete appointment
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            try
            {
                var doctorId = _userManager.GetUserId(User);
                var success = await _service.CompleteAppointmentAsync(appointmentId, doctorId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Appointment marked as completed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to complete appointment. It may not exist or is already completed.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while completing the appointment.";
            }

            return RedirectToAction("MyAppointments");
        }

        // Doctor: Delete appointment (only available appointments)
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            try
            {
                var doctorId = _userManager.GetUserId(User);
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId && !a.IsBooked);

                if (appointment != null)
                {
                    _context.Appointments.Remove(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Appointment slot deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot delete booked or non-existent appointment.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the appointment.";
            }

            return RedirectToAction("MyAppointments");
        }

        // Patient: Rate appointment
        //[Authorize(Roles = "Patient")]
        //[HttpPost]
        //public async Task<IActionResult> Rate(int appointmentId, int stars, string comment)
        //{
        //    try
        //    {
        //        var patientId = _userManager.GetUserId(User);
        //        //bool success = await _service.RateAppointmentAsync(appointmentId, stars, comment, patientId);

        //        if (success)
        //        {
        //            TempData["SuccessMessage"] = "Thank you for your rating!";
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Cannot rate this appointment. It may not be completed or already rated.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "An error occurred while submitting your rating.";
        //    }

        //    return RedirectToAction("PatientAppointments");
        //}

        // Get doctor ratings
        //[AllowAnonymous]
        //[HttpGet("Doctor/{doctorId}/Ratings")]
        //public async Task<IActionResult> GetDoctorRatings(string doctorId)
        //{
        //    var ratings = await _service.GetDoctorRatingsAsync(doctorId);
        //    return Ok(ratings);
        //}

        // Get doctor average rating
        //[AllowAnonymous]
        //[HttpGet("Doctor/{doctorId}/AverageRating")]
        //public async Task<IActionResult> GetDoctorAverageRating(string doctorId)
        //{
        //    var average = await _service.GetDoctorAverageRatingAsync(doctorId);
        //    return Ok(new { averageRating = average });
        //}

        // Search appointments (for doctor)
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var doctorId = _userManager.GetUserId(User);
            var appointments = await _service.GetDoctorAppointmentsAsync(doctorId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                appointments = appointments
                    .Where(a =>
                        (a.PatientName != null && a.PatientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        a.AppointmentDate.ToString("dd MMM yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            return View("MyAppointments", appointments);
        }
    }
}