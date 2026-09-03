
namespace Healthcare.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }= Guid.NewGuid();
        public Guid PatientId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public decimal Total { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid MedicineId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Medicine Medicine { get; set; } = default!;
        public Order Order { get; set; } = default!;
    }
}
