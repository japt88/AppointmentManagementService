namespace AppointmentManagementService.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid PatientId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime? CanceledAt { get; set; }
        public Patient PatientInfo { get; set; }
    }
}
