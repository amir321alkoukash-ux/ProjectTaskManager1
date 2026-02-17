using ProjectTaskManager.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public class Company : BaseEntity
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;


        public ICollection<User> Users { get; set; }
        public ICollection<Project> Projects { get; set; }



        [MaxLength(500)]
        public string? Location { get; set; }
        public Company()
        {
            Users = new HashSet<User>();
            Projects = new HashSet<Project>();
        }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}