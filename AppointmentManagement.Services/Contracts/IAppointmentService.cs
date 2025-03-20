using AppointmentManagementService.Entities;
using FluentResults;

namespace AppointmentManagementService.Services.Contracts
{
    public interface IAppointmentService
    {
        Task<Result<Appointment>> ScheduleAppointment(Appointment appointment);
        Task<Result<string>> CancelAppointment(Guid appointmentId);
    }
}
