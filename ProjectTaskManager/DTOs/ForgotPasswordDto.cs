using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.DTOs
{
    public class ForgottenPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}