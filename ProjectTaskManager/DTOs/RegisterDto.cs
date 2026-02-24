using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Mobile { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string Role { get; set; } // "Admin" or "Employee"
    }
}