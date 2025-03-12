// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Console;

public class UserConsolePasswordResetService : BaseOTPResetPasswordService
{
    public UserConsolePasswordResetService(
        IConfiguration configuration, 
        IEnumerable<IOTPAuthenticator> otpAuthenticators,
        IUserConsoleNotificationService notificationService, 
        IGrantedTokenHelper grantedTokenHelper, 
        IAuthenticationHelper authenticationHelper) : base(configuration, otpAuthenticators, notificationService, grantedTokenHelper, authenticationHelper)
    {
    }

    public override string NotificationMode => Constants.ConsoleAmr;

    public override string GetDestination(User user) => Constants.ConsoleAmr;

    protected override IOTPRegisterOptions GetOTPOptions()
    {
        var section = Configuration.GetSection(typeof(IdServerConsoleOptions).Name);
        return section.Get<IdServerConsoleOptions>() ?? new IdServerConsoleOptions();
    }
}
