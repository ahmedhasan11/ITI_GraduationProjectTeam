using System.ComponentModel.DataAnnotations;
using Healthcare.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Healthcare.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }
    }

}