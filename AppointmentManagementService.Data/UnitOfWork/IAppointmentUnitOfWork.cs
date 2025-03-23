using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Entities;

namespace AppointmentManagementService.Data.UnitOfWork
{
    public interface IAppointmentUnitOfWork : IDisposable
    {
        IAppointmentRepository Appointments { get; }
        IPatientRepository Patients { get; }
        Task<int> CompleteAsync();
    }
}
