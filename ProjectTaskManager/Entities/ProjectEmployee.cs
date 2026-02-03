using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTaskManager.Entities
{
    public class ProjectEmployee : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
    }
}