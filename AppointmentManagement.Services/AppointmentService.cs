using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services.Contracts;
using FluentResults;

namespace AppointmentManagementService.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentUnitOfWork _unitOfWork;

        public AppointmentService(IAppointmentUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> CancelAppointment(Guid appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null || (appointment.AppointmentDate - DateTime.UtcNow).TotalHours < 24)
                return Result.Fail("Appointments must be canceled at least 24 hours in advance.");

            appointment.IsCanceled = true;
            appointment.CanceledAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.CompleteAsync();
            return Result.Fail("The appointment has been canceled.");
        }

        public async Task<Result<Appointment>> ScheduleAppointment(Appointment appointment)
        {
            var existingAppointments = await _unitOfWork.Appointments.GetAllAsync();
            if (existingAppointments.Any(a => a.PatientId == appointment.PatientId &&
                                              a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                                              !a.IsCanceled))
            {
                return Result.Fail("A patient cannot book more than one appointment on the same day.");
            }

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.CompleteAsync();
            return Result.Ok(appointment);
        }
    }
}
