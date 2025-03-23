using AppointmentManagementService.Entities;

namespace AppointmentManagementService.Data.Repositories
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<Patient?> GetByEmailAsync(string email);
    }
}
