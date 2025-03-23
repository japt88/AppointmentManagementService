using AppointmentManagementService.Data.Repositories;

namespace AppointmentManagementService.Data.UnitOfWork
{
    public class AppointmentUnitOfWork : IAppointmentUnitOfWork
    {
        private readonly AppointmentDbContext _context;

        public IAppointmentRepository Appointments { get; }

        public IPatientRepository Patients { get; }


        public AppointmentUnitOfWork(AppointmentDbContext context)
        {
            _context = context;
            Appointments = new AppointmentRepository(context);
            Patients = new PatientRepository(context);
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
