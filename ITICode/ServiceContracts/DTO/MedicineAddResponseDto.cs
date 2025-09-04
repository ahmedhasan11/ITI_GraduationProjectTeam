namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class MedicineAddResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string Message { get; set; }
    }
}
