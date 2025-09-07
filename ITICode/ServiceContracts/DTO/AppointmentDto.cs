namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public string DoctorId { get; set; } = default!;
        public string? PatientId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsBooked { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsRated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? BookedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string FormattedTime => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        public string FormattedDate => AppointmentDate.ToString("dd MMM yyyy");
        public bool IsAvailable => !IsBooked && !IsCompleted;
    }
}
