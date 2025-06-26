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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Captcha;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.UI.TmpPwd;

[Area(Constants.AreaTmpPwd)]
public class AuthenticateController : BaseAuthenticationMethodController<ResetTemporaryPasswordViewModel>
{
    private readonly IPasswordValidationService _passwordValidationService;

    public AuthenticateController(
        ITemplateStore templateStore,
        IConfiguration configuration,
        IAuthenticationHelper authenticationHelper,
        IDistributedCache distributedCache,
        IAuthenticationSchemeProvider authenticationSchemeProvider,
        IPasswordAuthenticationService userAuthenticationService,
        IOptions<IdServerHostOptions> options,
        IDataProtectionProvider dataProtectionProvider,
        ITokenRepository tokenRepository,
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder,
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
        IWorkflowHelper workflowHelper,
        ICaptchaValidatorFactory captchaValidatorFactory,
        IPasswordValidationService passwordValidationService,
        IOptions<FormBuilderOptions> formBuilderOptions) : base(templateStore, configuration, options, authenticationSchemeProvider, userAuthenticationService, dataProtectionProvider, tokenRepository, transactionBuilder, jwtBuilder, authenticationHelper, clientRepository, amrHelper, userRepository, userSessionRepository, userTransformer, busControl, antiforgery, authenticationContextClassReferenceRepository, sessionManager, workflowStore, formStore, languageRepository, acrHelper, workflowHelper, captchaValidatorFactory, formBuilderOptions)
    {
        _passwordValidationService = passwordValidationService;
    }

    protected override string Amr => Constants.AreaTmpPwd;

    protected override bool IsExternalIdProvidersDisplayed => false;

    protected override bool TryGetLogin(AcrAuthInfo amr, out string login)
    {
        login = null;
        if (amr == null || string.IsNullOrWhiteSpace(amr.Login)) return false;
        login = amr.Login;
        return true;
    }

    protected override async Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, ResetTemporaryPasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        using (var transaction = TransactionBuilder.Build())
        {
            var user = await UserRepository.GetById(authenticatedUserId, cancellationToken);
            if (user == null)
            {
                return UserAuthenticationResult.Error(AuthFormErrorMessages.CannotResolveUser);
            }

            var credential = user.Credentials.SingleOrDefault(c => c.CredentialType == Constants.AreaPwd && c.IsActive);
            if (credential == null)
            {
                return UserAuthenticationResult.Error(AuthFormErrorMessages.NoActivePassword);
            }

            if (!credential.IsTemporary)
            {
                return UserAuthenticationResult.Error(AuthFormErrorMessages.PasswordIsNotTemporary);
            }

            var passwordValidationResult = _passwordValidationService.Validate(viewModel.Password);
            if (passwordValidationResult != null)
            {
                return UserAuthenticationResult.Error(passwordValidationResult.Select(p => p.code).ToList());
            }

            credential.Value = PasswordHelper.ComputerHash(credential, viewModel.Password);
            credential.IsTemporary = false;
            UserRepository.Update(user);
            await transaction.Commit(cancellationToken);
            return UserAuthenticationResult.Ok(user);
        }
    }

    protected override void EnrichViewModel(ResetTemporaryPasswordViewModel viewModel)
    {

    }
}
