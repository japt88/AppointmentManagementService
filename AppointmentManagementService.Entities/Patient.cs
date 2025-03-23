using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManagementService.Entities
{
    public class Patient : BaseEntity
    {
        [MaxLength(60)]
        public string FirstName { get; set; }
        [MaxLength(60)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
