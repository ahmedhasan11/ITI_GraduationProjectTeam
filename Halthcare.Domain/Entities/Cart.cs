using Healthcare.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.Entities
{
	public class Cart
	{
		public Guid Id { get; private set; } = Guid.NewGuid();
		public Guid? UserId { get; private set; }
		public string? SessionId { get; private set; }

		private readonly List<CartItem> _items = new();
		public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

		private Cart() { }

		public Cart(Guid? userId, string? sessionId = null)
		{
			if (userId == null && string.IsNullOrWhiteSpace(sessionId))
			{
				throw new ValidationException("Cart must be associated with either a UserId or a SessionId.");
			}

			UserId = userId;
			SessionId = sessionId;
		}
		public void AddItem(Guid medicineId, int quantity)
		{
			if (medicineId == Guid.Empty)
			{
				throw new ValidationDomainException(nameof(medicineId), "MedicineId cannot be empty.");
			}

			if (quantity <= 0)
			{
				throw new ValidationDomainException(nameof(quantity), "Quantity must be greater than zero.");
			}

			var existingItem = _items.FirstOrDefault(i => i.MedicineId == medicineId);
			if (existingItem != null)
			{
				existingItem.IncreaseQuantity(quantity);
			}
			else
			{
				_items.Add(new CartItem(medicineId, quantity));
			}
		}
		public void RemoveItem(Guid medicineId)
		{
			if (medicineId == Guid.Empty)
			{
				throw new ValidationDomainException(nameof(medicineId), "MedicineId cannot be empty.");
			}

			var item = _items.FirstOrDefault(i => i.MedicineId == medicineId);
			if (item == null)
			{
				throw new NotFoundException(nameof(CartItem), medicineId);
			}

			_items.Remove(item);
		}
		public void UpdateQuantity(Guid medicineId, int newQuantity)
		{
			if (medicineId == Guid.Empty)
			{
				throw new ValidationDomainException(nameof(medicineId), "MedicineId cannot be empty.");
			}

			if (newQuantity < 0)
			{
				throw new ValidationDomainException(nameof(newQuantity), "Quantity cannot be negative.");
			}

			var item = _items.FirstOrDefault(i => i.MedicineId == medicineId);
			if (item == null)
			{
				throw new NotFoundException(nameof(CartItem), medicineId);
			}

			if (newQuantity == 0)
			{
				_items.Remove(item);
			}
			else
			{
				item.SetQuantity(newQuantity);
			}
		}
		public void ClearCart()
		{
			_items.Clear();
		}
	}
}
