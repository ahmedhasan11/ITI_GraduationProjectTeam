namespace ITI_Hackathon.ServiceContracts.DTO
{
	public class DoctorApprovalRequestDTO
	{
		public string UserId { get; set; } = default!;
		public bool Approve { get; set; }   // true = approve, false = rejec
	}
}
