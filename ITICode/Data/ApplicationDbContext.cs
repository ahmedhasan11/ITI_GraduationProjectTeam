
using ITI_Hackathon.Entities;
using ITI_Hackathon.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Doctor ↔ User
            b.Entity<DoctorProfile>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<DoctorProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient ↔ User
            b.Entity<PatientProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PatientProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prescription ↔ Doctor
            b.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prescription ↔ Patient
            b.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem ↔ Medicine
            b.Entity<CartItem>()
                .HasOne(ci => ci.Medicine)
                .WithMany()
                .HasForeignKey(ci => ci.MedicineId);

            // OrderItem ↔ Order
            b.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem ↔ Medicine
            b.Entity<OrderItem>()
                .HasOne(oi => oi.Medicine)
                .WithMany()
                .HasForeignKey(oi => oi.MedicineId);

            // ChatThread ↔ Patient
            b.Entity<ChatThread>()
                .HasOne(ct => ct.Patient)
                .WithMany()
                .HasForeignKey(ct => ct.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatThread ↔ Doctor
            b.Entity<ChatThread>()
                .HasOne(ct => ct.Doctor)
                .WithMany()
                .HasForeignKey(ct => ct.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatMessage ↔ Thread
            b.Entity<ChatMessage>()
                .HasOne(m => m.Thread)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            // ChatMessage ↔ Sender (User)
            b.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionItem ↔ Prescription
            b.Entity<PrescriptionItem>()
                .HasOne(pi => pi.Prescription)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PrescriptionId);

            // PrescriptionItem ↔ Medicine
            b.Entity<PrescriptionItem>()
                .HasOne(pi => pi.Medicine)
                .WithMany()
                .HasForeignKey(pi => pi.MedicineId);

            // Medicine Price precision
            b.Entity<Medicine>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");

            // Index for ChatMessage queries (Thread + SentAt)
            b.Entity<ChatMessage>()
                .HasIndex(m => new { m.ThreadId, m.SentAt });
        }
    }
}
