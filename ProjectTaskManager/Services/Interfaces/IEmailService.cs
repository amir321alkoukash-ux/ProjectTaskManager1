using System.Threading.Tasks;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName);
        Task SendPasswordResetEmailAsync(string to, string resetLink);
    }
}