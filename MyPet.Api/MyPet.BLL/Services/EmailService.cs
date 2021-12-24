using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.EmailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;

        public EmailService(ILogger<EmailService> logger)
        {
            this.logger = logger;
        }

        public async Task<bool> SendConfirmationEmail(EmailConfig emailConfig, string to, string userId, string emailToken)
        {
            string confirmationUrl = $"{emailConfig.EmailConfirmationLink}?userid={userId}&emailtoken={emailToken}";
                
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailConfig.Host, emailConfig.Port, true);

                    if (emailConfig.RequiresAuthentication)
                    {
                        if (client.AuthenticationMechanisms.Contains("NTLM"))
                        {
                            var auth = new SaslMechanismNtlm(emailConfig.UserName, emailConfig.Password);
                            await client.AuthenticateAsync(auth);
                        }
                        else
                        {
                            await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                        }
                    }

                    var message = new MimeMessage
                    {
                        Subject = "Email confirmation message",
                        Body = new BodyBuilder { HtmlBody = $"Please, confirm email following the link: <a href='{confirmationUrl}'>{confirmationUrl}</a>" }.ToMessageBody(),
                    };

                    message.From.Add(new MailboxAddress(Encoding.UTF8, emailConfig.SenderName, emailConfig.SenderEmail));

                    message.To.Add(new MailboxAddress(Encoding.UTF8, string.Empty, to));

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                logger.LogInformation($"email confirmation has been send to user with id '{userId}'. Email address: {to}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception catched while sending confirmation email to user with id '{userId}'. Email adress: {to}. Exception message: {ex.Message}. StackTrace: \r\n {ex.StackTrace}");
                return false;
            }
        }
    }
}
