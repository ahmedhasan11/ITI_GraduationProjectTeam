namespace ITI_Hackathon.ServiceContracts.DTO
{
	public class DoctorPendingDTO
	{
		public string UserId { get; set; } = default!;   // comes from ApplicationUser
		public string FullName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string Specialty { get; set; } = default!;
		public string? Bio { get; set; }
		public string? LicenseNumber { get; set; }
	}
}
