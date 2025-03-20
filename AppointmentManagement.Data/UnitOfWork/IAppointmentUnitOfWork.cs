using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Entities;

namespace AppointmentManagementService.Data.UnitOfWork
{
    public interface IAppointmentUnitOfWork : IDisposable
    {
        IGenericRepository<Appointment> Appointments { get; }
        IGenericRepository<Patient> Patients { get; }
        Task<int> CompleteAsync();
    }
}
