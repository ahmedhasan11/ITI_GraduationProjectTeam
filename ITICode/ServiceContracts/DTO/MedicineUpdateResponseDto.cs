namespace ITI_Hackathon.ServiceContracts.DTO
{
    public class MedicineUpdateResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public MedicineDetailsDto? UpdatedMedicine { get; set; }
    }
}
