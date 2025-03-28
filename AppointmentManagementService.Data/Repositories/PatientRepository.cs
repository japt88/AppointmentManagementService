﻿using AppointmentManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementService.Data.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        private readonly AppointmentDbContext _context;
        public PatientRepository(AppointmentDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Patient?> GetByIdIncludingAppointmentsAsync(Guid id)
        {
            return await _context.Patients.Include(x => x.Appointments).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
