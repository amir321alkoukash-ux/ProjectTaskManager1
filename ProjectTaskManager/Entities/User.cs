using Microsoft.AspNetCore.Identity;

namespace ProjectTaskManager.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual Company Company { get; set; }
        public  ICollection<Employee>? Employees { get; set; }
        public User()
        {
            Employees = new HashSet<Employee>();
        }

        // Navigation properties
       
    }
}