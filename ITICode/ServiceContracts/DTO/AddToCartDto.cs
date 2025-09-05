namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class AddToCartDto
    {
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
    }

}
