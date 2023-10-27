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

        public Task Send(string message, User user, ICollection<UserClaim> claims)
        {
            var phoneNumber = claims.First(c => c.Name == JwtRegisteredClaimNames.PhoneNumber).Value;
            return Send(message, phoneNumber);
        }

        public async Task Send(string message, string destination)
        {
            var smsHostOptions = GetOptions();
            TwilioClient.Init(smsHostOptions.AccountSid, smsHostOptions.AuthToken);
            await MessageResource.CreateAsync(
                body: string.Format(smsHostOptions.Message, message),
                from: new PhoneNumber(smsHostOptions.FromPhoneNumber),
                to: new PhoneNumber(destination));
        }

        private IdServerSmsOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerSmsOptions).Name);
            return section.Get<IdServerSmsOptions>();
        }
    }
}
