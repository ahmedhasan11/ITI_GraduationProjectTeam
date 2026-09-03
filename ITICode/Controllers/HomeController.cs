using System.Diagnostics;
using ITI_Hackathon.Models;
using Healthcare.Application.ServiceContracts;
using Healthcare.Application.Services;
using Healthcare.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Healthcare.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IMedicineService _medicineService;
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IMedicineService medicineService, IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _logger = logger;
            _medicineService = medicineService;
            _doctorService = doctorService;
            _appointmentService = appointmentService;

        }

        public async Task<IActionResult> Index()
        {
            HomeIndexViewModel vm = new HomeIndexViewModel
            {
                Medicines = await _medicineService.GetAllMedicineAsync(),
                Doctors = await _doctorService.GetApprovedDoctorsAsync()
            };

            return View(vm);
        }

        public async Task< IActionResult> GetMedicines()
        {
            IEnumerable<MedicineListDto> Medicines = await _medicineService.GetAllMedicineAsync();

			return View(Medicines);
        }

        public async Task<IActionResult> GetDoctors()
        {
            IEnumerable<DoctorApprovedDTO> Doctors = await _doctorService.GetApprovedDoctorsAsync();

			return View(Doctors);
        }
        [Authorize]
        public async Task<IActionResult> DoctorDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Doctor UserId is required");

            var doctor = await _doctorService.GetDoctorDetails(userId);

            if (doctor == null)
                return NotFound("Doctor not found");

            var availableAppointments = await _appointmentService.GetAvailableAppointmentsAsync(userId);
            doctor.AvailableAppointments = availableAppointments;

            return View(doctor);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
