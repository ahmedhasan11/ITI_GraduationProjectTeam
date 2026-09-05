using Healthcare.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.ValueObjects
{
	public class ShippingAddress
	{
		public string RecipientName { get; private set; } = default!;
		public string PhoneNumber { get; private set; } = default!;
		public string City { get; private set; } = default!;
		public string Street { get; private set; } = default!;
		public string? BuildingNumber { get; private set; }
		public string? PostalCode { get; private set; }

		// Parameterless constructor for EF Core Owned Type
		private ShippingAddress() { }

		public ShippingAddress(
			string recipientName,
			string phoneNumber,
			string city,
			string street,
			string? buildingNumber = null,
			string? postalCode = null)
		{
			if (string.IsNullOrWhiteSpace(recipientName))
				throw new ValidationDomainException(nameof(recipientName), "Recipient name is required.");

			if (string.IsNullOrWhiteSpace(phoneNumber))
				throw new ValidationDomainException(nameof(phoneNumber), "Phone number is required.");

			if (string.IsNullOrWhiteSpace(city))
				throw new ValidationDomainException(nameof(city), "City is required.");

			if (string.IsNullOrWhiteSpace(street))
				throw new ValidationDomainException(nameof(street), "Street address is required.");

			RecipientName = recipientName.Trim();
			PhoneNumber = phoneNumber.Trim();
			City = city.Trim();
			Street = street.Trim();
			BuildingNumber = buildingNumber?.Trim();
			PostalCode = postalCode?.Trim();
		}
	}
}
