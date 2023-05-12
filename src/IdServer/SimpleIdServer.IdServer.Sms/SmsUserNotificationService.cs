// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SimpleIdServer.IdServer.Sms
{
    public interface ISmsUserNotificationService : IUserNotificationService { }

    public class SmsUserNotificationService : ISmsUserNotificationService
    {
        private readonly IdServerSmsOptions _smsHostOptions;

        public SmsUserNotificationService(IOptions<IdServerSmsOptions> smsHostOptions)
        {
            _smsHostOptions = smsHostOptions.Value;
        }

        public string Name => Constants.AMR;

        public async Task Send(string message, User user)
        {
            var phoneNumber = user.OAuthUserClaims.First(c => c.Name == JwtRegisteredClaimNames.PhoneNumber).Value;
            TwilioClient.Init(_smsHostOptions.AccountSid, _smsHostOptions.AuthToken);
            await MessageResource.CreateAsync(
                body: string.Format(_smsHostOptions.Message, message),
                from: new PhoneNumber(_smsHostOptions.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber));
        }
    }
}
