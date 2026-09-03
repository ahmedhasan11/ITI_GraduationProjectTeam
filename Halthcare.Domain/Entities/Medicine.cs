
using Healthcare.Domain.Exceptions;
using Healthcare.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Domain.Entities
{
    public class Medicine
    {
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public Guid? CategoryId { get; set; }
		public Category? Category { get; set; }
		public bool RequiresPrescription { get; set; }
		public Money Price { get; set; } = Money.Zero;
		public int Stock { get; private set; }
		public string? ImageUrl { get; set; }
		public bool IsAvailable { get; private set; } = true;
		public bool IsDeleted { get; private set; } = false;

		private Medicine() { }

		// Primary constructor for creating new medicines
		public Medicine(string name, string? description, decimal price, int? stock = 0, Guid? categoryId = null, bool requiresPrescription = false, string? imageUrl = null, bool isAvailable = true)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ValidationDomainException(nameof(name), "Medicine name is required.");

			if (stock < 0)
				throw new ValidationDomainException(nameof(stock), "Stock cannot be negative.");

			Name = name.Trim();
			Description = description?.Trim();
			Price = Money.From(price);
			if (stock == null)
			{
				Stock = 0;
			}
			else
			{
				Stock = stock.Value;
			}
			CategoryId = categoryId;
			RequiresPrescription = requiresPrescription;
			ImageUrl = imageUrl;
			IsAvailable = isAvailable;
			IsDeleted = false;
		}
		public bool HasStock(int requestedQuantity) => Stock >= requestedQuantity;
		public void IncreaseStock(int quantity)
		{
			if (IsDeleted)
				throw new BusinessException("Cannot increase stock for a deleted medicine.");

			if (quantity <= 0)
				throw new ValidationDomainException(nameof(quantity), "Quantity to increase must be greater than zero.");

			Stock += quantity;
		}
		public void DecreaseStock(int quantity)
		{
			if (IsDeleted)
				throw new BusinessException("Cannot decrease stock for a deleted medicine.");

			if (quantity <= 0)
				throw new ValidationDomainException(nameof(quantity), "Quantity to decrease must be greater than zero.");
			if (quantity > Stock)
				throw new BusinessException($"Cannot decrease stock below 0. Current stock: {Stock}, requested decrease: {quantity}");

			Stock -= quantity;
		}

		public void MakeAvailable()
		{
			if (IsDeleted)
				throw new BusinessException("Cannot make a deleted medicine available. Restore it first.");

			IsAvailable = true;
		}
		public void MakeUnavailable()
		{
			if (IsDeleted)
				throw new BusinessException("Cannot modify a deleted product.");

			IsAvailable = false;
		}
		public void SoftDelete()
		{
			IsDeleted = true;
			IsAvailable = false;
		}
		public void Restore()
		{
			IsDeleted = false;
			IsAvailable = true;
		}
	}
}
