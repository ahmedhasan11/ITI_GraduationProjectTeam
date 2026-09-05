using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcare.Domain.ValueObjects
{
	public class CreateOrderItemData
	{
		public Guid MedicineId { get; set; }

		public string MedicineName { get; set; }

		public int Quantity { get; set; }

		public decimal UnitPrice { get; set; }
	}
}
