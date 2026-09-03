
namespace Healthcare.Domain.Entities
{
    public class Medicine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool RequiresPrescription { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }
}
