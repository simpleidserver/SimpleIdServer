// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.UI.TmpPwd;

[Area(Constants.AreaTmpPwd)]
public class AuthenticateController : BaseAuthenticationMethodController<>
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IWorkflowStore _workflowStore;
    private readonly IFormStore _formStore;
    private readonly IAntiforgery _antiforgery;
    private readonly ILanguageRepository _languageRepository;
    private readonly IAcrHelper _acrHelper;
    private readonly ITemplateStore _templateStore;
    private readonly IUserRepository _userRepository;
    private readonly FormBuilderOptions _formBuilderOptions;

    public AuthenticateController(
            ITemplateStore templateStore,
            IConfiguration configuration,
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IUserAuthenticationService userAuthenticationService,
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            ITransactionBuilder transactionBuilder,
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
            IWorkflowHelper workflowHelper,
            IOptions<FormBuilderOptions> formBuilderOptions) : base(clientRepository, userRepository, userSessionRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, authenticationHelper, transactionBuilder, tokenRepository, jwtBuilder, workflowStore, formStore, acrHelper, authenticationContextClassReferenceRepository, workflowHelper, options)
    {
        _transactionBuilder = transactionBuilder;
        _workflowStore = workflowStore;
        _formStore = formStore;
        _antiforgery = antiforgery;
        _languageRepository = languageRepository;
        _acrHelper = acrHelper;
        _templateStore  = templateStore;
        _userRepository = userRepository;
        _formBuilderOptions = formBuilderOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var acrInfo = await AcrHelper.GetAcr(cancellationToken);
        var workflow = await _workflowStore.Get(prefix, acrInfo.Au, cancellationToken);
        var viewModel = await BuildWorkflowViewModel(prefix, acrInfo, cancellationToken);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, [FromQuery] ResetPasswordIndexViewModel vm, CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var workflow = await _workflowStore.Get(prefix, vm.WorkflowId, cancellationToken);
            var result = await BuildWorkflowViewModel(prefix, workflow, cancellationToken);
            var errors = vm.Validate(ModelState);
            if (errors.Any())
            {
                result.ErrorMessages = errors; ;
                result.SetInput(vm);
                return View(result);
            }

            var acr = await _acrHelper.GetAcr(cancellationToken);
            if (acr == null || string.IsNullOrWhiteSpace(acr.UserId))
            {
                result.SetErrorMessage(Global.UserNotAuthenticated);
                result.SetInput(vm);
                return View(result);
            }

            var user = await _userRepository.GetById(acr.UserId, cancellationToken);
            if (user == null)
            {
                result.SetErrorMessage(Global.CannotResolveUser);
                result.SetInput(vm);
                return View(result);
            }

            var credential = user.Credentials.SingleOrDefault(c => c.CredentialType == Constants.AreaPwd && c.IsActive);
            if (credential == null)
            {
                result.SetErrorMessage(Global.NoActivePassword);
                result.SetInput(vm);
                return View(result);
            }

            if(!credential.IsTemporary)
            {
                result.SetErrorMessage(Global.PasswordIsNotTemporary);
                result.SetInput(vm);
                return View(result);
            }

            credential.Value = PasswordHelper.ComputerHash(credential, vm.Password);
            credential.IsTemporary = false;
            _userRepository.Update(user);
            await transaction.Commit(cancellationToken);
            return await Authenticate(prefix, vm, Constants.AreaPwd, user, cancellationToken);
        }
    }

    private async Task<WorkflowViewModel> BuildWorkflowViewModel(string realm, WorkflowRecord workflow, CancellationToken cancellationToken)
    {
        var records = await _formStore.GetLatestPublishedVersionByCategory(realm, FormCategories.Authentication, cancellationToken);
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var languages = await _languageRepository.GetAll(cancellationToken);
        var template = await _templateStore.GetActive(realm, cancellationToken);
        return new SidWorkflowViewModel
        {
            Workflow = workflow,
            FormRecords = records,
            Languages = languages,
            Template = template,
            AntiforgeryToken = new AntiforgeryTokenRecord
            {
                CookieName = _formBuilderOptions.AntiforgeryCookieName,
                CookieValue = tokenSet.CookieToken,
                FormField = tokenSet.FormFieldName,
                FormValue = tokenSet.RequestToken
            },
            CurrentStepId = workflow.Steps.OrderBy(s => workflow.ComputeLevel(s)).First().Id
        };
    }
}
