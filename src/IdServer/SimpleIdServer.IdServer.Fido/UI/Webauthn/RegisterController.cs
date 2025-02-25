// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Fido.UI.Webauthn;

[Area(Constants.AMR)]
public class RegisterController : BaseRegisterController<RegisterWebauthnViewModel>
{
    public RegisterController(
        IOptions<IdServerHostOptions> options, 
        IOptions<FormBuilderOptions> formOptions,
        IDistributedCache distributedCache, 
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder,
        IAntiforgery antiforgery,
        IFormStore formStore,
        IWorkflowStore workflowStore,
        ILanguageRepository languageRepository,
        IRealmStore realmStore) : base(options, formOptions, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, antiforgery, formStore, workflowStore, languageRepository, realmStore)
    {
    }

    protected override string Amr => Constants.AMR;

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var newPrefix = prefix;
        if (!string.IsNullOrWhiteSpace(prefix))
            newPrefix = $"{prefix}/";
        var isAuthenticated = User.Identity.IsAuthenticated;
        var userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var res = new SidWorkflowViewModel();
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        var login = string.Empty;
        if (!isAuthenticated) login = userRegistrationProgress.User?.Name;
        else login = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var vm = new RegisterWebauthnViewModel
        {
            Login = login,
            BeginRegisterUrl = $"{issuer}/{newPrefix}{Constants.EndPoints.BeginRegister}",
            EndRegisterUrl = $"{issuer}/{newPrefix}{Constants.EndPoints.EndRegister}",
            ReturnUrl = userRegistrationProgress?.RedirectUrl ?? redirectUrl
        };
        var result = await BuildViewModel(userRegistrationProgress, vm, prefix, cancellationToken);
        result.SetInput(vm);
        return View(result);
    }

    protected override WorkflowRecord BuildNewUpdateCredentialWorkflow()
    {
        return StandardFidoRegistrationWorkflows.WebauthnWorkflow;
    }

    protected override void EnrichUser(User user, RegisterWebauthnViewModel viewModel) { }
}
