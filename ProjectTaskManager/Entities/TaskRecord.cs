using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTaskManager.Entities
{
    public class TaskRecord : BaseEntity
    {
       

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public DateTime Createdat { get; set; } = DateTime.UtcNow;
        public DateTime? Updatedat { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual Employee? Employee { get; set; }
    }
}