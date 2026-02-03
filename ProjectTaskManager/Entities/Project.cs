using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTaskManager.Entities
{
    public class Project : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<TaskRecord> Tasks { get; set; } = new List<TaskRecord>();
        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    }
}