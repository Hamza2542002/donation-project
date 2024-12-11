using donation_project.models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace donation_project.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSetting> _emailSetting;

        public EmailService(IOptions<EmailSetting> emailSetting)
        {
            _emailSetting = emailSetting;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient
            {
                EnableSsl = _emailSetting.Value.EnableSsl,
                Host = _emailSetting.Value.Host,
                Port = _emailSetting.Value.Port,
                Credentials = new NetworkCredential(_emailSetting.Value.Email , _emailSetting.Value.Password)
            };
            var emailMessage = new MailMessage(_emailSetting.Value.Email!, to , subject , body);

            await smtpClient.SendMailAsync(emailMessage);
        }
    }
}
