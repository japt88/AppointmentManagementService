﻿using AppointmentManagementService.Entities;

namespace AppointmentManagementService.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAllPatientAppointmentsAsync(Guid patientId);
        Task<IEnumerable<Appointment>> GetAllActivePatientAppointmentsAsync(Guid patientId);
    }
}
