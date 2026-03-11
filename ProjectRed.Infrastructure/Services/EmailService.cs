
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ProjectRed.Core.Configuration;
using ProjectRed.Core.Interfaces.Services.Email;

namespace ProjectRed.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string smtpServer;
        private readonly int smtpPort = 587;
        private readonly string smtpUsername;
        private readonly string smtpPassword;
        private readonly string fromEmail;
        private readonly string fromName;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            var settings = emailSettings.Value;

            smtpServer = settings.MailServer;
            smtpUsername = settings.SenderEmail;
            smtpPassword = settings.SenderPassword;
            fromEmail = settings.SenderEmail;
            fromName = settings.SenderName;
        }

        public async Task SendEmailAsync(string email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Greetings";

            message.Body = new TextPart("plain")
            {
                Text = $"How are you?"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(smtpUsername, smtpPassword);
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to send email: " + ex.Message);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
