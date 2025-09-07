using System.ComponentModel.DataAnnotations;

namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class BookAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string PatientId { get; set; }
    }

}
