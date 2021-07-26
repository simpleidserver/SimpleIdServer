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
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SimpleIdServer.UI.Authenticate.Sms.Services
{
    public class SmsAuthService : ISmsAuthService
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly IEnumerable<IOTPAuthenticator> _authenticators;
        private readonly OAuthHostOptions _options;
        private readonly SmsHostOptions _smsHostOptions;

        public SmsAuthService(
            IOAuthUserRepository oauthUserRepository,
            IEnumerable<IOTPAuthenticator> authenticators,
            IOptions<OAuthHostOptions> options,
            IOptions<SmsHostOptions> smsHostOptions)
        {
            _oauthUserRepository = oauthUserRepository;
            _authenticators = authenticators;
            _options = options.Value;
            _smsHostOptions = smsHostOptions.Value;
        }

        public async Task<OAuthUser> Authenticate(string phoneNumber, long code, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.PhoneNumber, phoneNumber, cancellationToken);
            if (user == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.UNKNOWN_PHONENUMBER);
            }

            var otpAuthenticator = GetOTPAuthenticator();
            if (!otpAuthenticator.Verify(code, user))
            {
                throw new BaseUIException(Exceptions.ErrorCodes.INVALID_CONFIRMATIONCODE);
            }

            return user;
        }

        public async Task<long> SendCode(string phoneNumber, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.PhoneNumber, phoneNumber, cancellationToken);
            if (user == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.UNKNOWN_PHONENUMBER);
            }

            var otpAuthenticator = GetOTPAuthenticator();
            var otpCode = otpAuthenticator.GenerateOtp(user);
            TwilioClient.Init(_smsHostOptions.AccountSid, _smsHostOptions.AuthToken);
            await MessageResource.CreateAsync(
                body: string.Format(_smsHostOptions.Message, otpCode),
                from: new PhoneNumber(_smsHostOptions.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber));
            return otpCode;
        }

        private IOTPAuthenticator GetOTPAuthenticator()
        {
            return _authenticators.First(a => a.Alg == _options.OTPAlg);
        }
    }
}