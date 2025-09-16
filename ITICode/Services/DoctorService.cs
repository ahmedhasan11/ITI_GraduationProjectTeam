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
				.Where(d => d.IsApproved == true && d.User.IsDoctor == true)
				.Select(d => new DoctorApprovedDTO
				{
					UserId = d.UserId,
					FullName = d.User.FullName,
					Email = d.User.Email,
					Specialty = d.Specialty,
					Rating = d.Rating,
					CompletedChats = d.CompletedChats,
					IsApproved = d.IsApproved,
					LicenseNumber = d.LicenseNumber,
					Bio = d.Bio
				}).ToListAsync();

			return DoctorsApproved;
		}

		public async Task<IEnumerable<DoctorPendingDTO>> GetPendningDoctorsAsync()
		{
			IEnumerable<DoctorPendingDTO> DoctorsPending = await _context.Doctors.Include(d => d.User)
				.Where(d => d.IsApproved == false)
				.Select(d => new DoctorPendingDTO()
				{
					UserId = d.UserId,
					FullName = d.User.FullName,
					Email = d.User.Email,
					Specialty = d.Specialty,
					Bio = d.Bio,
					LicenseNumber = d.LicenseNumber
				}).ToListAsync();
			return DoctorsPending;
		}
		public async Task<string> ApproveDoctorAsync(string userId)
		{
			var doctor = await _context.Doctors
				.Include(d => d.User)
				.FirstOrDefaultAsync(d => d.UserId == userId);

			if (doctor == null)
			{
				return "Doctor not found";
			}
			doctor.IsApproved = true;
			//_context.Doctors.Update(doctor); 
			await _context.SaveChangesAsync();

			//we need to make it as a popup message and also have to be sent to the gamil user

			return $"Doctor {doctor.User.FullName} has been approved successfully.";

		}

		public async Task<string> RejectDoctorAsync(string userId)
		{
			var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == userId);
			if (doctor == null)
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
			var doctor = await _context.Doctors
				.Include(d => d.User)
				.FirstOrDefaultAsync(d => d.UserId == userId);

			if (doctor == null)
			{
				return "Doctor not found";
			}
			var threads = await _context.ChatThreads
				.Where(t => t.DoctorId == userId)
				.ToListAsync();

			if (threads.Any())
			{
				_context.ChatThreads.RemoveRange(threads);
			}
			var doctorPayments = await _context.DoctorPayments
				.Where(p => p.DoctorId == userId)
				.ToListAsync();

			if (doctorPayments.Any())
			{
				_context.DoctorPayments.RemoveRange(doctorPayments);
			}

			var appointments = await _context.Appointments
				.Where(a => a.DoctorId == userId)
				.ToListAsync();

			if (appointments.Any())
			{
				_context.Appointments.RemoveRange(appointments);
			}

			_context.Doctors.Remove(doctor);

			var user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}

			await _context.SaveChangesAsync();

			return $"Doctor {doctor.User.FullName} and all related data (chats, payments, appointments) have been completely removed.";
		}




		//we need to convert thedoctor profile obj -->to patientprofile if changedtopatient
		public async Task<bool> EditDoctorRoleAsyncc(DoctorEditRoleDTO dto)
		{
			var user = await _userManager.FindByIdAsync(dto.UserId);
			if (user == null)
			{
				return false;
			}

			var currentRoles = await _userManager.GetRolesAsync(user);
			await _userManager.RemoveFromRolesAsync(user, currentRoles);

			if (dto.NewRole == "Doctor")
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
		public async Task<DoctorApprovedDTO> GetDoctorDetails(string userId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return null;

            // Map to DTO
            var doctorDto = new DoctorApprovedDTO
            {
                UserId = doctor.UserId,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                Bio = doctor.Bio,
                Specialty = doctor.Specialty,
                LicenseNumber = doctor.LicenseNumber,
                Rating = doctor.Rating,
                AvailableAppointments = new List<AppointmentDto>() 
            };

            return doctorDto;
        }
	}
}
