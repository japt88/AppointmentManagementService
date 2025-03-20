using AppointmentManagementService.Entities;
using FluentResults;

namespace AppointmentManagementService.Services.Contracts
{
    public interface IPatientService
    {
        Task<Result<Patient>> CreatePatient(Patient patient);
        Task<Patient> GetPatientById(Guid id);
        Task<IEnumerable<Patient>> GetAllPatients();
        Task<Result<Patient>> UpdatePatient(Guid id, Patient patient);
    }
}
