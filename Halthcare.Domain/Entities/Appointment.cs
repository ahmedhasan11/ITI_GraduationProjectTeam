
public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DoctorId { get; set; }
    public Guid? PatientId { get; set; } 
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsBooked { get; set; } = false;
    public DateTime? BookedAt { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsRated { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
}
