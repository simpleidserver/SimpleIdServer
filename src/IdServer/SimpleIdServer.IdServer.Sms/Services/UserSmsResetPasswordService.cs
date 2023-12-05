// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Sms.Services
{
    public class UserSmsResetPasswordService : BaseOTPResetPasswordService
    {
        public UserSmsResetPasswordService(
            IConfiguration configuration, 
            IEnumerable<IOTPAuthenticator> otpAuthenticators, 
            IUserNotificationService notificationService, 
            IGrantedTokenHelper grantedTokenHelper,
            IAuthenticationHelper authenticationHelper) : base(configuration, otpAuthenticators, notificationService, grantedTokenHelper, authenticationHelper)
        {
        }

        public override string NotificationMode => Constants.AMR;

        public override string GetDestination(User user)
        {
            var phoneNumberClaim = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber);
            return phoneNumberClaim?.Value;
        }

        protected override IOTPRegisterOptions GetOTPOptions()
        {
            var section = Configuration.GetSection(typeof(IdServerSmsOptions).Name);
            return section.Get<IdServerSmsOptions>();
        }
    }
}
