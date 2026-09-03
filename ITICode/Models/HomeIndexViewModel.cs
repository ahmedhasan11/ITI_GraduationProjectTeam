using Healthcare.Application.DTOs;

namespace Healthcare.Presentation.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<MedicineListDto> Medicines { get; set; } = new List<MedicineListDto>();
        public IEnumerable<DoctorApprovedDTO> Doctors { get; set; } = new List<DoctorApprovedDTO>();

    }
}
