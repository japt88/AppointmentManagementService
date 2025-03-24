using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Domain.Appointment;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services.Contracts;
using AutoMapper;
using FluentResults;

namespace AppointmentManagementService.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<Result<AppointmentDto>> ScheduleAppointment(CreateAppointmentDto appointmentDto)
        {
            var patientData= await _unitOfWork.Patients.GetByIdAsync(appointmentDto.PatientId);
            if (patientData == null)
            {
                return Result.Fail("The patient does not exist.");
            }

            var existingAppointments = await _unitOfWork.Appointments.GetAllAsync();
            if (existingAppointments.Any(a => a.PatientId == appointmentDto.PatientId &&
                                              a.AppointmentDate.Date == appointmentDto.AppointmentDate.Date &&
                                              !a.IsCanceled))
            {
                return Result.Fail("A patient cannot book more than one appointment on the same day.");
            }

            var appointment = _mapper.Map<Appointment>(appointmentDto);
            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.CompleteAsync();
            return Result.Ok(_mapper.Map<AppointmentDto>(appointment));
        }

        public async Task<IEnumerable<AppointmentDto>> GetPatientAppointments(Guid patientId)
        {
            var appointments = await _unitOfWork.Appointments.GetAllPatientAppointmentsAsync(patientId);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }
}
