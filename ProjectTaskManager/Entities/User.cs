using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public class User : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string Mobile { get; set; }

        // Foreign key
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        // Navigation properties
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public ICollection<TaskRecord> Tasks { get; set; }
    }
}