namespace Healthcare.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
		public Guid? UserId { get; set; }  
        public string? SessionId { get; set; }
        public Guid MedicineId { get; set; }
        public int Quantity { get; set; }
        public Medicine Medicine { get; set; } = default!;
    }
}
