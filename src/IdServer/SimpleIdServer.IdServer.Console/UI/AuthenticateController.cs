// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Console.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Console.UI;

[Area(Constants.AMR)]
public class AuthenticateController : BaseOTPAuthenticateController<AuthenticateConsoleViewModel>
{
    private readonly IConfiguration _configuration;

    public AuthenticateController(
        IConfiguration configuration,
        IEnumerable<IUserNotificationService> notificationServices, 
        IEnumerable<IOTPAuthenticator> otpAuthenticators,
        IUserAuthenticationService userAuthenticationService, 
        IAuthenticationSchemeProvider authenticationSchemeProvider, 
        IOptions<IdServerHostOptions> options,
        IDataProtectionProvider dataProtectionProvider, 
        IAuthenticationHelper authenticationHelper,
        IClientRepository clientRepository, 
        IAmrHelper amrHelper, 
        IUserRepository userRepository, 
        IUserSessionResitory userSessionRepository, 
        IUserTransformer userTransformer, 
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder, 
        IBusControl busControl) : base(notificationServices, otpAuthenticators, userAuthenticationService, authenticationSchemeProvider, options, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, tokenRepository, jwtBuilder, busControl)
    {
        _configuration = configuration;
    }

    protected override string FormattedMessage => GetOptions()?.HttpBody;

    protected override string Amr => Constants.AMR;

    protected override bool IsExternalIdProvidersDisplayed => false;

    protected override bool TryGetLogin(AmrAuthInfo amrInfo, out string login)
    {
        login = amrInfo.Login;
        return true;
    }

    private IdServerConsoleOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(IdServerConsoleOptions).Name);
        return section.Get<IdServerConsoleOptions>();
    }
}
