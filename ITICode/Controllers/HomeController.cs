using System.Diagnostics;
using ITI_Hackathon.Models;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.Services;
using Medicine_Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITI_Hackathon.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IMedicineService _medicineService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IMedicineService medicineService, IDoctorService doctorService)
        {
            _logger = logger;
            _medicineService = medicineService;
            _doctorService = doctorService;

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
