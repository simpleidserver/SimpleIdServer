// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Otp.Services;
using SimpleIdServer.IdServer.Otp.UI.ViewModels;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Otp.UI;

[Area(Constants.Amr)]
public class AuthenticateController : BaseOTPAuthenticateController<AuthenticateOtpViewModel>
{
    public AuthenticateController(
        IConfiguration configuration,
        IEnumerable<IUserNotificationService> notificationServices, 
        IEnumerable<IOTPAuthenticator> otpAuthenticators,
        IOtpAuthenticationService userAuthenticationService, 
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
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder, 
        IBusControl busControl,
        IAntiforgery antiforgery,
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
        ISessionManager sessionManager,
        IWorkflowStore workflowStore,
        IFormStore formStore,
        IOptions<FormBuilderOptions> formBuilderOptions) : base(configuration, notificationServices, otpAuthenticators, userAuthenticationService, authenticationSchemeProvider, options, dataProtectionProvider, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, tokenRepository, transactionBuilder, jwtBuilder, busControl, antiforgery, authenticationContextClassReferenceRepository, sessionManager, workflowStore, formStore, formBuilderOptions)
    {
    }

    protected override string FormattedMessage => string.Empty;

    protected override string Amr => Constants.Amr;

    protected override bool IsExternalIdProvidersDisplayed => false;

    protected override bool TryGetLogin(AmrAuthInfo amrInfo, out string login)
    {
        login = amrInfo.Login;
        return true;
    }
}
