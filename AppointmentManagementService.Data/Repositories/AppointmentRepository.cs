using AppointmentManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementService.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {

        private readonly AppointmentDbContext _context;

        public AppointmentRepository(AppointmentDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllPatientAppointmentsAsync(Guid patientId)
        {
            return await _context.Appointments.Where(a => a.PatientId == patientId).ToListAsync();
        }
    }
}
