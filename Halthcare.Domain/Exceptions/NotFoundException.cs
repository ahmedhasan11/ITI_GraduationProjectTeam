using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.Exceptions
{
	public class NotFoundException : DomainException
	{
		public NotFoundException(string message) : base(message) { }

		public NotFoundException(string name, object key)
			: base($"Entity \"{name}\" ({key}) was not found.") { }
	}
}
