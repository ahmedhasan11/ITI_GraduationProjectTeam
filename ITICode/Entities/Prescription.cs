using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_Hackathon.Entities
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Doctor))]
        public string DoctorId { get; set; } = default!;

        [Required]
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser Doctor { get; set; } = default!;
        public ApplicationUser Patient { get; set; } = default!;
        public List<PrescriptionItem> Items { get; set; } = new();
    }

    public class PrescriptionItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Prescription))]
        public int PrescriptionId { get; set; }

        [Required]
        [ForeignKey(nameof(Medicine))]
        public int MedicineId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Range(1, 365)]
        public int Days { get; set; }

        [Range(1, 10)]
        [Display(Name = "Times Per Day")]
        public int TimesPerDay { get; set; }

        public Prescription Prescription { get; set; } = default!;
        public Medicine Medicine { get; set; } = default!;
    }
}