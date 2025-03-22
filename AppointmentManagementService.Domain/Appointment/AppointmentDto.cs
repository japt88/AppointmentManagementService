using AppointmentManagementService.Shared.Enums;

namespace AppointmentManagementService.Domain.Appointment
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime? CanceledAt { get; set; }
    }
}
