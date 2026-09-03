using System.ComponentModel.DataAnnotations;

namespace Healthcare.Application.DTOs
{
    public class BookAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string PatientId { get; set; }
    }

}
