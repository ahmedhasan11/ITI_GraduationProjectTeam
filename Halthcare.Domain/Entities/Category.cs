using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.Entities
{
	public class Category
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		public List<Medicine> Medicines { get; set; } = new List<Medicine>();

	}
}
