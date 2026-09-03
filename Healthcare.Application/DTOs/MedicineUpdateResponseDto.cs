namespace Healthcare.Application.DTOs
{
    public class MedicineUpdateResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public MedicineDetailsDto? UpdatedMedicine { get; set; }
    }
}
