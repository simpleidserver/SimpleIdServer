// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using System.Net;
using System.Net.Mail;

namespace SimpleIdServer.IdServer.Email
{
    public interface IEmailUserNotificationService : IUserNotificationService { }

    public class EmailUserNotificationService : IEmailUserNotificationService
    {
        private readonly IConfiguration _configuration;

        public EmailUserNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => Constants.AMR;

        public Task Send(string message, User user)
        {
            var emailOptions = GetOptions();
            var email = user.Email;
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailOptions.SmtpEnableSsl;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailOptions.SmtpUserName, emailOptions.SmtpPassword);
                smtpClient.Host = emailOptions.SmtpHost;
                smtpClient.Port = emailOptions.SmtpPort;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailOptions.FromEmail),
                    Subject = emailOptions.Subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);
            }

            return Task.CompletedTask;
        }

        private IdServerEmailOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerEmailOptions).Name);
            return section.Get<IdServerEmailOptions>();
        }
    }
}
