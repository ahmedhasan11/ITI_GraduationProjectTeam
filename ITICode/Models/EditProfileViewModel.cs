using System.ComponentModel.DataAnnotations;

namespace ITI_Hackathon.Models.Account
{
    public class EditProfileViewModel
    {
        public string UserId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;

        // Patient fields
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        // Doctor fields
        public string? Specialty { get; set; }
        public string? Bio { get; set; }
        public string? LicenseNumber { get; set; }
    }
}
