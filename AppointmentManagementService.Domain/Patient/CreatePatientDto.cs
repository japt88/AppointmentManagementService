using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementService.Domain.Patient
{
    public class CreatePatientDto
    {
        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(60)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
    }
}
