using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Models.Account
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}