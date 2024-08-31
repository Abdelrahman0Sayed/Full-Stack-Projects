using System.Collections.Immutable;
using System.Net;
using api.Dtos.Account;
using api.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Net.Mail;
using NETCore.MailKit;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Reflection;
using Azure.Identity;
using MailKit.Net.Smtp;
using MailKit.Security;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.Extensions.Options;

namespace api.Services
{
    public class EmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MailSettings _mailSettings;

        public EmailService( UserManager<ApplicationUser> userManager ,IOptions<MailSettings> mailSettings)
        {
            _userManager = userManager;
            _mailSettings = mailSettings.Value;
        }


        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments)
        {
            // Prepare the email
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject
            };

            email.To.Add(MailboxAddress.Parse(mailTo));

            // Prepare the attachments of email
            var builder = new BodyBuilder();
            if (attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            // Prepare the body
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));
            
            // Prepare the email provider
            using var smtp = new SmtpClient();
            
            // Provider Settings
            smtp.Connect(_mailSettings.Host , _mailSettings.Port , SecureSocketOptions.StartTls);
            // Add Authentications
            smtp.Authenticate(_mailSettings.Email , _mailSettings.Password);

            await smtp.SendAsync(email);
            
            smtp.Disconnect(true);
        }
    }
}
