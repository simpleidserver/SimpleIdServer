// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Email.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseOTPRegisterController<IdServerEmailOptions, RegisterEmailViewModel>
{
    private readonly IAuthenticationHelper _authenticationHelper;
    public RegisterController(
        ILogger<BaseOTPRegisterController<IdServerEmailOptions, RegisterEmailViewModel>> logger,
        IOptions<FormBuilderOptions> formOptions,
        IAuthenticationHelper authenticationHelper,
        IOptions<IdServerHostOptions> options,
        IDistributedCache distributedCache,
        IUserRepository userRepository,
        ITransactionBuilder transactionBuilder,
        IEnumerable<IOTPAuthenticator> otpAuthenticators,
        IConfiguration configuration,
        IEmailUserNotificationService userNotificationService,
        ITokenRepository tokenRepository,
        IAntiforgery antiforgery,
        IFormStore formStore,
        IWorkflowStore workflowStore,
        IJwtBuilder jwtBuilder,
        ILanguageRepository languageRepository,
        IRealmStore realmStore) : base(logger, options, formOptions, distributedCache, userRepository, transactionBuilder, otpAuthenticators, configuration, userNotificationService, antiforgery, formStore, workflowStore, tokenRepository, jwtBuilder, languageRepository, realmStore)
    {
        _authenticationHelper = authenticationHelper;
    }

    protected override string Amr => Constants.AMR;

    protected override void Enrich(RegisterEmailViewModel viewModel, User user)
    {
        viewModel.Value = user.Email;
        viewModel.IsVerified = user.EmailVerified;
    }

    protected override void BuildUser(User user, RegisterEmailViewModel viewModel)
    {
        user.Email = viewModel.Value;
        user.EmailVerified = true;
    }

    protected override async Task<bool> IsUserExists(string value, string prefix)
    {
        string nameIdentifier = string.Empty;
        if(User.Identity.IsAuthenticated) nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        if(!string.IsNullOrWhiteSpace(nameIdentifier))
        {
            return await _authenticationHelper.AtLeastOneUserWithSameEmail(nameIdentifier, value, prefix, CancellationToken.None);
        }

        return await UserRepository.IsEmailExists(value, prefix, CancellationToken.None);
    }
}
