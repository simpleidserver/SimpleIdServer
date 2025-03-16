// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.UI;

[Area(Constants.AreaPwd)]
public class AuthenticateController : BaseAuthenticationMethodController<AuthenticatePasswordViewModel>
{
    private readonly IConfiguration _configuration;

    public AuthenticateController(
        IConfiguration configuration,
        IPasswordAuthenticationService userAuthenticationService,
        IAuthenticationSchemeProvider authenticationSchemeProvider,
        IOptions<IdServerHostOptions> options,
        IDataProtectionProvider dataProtectionProvider,
        ITokenRepository tokenRepository,
        ITransactionBuilder transactionBuidler,
        IJwtBuilder jwtBuilder,
        IAuthenticationHelper authenticationHelper,
        IClientRepository clientRepository,
        IAmrHelper amrHelper,
        IUserRepository userRepository,
        IUserSessionResitory userSessionRepository,
        IUserTransformer userTransformer,
        IBusControl busControl,
        IAntiforgery antiforgery,
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
        ISessionManager sessionManager,
        IWorkflowStore workflowStore,
        IFormStore formStore,
        ILanguageRepository languageRepository,
        IAcrHelper acrHelper,
        IOptions<FormBuilderOptions> formBuilderOptions) : base(configuration, options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, tokenRepository, transactionBuidler, jwtBuilder, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, busControl, antiforgery, authenticationContextClassReferenceRepository, sessionManager, workflowStore, formStore, languageRepository, acrHelper, formBuilderOptions)
    {
        _configuration = configuration;
    }

    protected override string Amr => Constants.AreaPwd;

    protected override bool IsExternalIdProvidersDisplayed => true;

    protected override Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        return Task.FromResult(UserAuthenticationResult.Ok());
    }

    protected override bool TryGetLogin(AcrAuthInfo amrInfo, out string login)
    {
        login = amrInfo.Login;
        return true;
    }

    protected override void EnrichViewModel(AuthenticatePasswordViewModel viewModel)
    {
        var options = GetOptions();
        viewModel.CanResetPassword = options?.CanResetPassword ?? false;
    }

    private IdServerPasswordOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(IdServerPasswordOptions).Name);
        return section.Get<IdServerPasswordOptions>();
    }
}
