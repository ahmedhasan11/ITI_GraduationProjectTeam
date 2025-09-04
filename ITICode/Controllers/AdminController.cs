using ITI_Hackathon.Data;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using ITI_Hackathon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IDoctorService _doctorservice;

        private readonly IMedicineService _medicineservice;

        public AdminController(ApplicationDbContext db, IDoctorService doctorService, IMedicineService medicineService) 
        {
            _db = db;
            _doctorservice = doctorService;
            _medicineservice = medicineService;


        }

        // GET: /Admin/PendingDoctors
        public async Task<IActionResult> PendingDoctors()
        {


            IEnumerable<DoctorPendingDTO> PendingDoctors = await _doctorservice.GetPendningDoctorsAsync();
;
           return View(PendingDoctors);
        }

        //GET:/Admin/ApprovedDoctors

        public async Task<IActionResult> ApprovedDoctors()
        {


            IEnumerable<DoctorApprovedDTO> ApprovedDoctors = await _doctorservice.GetApprovedDoctorsAsync();

            return View(ApprovedDoctors);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDoctor(string userID)
        {
             string result=await _doctorservice.ApproveDoctorAsync(userID);
            ViewBag.Message = result;
            return RedirectToAction("PendingDoctors");
        }
        [HttpPost]
		public async Task<IActionResult> RejectDoctor(string userID)
		{
			string result = await _doctorservice.RejectDoctorAsync(userID);
			ViewBag.Message = result;
			return RedirectToAction("PendingDoctors");
		}


		/*@foreach (var doctor in Model)
{
    <tr>
        <td>@doctor.FullName</td>
        <td>@doctor.Email</td>
        <td>@doctor.Specialty</td>
        <td>
            <form asp-action="ApproveDoctor" method="post" style="display:inline;">
                <input type="hidden" name="userId" value="@doctor.UserId" />
                <button type="submit" class="btn btn-success">Approve</button>
            </form>
            <form asp-action="RejectDoctor" method="post" style="display:inline;">
                <input type="hidden" name="userId" value="@doctor.UserId" />
                <button type="submit" class="btn btn-danger">Reject</button>
            </form>
        </td>
    </tr>
}*/


        public async Task<IActionResult> EditDoctorRoleAsync(DoctorEditRoleDTO doctorEditRoleDTO)
        {
            bool changedoctorrole=await _doctorservice.EditDoctorRoleAsync(doctorEditRoleDTO);

            return RedirectToAction("ApprovedDoctors");
        }

        public async Task<IActionResult> DeleteDoctorAsync(string userID)
        {
            var result=await _doctorservice.DeleteDoctorAsync(userID);
            ViewBag.message = result;
            return RedirectToAction("ApprovedDoctors");
        }


        //GET: /Medicine
        public async Task<IActionResult> Index()
        {
            var medicines = await _medicineservice.GetAllMedicineAsync();
            return View(medicines);
        }




        // GET: /Medicine/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _medicineservice.GetMedicineByIdAsync(id);
            if (medicine == null) return NotFound();

            return View(medicine);
        }

        // GET: /Medicine/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Medicine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineAddRequestDto request)
        {
            if (!ModelState.IsValid) return View(request);

            var result = await _medicineservice.AddMedicineAsync(request);
            if (result != null)
            {
                TempData["Success"] = "Medicine added successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to add medicine.";
            return View(request);
        }

        // GET: /Medicine/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _medicineservice.GetMedicineByIdAsync(id);
            if (medicine == null) return NotFound();

            var dto = new MedicineUpdateRequestDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Category = medicine.Category,
                Description = medicine.Description,
                Price = medicine.Price,
                Stock = medicine.Stock,
                ImageUrl = medicine.ImageUrl
            };

            return View(dto);
        }

        // POST: /Medicine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicineUpdateRequestDto request)
        {
            if (!ModelState.IsValid) return View(request);

            var result = await _medicineservice.UpdateMedicineAsync(request);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return View(request);
        }


        // GET: /Medicine/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineservice.GetMedicineByIdAsync(id);
            if (medicine == null) return NotFound();

            var dto = new MedicineDeleteRequestDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                ImageUrl = medicine.ImageUrl
            };

            return View(dto);
        }


        // POST: /Medicine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(MedicineDeleteRequestDto request)
        {
            var result = await _medicineservice.DeleteMedicineAsync(request);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Medicine/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                TempData["Error"] = "Please enter a search term.";
                return RedirectToAction(nameof(Index));
            }

            var results = await _medicineservice.SearchMedicineAsync(searchTerm);

            if (!results.Any())
            {
                TempData["Info"] = "No medicines found matching your search.";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", results);
        }



    }
}
