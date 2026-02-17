using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ProjectTaskManager.Services.Interfaces;

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
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            using var client = new SmtpClient
            {
                Host = smtpSettings["Host"],
                Port = int.Parse(smtpSettings["Port"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"]),
                Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "Reset Your Password - ProjectTaskManager";

            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #333;'>Password Reset Request</h2>
                <p>We received a request to reset your password for your ProjectTaskManager account.</p>
                <p>Click the button below to reset your password. This link will expire in 24 hours.</p>
                <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Reset Password</a>
                <p>If you didn't request this, you can safely ignore this email.</p>
                <p>Best regards,<br/>ProjectTaskManager Team</p>
            </body>
            </html>";

            await SendEmailAsync(to, subject, body);
        }
    }
}