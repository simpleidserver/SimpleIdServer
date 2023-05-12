// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using System.Net;
using System.Net.Mail;

namespace SimpleIdServer.IdServer.Email
{
    public interface IEmailUserNotificationService : IUserNotificationService { }

    public class EmailUserNotificationService : IEmailUserNotificationService
    {
        private readonly IdServerEmailOptions _emailOptions;

        public EmailUserNotificationService(IOptions<IdServerEmailOptions> options)
        {
            _emailOptions = options.Value;
        }

        public string Name => Constants.AMR;

        public Task Send(string message, User user)
        {
            var email = user.Email;
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = _emailOptions.SmtpEnableSsl;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailOptions.SmtpUserName, _emailOptions.SmtpPassword);
                smtpClient.Host = _emailOptions.SmtpHost;
                smtpClient.Port = _emailOptions.SmtpPort;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailOptions.FromEmail),
                    Subject = _emailOptions.Subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);
            }

            return Task.CompletedTask;
        }
    }
}
