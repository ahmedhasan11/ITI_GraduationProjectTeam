namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = default!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }

}
