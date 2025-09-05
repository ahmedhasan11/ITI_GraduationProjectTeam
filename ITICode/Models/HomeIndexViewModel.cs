using ITI_Hackathon.ServiceContracts.DTO;

namespace ITI_Hackathon.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<MedicineListDto> Medicines { get; set; } = new List<MedicineListDto>();
        public IEnumerable<DoctorApprovedDTO> Doctors { get; set; } = new List<DoctorApprovedDTO>();

    }
}
