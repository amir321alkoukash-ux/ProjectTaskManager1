using Microsoft.Extensions.Configuration;
using ProjectTaskManager.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProjectTaskManager.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            using var client = new SmtpClient(smtpServer, smtpPort);
            using var message = new MailMessage(fromEmail, to, subject, body);
            // Configure credentials, SSL, etc. as needed
            await client.SendMailAsync(message);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            string subject = "Password Reset Request";
            string body = $"Click the link to reset your password: {resetLink}";
            await SendEmailAsync(email, subject, body);
        }
    }
}