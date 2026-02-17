using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}