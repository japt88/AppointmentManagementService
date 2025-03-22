using AppointmentManagementService.Domain.Appointment;
using FluentResults;

namespace AppointmentManagementService.Services.Contracts
{
    public interface IAppointmentService
    {
        Task<Result<AppointmentDto>> ScheduleAppointment(CreateAppointmentDto appointmentDto);
        Task<Result<string>> CancelAppointment(Guid appointmentId);
        Task<IEnumerable<AppointmentDto>> GetPatientAppointments(Guid patientId);
    }
}
