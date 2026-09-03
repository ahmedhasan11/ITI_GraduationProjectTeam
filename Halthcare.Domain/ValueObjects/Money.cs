using Healthcare.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.ValueObjects
{
	public sealed class Money : IEquatable<Money>
	{
		public decimal Amount { get; }

		private Money(decimal amount)
		{
			Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
		}

		public static Money From(decimal amount)
		{
			if (amount < 0)
				throw new ValidationDomainException(nameof(amount), "Amount cannot be negative.");

			return new Money(amount);
		}
		public static Money Zero => new(0m);

		public override bool Equals(object? obj) => Equals(obj as Money);

		public bool Equals(Money? other)
		{
			if (other is null) return false;
			return Amount == other.Amount;
		}

		public override int GetHashCode() => Amount.GetHashCode();

		public override string ToString() => $"${Amount:F2}";
	}
}
