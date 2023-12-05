// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Email.Services
{
    public class UserEmailResetPasswordService : BaseOTPResetPasswordService
    {
        public UserEmailResetPasswordService(
            IConfiguration configuration,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IEmailUserNotificationService userNotificationService,
            IGrantedTokenHelper grantedTokenHelper,
            IAuthenticationHelper authenticationHelper) : base(configuration, otpAuthenticators, userNotificationService, grantedTokenHelper, authenticationHelper)
        {
        }

        public override string NotificationMode => Constants.AMR;

        public override string GetDestination(User user) => user.Email;

        protected override IOTPRegisterOptions GetOTPOptions()
        {
            var section = Configuration.GetSection(typeof(IdServerEmailOptions).Name);
            return section.Get<IdServerEmailOptions>();
        }
    }
}
