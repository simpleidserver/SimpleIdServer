// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using System.Net;
using System.Net.Mail;

namespace SimpleIdServer.IdServer.Email.UI.Services
{
    public interface IEmailAuthService
    {
        Task<User> Authenticate(string email, long otpCode, CancellationToken cancellationToken);
        Task<long> SendCode(string email, CancellationToken cancellationToken);
    }

    public class EmailAuthService : IEmailAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
        private readonly IdServerHostOptions _options;
        private readonly IdServerEmailOptions _emailOptions;

        public EmailAuthService(IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IOptions<IdServerHostOptions> options, IOptions<IdServerEmailOptions> emailOptions)
        {
            _userRepository= userRepository;
            _otpAuthenticators = otpAuthenticators;
            _options = options.Value;
            _emailOptions = emailOptions.Value;
        }

        public async Task<User> Authenticate(string email, long otpCode, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Query().Include(u => u.OAuthUserClaims).FirstOrDefaultAsync(u => u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.Email && c.Value == email), cancellationToken);
            if (user == null)
                throw new BaseUIException("unknown_email");

            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == _options.DefaultOTPAlg);
            if(!otpAuthenticator.Verify(otpCode, user))
                throw new BaseUIException("invalid_confirmationcode");
            return user;
        }

        public async Task<long> SendCode(string email, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Query().Include(u => u.OAuthUserClaims).FirstOrDefaultAsync(u => u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.Email && c.Value == email), cancellationToken);
            if (user == null)
                throw new BaseUIException("unknown_email");

            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == _options.DefaultOTPAlg);
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
    }
}
