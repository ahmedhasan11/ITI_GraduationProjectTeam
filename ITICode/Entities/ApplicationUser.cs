using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ITI_Hackathon.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }

        public DoctorProfile? DoctorProfile { get; set; }
        public PatientProfile? PatientProfile { get; set; }
    }

}