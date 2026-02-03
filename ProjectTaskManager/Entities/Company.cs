using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public class Company : BaseEntity
    {
       
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Location { get; set; }

     
        // Navigation Properties
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}