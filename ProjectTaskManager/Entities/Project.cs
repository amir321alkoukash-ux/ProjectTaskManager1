using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public class Project : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        // Foreign key
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        // Navigation properties
        public ICollection<TaskRecord> Tasks { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}