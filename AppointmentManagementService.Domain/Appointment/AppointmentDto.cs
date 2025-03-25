using AppointmentManagementService.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppointmentManagementService.Domain.Appointment
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }

        [JsonConverter(typeof(StringEnumConverter), true)]
        public AppointmentType AppointmentType { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime? CanceledAt { get; set; }
    }
}
