using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTaskManager.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        // Identity User relationship
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        [MaxLength(20)]
        public string? Mobile { get; set; }

        // Foreign Key
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }

        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<TaskRecord> Tasks { get; set; } = new List<TaskRecord>();
      
    }
}