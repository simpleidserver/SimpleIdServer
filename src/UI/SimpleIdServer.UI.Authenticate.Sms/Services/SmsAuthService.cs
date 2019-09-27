// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Exceptions;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.Sms.Services
{
    public class SmsAuthService : ISmsAuthService
    {
        private readonly IOAuthUserQueryRepository _oauthUserRepository;
        private readonly IDistributedCache _distributedCache;

        public SmsAuthService(IOAuthUserQueryRepository oauthUserRepository, IDistributedCache distributedCache)
        {
            _oauthUserRepository = oauthUserRepository;
            _distributedCache = distributedCache;
        }

        public async Task<OAuthUser> Authenticate(string phoneNumber, string confirmationCode)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.PhoneNumber, phoneNumber);
            if (user == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.UNKNOWN_PHONENUMBER);
            }

            var persistedPhoneNumber = await _distributedCache.GetStringAsync(confirmationCode);
            if (persistedPhoneNumber == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.INVALID_CONFIRMATIONCODE);
            }

            if (persistedPhoneNumber != phoneNumber)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.INVALID_PHONENUMBER);
            }

            return user;
        }

        public async Task<string> SendConfirmationCode(string phoneNumber)
        {
            var confirmationCode = Guid.NewGuid().ToString("N");
            await _distributedCache.SetStringAsync(confirmationCode, phoneNumber);
            return confirmationCode;
        }
    }
}