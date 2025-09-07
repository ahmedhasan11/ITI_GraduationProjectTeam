namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class ChatThreadDto
    {
        public int Id { get; set; }
        public string PatientId { get; set; } = default!;
        public string DoctorId { get; set; } = default!;
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";
    }


}
