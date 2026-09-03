

namespace Healthcare.Domain.Entities
{
    public class DoctorProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Specialty { get; set; } = "General";
        public string? Bio { get; set; }
        public string? LicenseNumber { get; set; }

        public double Rating { get; set; } = 0.0;
        public int CompletedChats { get; set; } = 0;
        public bool IsApproved { get; set; } = false;
    }

}
