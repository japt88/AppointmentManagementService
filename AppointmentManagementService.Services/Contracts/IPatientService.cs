using AppointmentManagementService.Domain.Patient;
using FluentResults;

namespace AppointmentManagementService.Services.Contracts
{
    public interface IPatientService
    {
        Task<Result<PatientDto>> CreatePatient(CreatePatientDto patientDto);
        Task<PatientDto> GetPatientById(Guid id);
        Task<IEnumerable<PatientDto>> GetAllPatients();
        Task<Result<PatientDto>> UpdatePatient(Guid id, CreatePatientDto patientDto);
  
    }
}
