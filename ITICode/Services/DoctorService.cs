using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Services
{
	public class DoctorService : IDoctorService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;
		public DoctorService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<IEnumerable<DoctorApprovedDTO>> GetApprovedDoctorsAsync()
		{
            IEnumerable<DoctorApprovedDTO> DoctorsApproved = await _context.Doctors.Include(d => d.User)
				.Where(d => d.IsApproved == true)
				.Select(d => new DoctorApprovedDTO
				{
					UserId = d.UserId,
					FullName = d.User.FullName,
					Email = d.User.Email,
					Specialty = d.Specialty,
					Rating = d.Rating,
					CompletedChats = d.CompletedChats
				}).ToListAsync();

			return DoctorsApproved;
		}

		public async Task<IEnumerable<DoctorPendingDTO>> GetPendningDoctorsAsync()
		{
            IEnumerable<DoctorPendingDTO> DoctorsPending = await _context.Doctors.Include(d => d.User)
				.Where(d=>d.IsApproved==false)
				.Select(d => new DoctorPendingDTO()
				{
					UserId=d.UserId,
					FullName=d.User.FullName,
					Email=d.User.Email,
					Specialty=d.Specialty,
					Bio=d.Bio,
					LicenseNumber=d.LicenseNumber
				}).ToListAsync();
			return DoctorsPending;
		}
		public async Task<string> ApproveDoctorAsync(string userId)
		{
			var doctor = await _context.Doctors
				.Include(d => d.User)
				.FirstOrDefaultAsync(d => d.UserId == userId);

			if (doctor==null)
			{
				return "Doctor not found";
			}
			doctor.IsApproved = true;
			//_context.Doctors.Update(doctor); 
			await _context.SaveChangesAsync();

			//we need to make it as a popup message and also have to be sent to the gamil user

			return $"Doctor {doctor.User.FullName} has been approved successfully.";


// هنا ممكن تبعت إيميل Approved return true;

		}

		public async Task<string> RejectDoctorAsync(string userId)
		{
			var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == userId);
			if (doctor==null)
			{
				return "doctor not found";
			}

			doctor.IsApproved = false;
			await _context.SaveChangesAsync();

			return $"Doctor {doctor.User.FullName} has been rejected.";

			//we need to make it as a popup message and also have to be sent to the gamil user
		}
		public async Task<string> DeleteDoctorAsync(string userId)
		{
			var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
			if (doctor==null)
			{
				return "doctor not found";
			}
			 _context.Doctors.Remove(doctor);
			var user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}
			await _context.SaveChangesAsync();

			return $"Doctor {doctor.User.FullName} has been completely removed from the system.";
		}

		//we need to convert thedoctor profile obj -->to patientprofile if changedtopatient
		public async Task<bool> EditDoctorRoleAsync(DoctorEditRoleDTO dto)
		{
			var user =await  _userManager.FindByIdAsync(dto.UserId);
			if (user==null)
			{
				return false;
			}

			var currentRoles = await _userManager.GetRolesAsync(user);
			await _userManager.RemoveFromRolesAsync(user, currentRoles);

			if (dto.NewRole=="Doctor")
			{
				await _userManager.AddToRoleAsync(user, "Doctor");
				user.IsDoctor = true;
				user.IsPatient = false;
			}
			else if (dto.NewRole == "Patient")
			{
				await _userManager.AddToRoleAsync(user, "Patient");
				user.IsPatient = true;
				user.IsDoctor = false;
			}


			await _userManager.UpdateAsync(user);
			return true;
		}




	}
}
