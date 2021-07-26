// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Common.Exceptions;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.UI;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.Email.Services
{
    public class EmailAuthService : IEmailAuthService
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly IEnumerable<IOTPAuthenticator> _authenticators;
        private readonly OAuthHostOptions _options;
        private readonly EmailHostOptions _emailOptions;

        public EmailAuthService(
            IOAuthUserRepository oauthUserRepository, 
            IEnumerable<IOTPAuthenticator> authenticators,
            IOptions<OAuthHostOptions> options,
            IOptions<EmailHostOptions> emailOptions)
        {
            _oauthUserRepository = oauthUserRepository;
            _authenticators = authenticators;
            _options = options.Value;
            _emailOptions = emailOptions.Value;
        }

        public async Task<OAuthUser> Authenticate(string email, long otpCode, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.Email, email, cancellationToken);
            if (user == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.UNKNOWN_EMAIL);
            }

            var otpAuthenticator = GetOTPAuthenticator();
            if (!otpAuthenticator.Verify(otpCode, user))
            {
                throw new BaseUIException(Exceptions.ErrorCodes.INVALID_CONFIRMATIONCODE);
            }

            return user;
        }

        public async Task<long> SendCode(string email, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.Email, email, cancellationToken);
            if (user == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.UNKNOWN_EMAIL);
            }

            var otpAuthenticator = GetOTPAuthenticator();
            var otpCode = otpAuthenticator.GenerateOtp(user);
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
                    Body = string.Format(_emailOptions.HttpBody, otpCode),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);
            }
            
            return otpCode;
        }

        private IOTPAuthenticator GetOTPAuthenticator()
        {
            return _authenticators.First(a => a.Alg == _options.OTPAlg);
        }
    }
}