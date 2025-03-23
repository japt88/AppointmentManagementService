using AppointmentManagementService.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementService.Domain.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public Guid PatientId { get; set; }
        [Required]
        public AppointmentType AppointmentType { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
    }
}
