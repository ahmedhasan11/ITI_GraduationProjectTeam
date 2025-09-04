namespace ITI_Hackathon.ServiceContracts.DTO
{
	public class DoctorApprovedDTO
	{
			public string UserId { get; set; } = default!;
			public string FullName { get; set; } = default!;
			public string Email { get; set; } = default!;
			public string Specialty { get; set; } = default!;
			public double Rating { get; set; }
			public int CompletedChats { get; set; }
			public string LicenseNumber { get; set; }
			public bool IsApproved { get; set; }
	}
}
