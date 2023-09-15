// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public SmsUserNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => Constants.AMR;

        public async Task Send(string message, User user)
        {
            var smsHostOptions = GetOptions();
            var phoneNumber = user.OAuthUserClaims.First(c => c.Name == JwtRegisteredClaimNames.PhoneNumber).Value;
            TwilioClient.Init(smsHostOptions.AccountSid, smsHostOptions.AuthToken);
            await MessageResource.CreateAsync(
                body: string.Format(smsHostOptions.Message, message),
                from: new PhoneNumber(smsHostOptions.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber));
        }

        private IdServerSmsOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerSmsOptions).Name);
            return section.Get<IdServerSmsOptions>();
        }
    }
}
