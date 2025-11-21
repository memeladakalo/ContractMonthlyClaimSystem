using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ContractMonthlyClaimSystem.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {


            //Builds and sends message correctly with MailKit
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_config["EmailSettings:From"]);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            //Connects securely to SMTP
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpServer"],
                int.Parse(_config["EmailSettings:Port"]),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _config["EmailSettings:Username"],
                _config["EmailSettings:Password"]
            );

            //CONNECTION CLOSES AFTER SENDING THE EMAIL
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);


        }

       }
}

