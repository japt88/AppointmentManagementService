using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Data;
using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Entities;

namespace AppointmentManagementService.Data.UnitOfWork
{
    public class AppointmentUnitOfWork : IAppointmentUnitOfWork
    {
        private readonly AppointmentDbContext _context;

        public IAppointmentRepository Appointments { get; }

        public IGenericRepository<Patient> Patients { get; }


        public AppointmentUnitOfWork(AppointmentDbContext context)
        {
            _context = context;
            Appointments = new AppointmentRepository(context);
            Patients = new GenericRepository<Patient>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
