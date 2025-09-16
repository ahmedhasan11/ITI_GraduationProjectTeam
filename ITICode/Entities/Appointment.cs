using System.ComponentModel.DataAnnotations;
using ITI_Hackathon.Entities;

public class Appointment
{
    public int Id { get; set; }
    [Required]
    public string DoctorId { get; set; } = default!;
    public ApplicationUser Doctor { get; set; } = default!;
    public string? PatientId { get; set; } 
    public ApplicationUser? Patient { get; set; } 
    [Required]
    public DateTime AppointmentDate { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public bool IsBooked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? BookedAt { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsRated { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
}
