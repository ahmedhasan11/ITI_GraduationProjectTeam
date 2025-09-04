using System.ComponentModel.DataAnnotations;

namespace ITI_Hackathon.Models.Account
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "Full name"), MaxLength(100)]
        public string FullName { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = default!;

        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        [Display(Name = "Register as")]
        public string Role { get; set; } = "Patient";
    }

}
