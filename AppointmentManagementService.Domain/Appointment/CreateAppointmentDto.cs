using AppointmentManagementService.Shared.Enums;

namespace AppointmentManagementService.Domain.Appointment
{
    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
