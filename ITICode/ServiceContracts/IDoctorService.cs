using ITI_Hackathon.ServiceContracts.DTO;

namespace ITI_Hackathon.ServiceContracts
{
	public interface IDoctorService
	{
		/// <summary>
		/// returning List of Doctors which is approved so the admin can see them in 
		/// Doctors Section in the DashBoard
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<DoctorApprovedDTO>> GetApprovedDoctorsAsync();
		/// <summary>
		/// returning list of Doctors which is still pending which already created a request and waiting for the 
		/// admin to accept them or rejeect them and we still didnt make that when they is rejected or approved a message sent to 
		/// the doctor at gmail that tellk im what is his status
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<DoctorPendingDTO>> GetPendningDoctorsAsync();

		/// <summary>
		/// method for making a request if the doctor is registered as doctor so he is waiting for the 
		/// admin to accept his request thats why it is returning boolean true-->admin approved 
		/// false-->admin rejected
		/// </summary>
		/// <param name="doctorrequest">contains the data which doctor enetered in the register</param>
		/// <returns></returns>
		//Task<bool> AddDoctorAsync(DoctorApprovalRequestDTO? doctorrequest);
		Task<string> ApproveDoctorAsync(string userId);
		Task<string> RejectDoctorAsync(string userId);
		Task<bool> EditDoctorRoleAsync(DoctorEditRoleDTO dto);
		Task<string> DeleteDoctorAsync(string userId);
	}
}

