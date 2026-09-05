
using Healthcare.Domain.Enums;
using Healthcare.Domain.Exceptions;
using Healthcare.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Healthcare.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }= Guid.NewGuid();
        public Guid PatientId { get; private set; }
        public OrderStatusEnum Status { get; private set; }

		public bool RequiresRefund { get; private set; }
		public ShippingAddress Address { get; private set; }

		public decimal SubTotal { get; private set; }
		public decimal ShippingFee { get; private set; }
		public decimal TotalAmount { get; private set; }


		private readonly List<OrderItem> _items = new();
		public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
		private Order() { }
        
        public static Order Create(Guid patientId, ShippingAddress address, IEnumerable<CreateOrderItemData> items, decimal shippingFee = 0)
        {
			if (patientId == Guid.Empty)
				throw new ValidationDomainException(nameof(patientId), "PatientId cannot be empty.");

			if (address == null)
				throw new ValidationDomainException(nameof(address), "Shipping address is required.");

			if (items == null || !items.Any())
				throw new ValidationDomainException(nameof(items), "Order must contain at least one item.");

			if (shippingFee < 0)
				throw new ValidationDomainException(nameof(shippingFee), "Shipping fee cannot be negative.");

			var order = new Order
			{
				PatientId = patientId,
				Address = address,
				ShippingFee = shippingFee,
				Status = OrderStatusEnum.Pending,
				RequiresRefund = false
			};

			foreach (var item in items)
			{
				OrderItem orderItem = new OrderItem(item.MedicineId, item.Quantity, item.UnitPrice, item.MedicineName);
				order._items.Add(orderItem);
			}
			order.SubTotal = order._items.Sum(i => i.SubTotal);
			order.TotalAmount = order.SubTotal + order.ShippingFee;
			return order;
		}


		public void MarkAsPaid()
		{
			if (Status != OrderStatusEnum.Pending && Status != OrderStatusEnum.PaymentFailed)
			{
				throw new BusinessException($"Cannot transition order status from '{Status}' to 'Paid'. Only Pending or PaymentFailed orders can be marked as Paid.");
			}

			Status = OrderStatusEnum.Paid;
		}

		public void MarkAsPaymentFailed()
		{
			if (Status != OrderStatusEnum.Pending)
			{
				throw new BusinessException($"Cannot mark order as PaymentFailed when current status is '{Status}'.");
			}

			Status = OrderStatusEnum.PaymentFailed;
		}

		public void MarkAsDelivered()
		{
			if (Status != OrderStatusEnum.Paid)
			{
				throw new BusinessException("Only paid orders can be marked as Delivered.");
			}

			Status = OrderStatusEnum.Delivered;
		}

		public void CancelOrder()
		{
			if (Status == OrderStatusEnum.Cancelled)
			{
				throw new BusinessException("Order is already cancelled.");
			}

			if (Status == OrderStatusEnum.Delivered)
			{
				throw new BusinessException("Cannot cancel an order after it has been delivered. Please use the Return option instead.");
			}

			if (Status == OrderStatusEnum.Returned)
			{
				throw new BusinessException("Cannot cancel an order that has already been returned.");
			}
			if (Status == OrderStatusEnum.Paid)
			{
				RequiresRefund = true;
			}

			Status = OrderStatusEnum.Cancelled;
		}

		public void ReturnOrder()
		{
			if (Status != OrderStatusEnum.Delivered)
			{
				throw new BusinessException("Only delivered orders can be returned.");
			}

			if (Status == OrderStatusEnum.Returned)
			{
				throw new BusinessException("Order has already been returned.");
			}
			Status = OrderStatusEnum.Returned;
			RequiresRefund = true;
		}

		public void UpdateShippingAddress(ShippingAddress newAddress)
		{
			if (newAddress == null)
				throw new ValidationDomainException(nameof(newAddress), "New shipping address cannot be null.");

			if (Status != OrderStatusEnum.Pending)
			{
				throw new BusinessException($"Cannot update shipping address when order status is '{Status}'. Address can only be updated while the order is Pending.");
			}

			Address = newAddress;
		}
	}

    public class OrderItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid OrderId { get; private set; }
        public Guid MedicineId { get; private set; }
		public string MedicineName { get; private set; }
		public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public Medicine Medicine { get; private set; }
        public Order Order { get; private set; }

		public decimal SubTotal { get; private set; }

		private OrderItem() { }

		public OrderItem(Guid medicineId, int quantity, decimal unitPrice, string medicineName)
		{
			if (medicineId == Guid.Empty)
				throw new ValidationDomainException(nameof(medicineId), "MedicineId cannot be empty.");

			if (string.IsNullOrWhiteSpace(medicineName))
				throw new ValidationDomainException(nameof(medicineName), "MedicineName is required.");

			if (quantity <= 0)
				throw new ValidationDomainException(nameof(quantity), "Quantity must be greater than zero.");

			if (unitPrice <= 0)
				throw new ValidationDomainException(nameof(unitPrice), "UnitPrice cannot be zero or negative.");

			MedicineId = medicineId;
			MedicineName= medicineName.Trim();
			Quantity = quantity;
			UnitPrice = unitPrice;
			SubTotal = quantity * unitPrice;
		}
	}
}
