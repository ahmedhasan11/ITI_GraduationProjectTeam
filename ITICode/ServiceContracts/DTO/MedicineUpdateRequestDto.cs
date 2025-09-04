using System.ComponentModel.DataAnnotations;

namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class MedicineUpdateRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
		public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }
}
