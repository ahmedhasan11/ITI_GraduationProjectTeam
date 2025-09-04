using ITI_Hackathon.Data;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
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
        public AdminController(ApplicationDbContext db, IDoctorService doctorService) 
        {
            _db = db;
            _doctorservice = doctorService;
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

	}
}
