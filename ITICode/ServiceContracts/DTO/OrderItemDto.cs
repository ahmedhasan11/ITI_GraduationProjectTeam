namespace ITI_Hackathon.ServiceContracts.DTO
{
	public class OrderItemDto
	{
		//representeseach item inside the OrderDro

		public int MedicineID { get; set; }

		public string MedicineName { get; set; }

		public int Quantity { get; set; }

		public decimal UnitPrice { get; set; }

		public decimal TotalPrice => Quantity * UnitPrice;

	}
}
