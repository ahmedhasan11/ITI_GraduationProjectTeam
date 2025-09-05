using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_Hackathon.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }  // can be null for guests

        public string? SessionId { get; set; }  // new column for guest carts

        [Required]
        [ForeignKey(nameof(Medicine))]
        public int MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public ApplicationUser? User { get; set; }
        public Medicine Medicine { get; set; } = default!;
    }

}
