using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManagementService.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
