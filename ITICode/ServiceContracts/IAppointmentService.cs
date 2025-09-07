using ITI_Hackathon.ServiceContracts.DTO;

namespace ITI_Hackathon.ServiceContracts
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(string doctorId);
        Task<List<AppointmentDto>> GetAvailableAppointmentsAsync(string doctorId = null);
        Task<List<AppointmentDto>> GetPatientAppointmentsAsync(string patientId);
        Task<AppointmentDto> AddAppointmentAsync(AppointmentDto dto);
        Task<bool> BookAppointmentAsync(int appointmentId, string patientId);
        Task<bool> CompleteAppointmentAsync(int appointmentId, string doctorId);
        Task<bool> DeleteAppointmentAsync(int appointmentId, string doctorId);

        //Task<bool> RateAppointmentAsync(int appointmentId, int rating, string comment, string patientId);
        //Task<List<RatingDto>> GetDoctorRatingsAsync(string doctorId);
        //Task<double> GetDoctorAverageRatingAsync(string doctorId);
    }

}
