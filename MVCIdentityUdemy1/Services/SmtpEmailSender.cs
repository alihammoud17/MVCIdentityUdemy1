using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace MVCIdentityUdemy1.Services
{
    public class SmtpEmailSender(IOptions<MailSettings> options) : IEmailSender
    {
        public async Task SendEmailAsync(string toAddress, string subject, string message)
        {
            var mailMessage = new MailMessage() 
            { 
                From = new MailAddress(options.Value.SenderEmail),
                Subject = subject,
                Body = message
            };

            mailMessage.To.Add(toAddress);

            using (var client = new SmtpClient(options.Value.SmtpServer, options.Value.Port)
            {
                Credentials = new NetworkCredential(options.Value.Username, options.Value.Password)
            })
            {
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
