namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class ChatThreadDto
    {
        public int Id { get; set; }
        public string PatientId { get; set; } = default!;
        public string DoctorId { get; set; } = default!;
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";

        // Add missing properties
        public DateTime UpdatedAt { get; set; }   // last update time of the thread
        public List<ChatMessageDto> Messages { get; set; } = new(); // messages inside the thread

    }


}
