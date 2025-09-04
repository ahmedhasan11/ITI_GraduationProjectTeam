using ITI_Hackathon.ServiceContracts.DTO;

namespace ITI_Hackathon.ServiceContracts
{
    public interface IMedicineService
    {       
        Task<MedicineAddResponseDto> AddMedicineAsync(MedicineAddRequestDto request);
       
        Task<IEnumerable<MedicineListDto>> GetAllMedicineAsync();
      
        Task<MedicineDetailsDto?> GetMedicineByIdAsync(int id);

        Task<IEnumerable<MedicineListDto>> SearchMedicineAsync(string searchTerm);

        Task<MedicineUpdateResponseDto> UpdateMedicineAsync(MedicineUpdateRequestDto request);
       
        Task<string> DeleteMedicineAsync(int id);
    }
}
