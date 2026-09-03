using Healthcare.Domain.Entities;
using Healthcare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<DoctorProfile> Doctors => Set<DoctorProfile>();
        public DbSet<PatientProfile> Patients => Set<PatientProfile>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<ChatThread> ChatThreads => Set<ChatThread>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<Consultation> DoctorPayments => Set<Consultation>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Category> Categories => Set<Category>();

		protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
			b.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		}
    }
}
