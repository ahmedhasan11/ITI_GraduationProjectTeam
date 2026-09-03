using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.Exceptions
{
	public class ValidationDomainException : DomainException
	{
		public IDictionary<string, string[]> Errors { get; }

		public ValidationDomainException(string message) : base(message)
		{
			Errors = new Dictionary<string, string[]>();
		}

		public ValidationDomainException(string propertyName, string errorMessage)
			: base(errorMessage)
		{
			Errors = new Dictionary<string, string[]>
			{
				{ propertyName, new[] { errorMessage } }
			};
		}

		public ValidationDomainException(IDictionary<string, string[]> errors)
			: base("One or more validation failures have occurred.")
		{
			Errors = errors;
		}
	}
}
