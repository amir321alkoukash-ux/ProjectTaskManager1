using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public class Company : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; }

        // Navigation properties
        public ICollection<Project> Projects { get; set; }
        public ICollection<User> Users { get; set; }
    }
}