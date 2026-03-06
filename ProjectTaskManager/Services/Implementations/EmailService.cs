using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string? _username;
        private readonly string? _password;

        public EmailService(IConfiguration configuration)
        {
            // Read from appsettings.json
            _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _fromEmail = configuration["EmailSettings:FromEmail"] ?? "noreply@projecttaskmanager.com";
            _username = configuration["EmailSettings:Username"]; // Optional
            _password = configuration["EmailSettings:Password"]; // Optional
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var message = new MailMessage(_fromEmail, to, subject, body);
            using var client = new SmtpClient(_smtpServer, _smtpPort);

            client.EnableSsl = true;

            // Add credentials if provided
            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                client.Credentials = new NetworkCredential(_username, _password);
            }

            await client.SendMailAsync(message);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName)
        {
            using var message = new MailMessage(_fromEmail, to, subject, body);
            using var client = new SmtpClient(_smtpServer, _smtpPort);

            // Add attachment
            using var ms = new MemoryStream(attachment);
            message.Attachments.Add(new Attachment(ms, attachmentName));

            client.EnableSsl = true;

            // Add credentials if provided
            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                client.Credentials = new NetworkCredential(_username, _password);
            }

            await client.SendMailAsync(message);
        }

        public Task SendPasswordResetEmailAsync(string to, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}