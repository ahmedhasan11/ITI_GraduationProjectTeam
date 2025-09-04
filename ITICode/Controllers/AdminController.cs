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

		#region Doctor Action Methods


		#region Doctor List Action Methods


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
		#endregion

		#region Approve& Reject Doctor Action Methods


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
		#endregion

		#region EditRole& DeleteDoctor


		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctorRoleAsyncc(DoctorEditRoleDTO doctorEditRoleDTO)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("ApprovedDoctors"); // or handle errors
			}
			bool changedoctorrole=await _doctorservice.EditDoctorRoleAsyncc(doctorEditRoleDTO);

            return RedirectToAction("ApprovedDoctors");
        }

        public async Task<IActionResult> DeleteDoctorAsync(string? userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                return NotFound("userID not found");
            }
            var result=await _doctorservice.DeleteDoctorAsync(userID);
            ViewBag.message = result;
            return RedirectToAction("ApprovedDoctors");
        }
		#endregion
		#endregion


		#region Medicine Action Methods 


		//GET: /Admin/GetAllMedicines
		public async Task<IActionResult> GetAllMedicines() //-->Done
        {
            var medicines = await _medicineservice.GetAllMedicineAsync();
            return View(medicines);
		} 


		  // GET: /Admin/GetMedicineByID/id
		public async Task<IActionResult> GetMedicineByID(int? id) //-->Done
        {
            if (id==null)
            {
                return NotFound("id is empty");
            }
            //checkfotthat id.value
            var medicine = await _medicineservice.GetMedicineByIdAsync(id.Value);
            if (medicine == null) return NotFound();

            return View(medicine);
        } 


		#region Add Medicine -->Done  


		// GET: /Medicine/Create
		[HttpGet]
		public IActionResult Create()
        {
            return View("AddMedicine");
        }

        // POST: /Medicine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineAddRequestDto medicinerequest)
        {
            //check again after
            //if (!ModelState.IsValid) return View(request);
            if (medicinerequest==null)
            {
				return NotFound("Meidicne is not added");
			}

            MedicineAddResponseDto medicineresponse = await _medicineservice.AddMedicineAsync(medicinerequest);
			return RedirectToAction("GetAllMedicines");

        }
		#endregion //-->Done 

		#region Edit Medicine -->Done


		// GET: /Admin/EditMedicine/id
		[HttpGet]
        public async Task<IActionResult> EditMedicine(int? id)
        {
            if (id==null)
            {
                return NotFound("id is empty");
            }
            var medicine = await _medicineservice.GetMedicineByIdAsync(id.Value);
            if (medicine == null) return NotFound("medicine not found");

            MedicineUpdateRequestDto dto = new MedicineUpdateRequestDto
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

        // POST: /Admin/Edit/dto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicineUpdateRequestDto request)
        {
            //if (!ModelState.IsValid) return View(request);

            MedicineUpdateResponseDto medicineresponse = await _medicineservice.UpdateMedicineAsync(request);
            if (medicineresponse.Success==false)
            {
				TempData["Error"] = medicineresponse.Message;
				return View("Edit",request);
			}

			TempData["Success"] = medicineresponse.Message;
			return RedirectToAction("GetAllMedicines");
		}
        #endregion -->Done -->Done


        #region Delete Medicine -->Done       
        public async Task<IActionResult> DeleteMedicinee(int? id)
        {
            if (id==null)
            {
                return NotFound("medicinerequest is empty");
            }
            string result = await _medicineservice.DeleteMedicineAsync(id.Value);

            ViewBag.Message=result;
		    return RedirectToAction("GetAllMedicines");


		}
        #endregion

        #region Search Medicine -->Done


        // GET: /Admin/Search
        [HttpGet]
		public async Task<IActionResult> Search(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                TempData["Error"] = "Please enter a search term.";
                return RedirectToAction("GetAllMedicines");
            }

            IEnumerable<MedicineListDto> Filteredmedicines = await _medicineservice.SearchMedicineAsync(searchTerm);

            if (!Filteredmedicines.Any())
            {
                TempData["Info"] = "No medicines found matching your search.";
                //return RedirectToAction("GetAllMedicines");
                return View("GetAllMedicines", Filteredmedicines);
            }

            return View("GetAllMedicines",Filteredmedicines);
        }
		#endregion


		#endregion

	}
}
