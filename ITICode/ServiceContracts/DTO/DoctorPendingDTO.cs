namespace ITI_Hackathon.ServiceContracts.DTO
{
<<<<<<< HEAD
    public class DoctorPendingDTO
=======
	public class DoctorPendingDTO
>>>>>>> 830182cce6b2f62feaed66bd10da1375357aecd8
	{
		public string UserId { get; set; } = default!;   // comes from ApplicationUser
		public string FullName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string Specialty { get; set; } = default!;
		public string? Bio { get; set; }
		public string? LicenseNumber { get; set; }
	}
}
