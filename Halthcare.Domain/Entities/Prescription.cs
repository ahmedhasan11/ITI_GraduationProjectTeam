namespace Healthcare.Domain.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }= Guid.NewGuid();
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public List<PrescriptionItem> Items { get; set; } = new();
    }

    public class PrescriptionItem
    {
        public Guid Id { get; set; }= Guid.NewGuid();
        public Guid PrescriptionId { get; set; }
        public Guid MedicineId { get; set; }
        public string? Notes { get; set; }
        public int Days { get; set; }
        public int TimesPerDay { get; set; }
        public Prescription Prescription { get; set; } = default!;
        public Medicine Medicine { get; set; } = default!;
    }
}