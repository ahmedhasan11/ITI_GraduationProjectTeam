using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_Hackathon.Entities
{
    public class DoctorProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; } = "General";

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; } = 0.0;

        public int CompletedChats { get; set; } = 0;

        public bool IsApproved { get; set; } = false;

        public ApplicationUser User { get; set; } = default!;
    }

}
