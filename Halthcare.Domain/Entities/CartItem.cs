using Healthcare.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Healthcare.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
		public Guid CartId { get; private set; }
		public Cart Cart { get; private set; } = default!;
		public Guid MedicineId { get; private set; }
        public int Quantity { get; private set; }
        public Medicine Medicine { get; private set; } = default!;

		public CartItem(Guid medicineId, int quantity)
		{
			if (medicineId == Guid.Empty)
				throw new ValidationDomainException(nameof(medicineId), "MedicineId cannot be empty.");

			if (quantity <= 0)
				throw new ValidationDomainException(nameof(quantity), "Quantity must be greater than zero.");

			MedicineId = medicineId;
			Quantity = quantity;
		}

		public void IncreaseQuantity(int amount)
		{
			if (amount <= 0)
				throw new ValidationDomainException(nameof(amount), "Amount to increase must be greater than zero.");

			Quantity += amount;
		}

		public void DecreaseQuantity(int amount)
		{
			if (amount <= 0)
				throw new ValidationDomainException(nameof(amount), "Amount to decrease must be greater than zero.");

			if (amount >= Quantity)
				throw new ValidationDomainException(nameof(amount), "Decrease amount cannot be equal to or greater than current quantity. Use RemoveItem instead.");

			Quantity -= amount;
		}

		public void SetQuantity(int newQuantity)
		{
			if (newQuantity <= 0)
				throw new ValidationDomainException(nameof(newQuantity), "Quantity must be greater than zero.");

			Quantity = newQuantity;
		}
	}
}
