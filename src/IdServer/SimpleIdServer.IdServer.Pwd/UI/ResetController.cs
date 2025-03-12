// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Pwd.UI;

[Area(Constants.Areas.Password)]
public class ResetController : BaseController
{
    private readonly IdServerHostOptions _options;
    private readonly IEnumerable<IResetPasswordService> _resetPasswordServices;
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly IConfiguration _configuration;
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IFormStore _formStore;
    private readonly IAntiforgery _antiforgery;
    private readonly IWorkflowStore _workflowStore;
    private readonly ILanguageRepository _languageRepository;
    private readonly IRealmStore _realmStore;
    private readonly ILogger<ResetController> _logger;
    private readonly FormBuilderOptions _formBuilderOptions;

    public ResetController(
        IOptions<IdServerHostOptions> options,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        IEnumerable<IResetPasswordService> resetPasswordServices,
        IAuthenticationHelper authenticationHelper,
        IConfiguration configuration,
        IGrantedTokenHelper grantedTokenHelper,
        IUserRepository userRepository,
        ITransactionBuilder transactionBuilder,
        IFormStore formStore,
        IAntiforgery antiforgery,
        IWorkflowStore workflowStore,
        ILanguageRepository languageRepository,
        IRealmStore realmStore,
        ILogger<ResetController> logger,
        IOptions<FormBuilderOptions> formBuilderOptions) : base(tokenRepository, jwtBuilder)
    {
        _options = options.Value;
        _resetPasswordServices = resetPasswordServices;
        _authenticationHelper = authenticationHelper;
        _configuration = configuration;
        _grantedTokenHelper = grantedTokenHelper;
        _userRepository = userRepository;
        _transactionBuilder = transactionBuilder;
        _formStore = formStore;
        _antiforgery = antiforgery;
        _workflowStore = workflowStore;
        _languageRepository = languageRepository;
        _realmStore = realmStore;
        _logger = logger;
        _formBuilderOptions = formBuilderOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, [FromQuery] ResetPasswordIndexViewModel vm, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        string login = null;
        var notificationMode = GetOptions()?.NotificationMode ?? Constants.DefaultNotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        if (User.Identity.IsAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _authenticationHelper.GetUserByLogin(nameIdentifier, prefix, cancellationToken);
            login = _authenticationHelper.GetLogin(user);
        }

        var viewModel = new ResetPasswordViewModel
        {
            Login = login,
            NotificationMode = notificationMode,
            Value = null,
            ReturnUrl = vm.ReturnUrl,
            Realm = _realmStore.Realm,
            WorkflowId = vm.WorkflowId,
            StepId = vm.StepId,
            CurrentLink = vm.CurrentLink
        };
        var result = await BuildWorkflowViewModel(prefix, vm, cancellationToken);
        result.SetInput(viewModel);
        result.MoveNextStep(vm);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, ResetPasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var result = await BuildWorkflowViewModel(prefix, viewModel, cancellationToken);
        result.StayCurrentStep(viewModel);
        result.SetInput(viewModel);
        viewModel.Realm = prefix;
        // 1. Validate the view model.
        var validationResult = viewModel.Validate(ModelState);
        if(validationResult.Any())
        {
            result.ErrorMessages = validationResult;
            return View(result);
        }

        // 2. Check the user exists.
        var user = await GetUser();
        if(user == null)
        {
            result.SetErrorMessage(Global.UserIsUnknown);
            return View(result);
        }

        // 3. Check the destination.
        var options = GetOptions();
        var notificationMode = options.NotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        var destination = service.GetDestination(user);
        if(string.IsNullOrWhiteSpace(destination))
        {
            result.SetErrorMessage(Global.MissingDestination);
            return View(result);
        }

        if(viewModel.Value != destination)
        {
            result.SetErrorMessage(Global.InvalidDestination);
            return View(result);
        }

        // 4. Send the OTP code.
        var url = Url.Action("Confirm", "Reset", new
        {
            area = Constants.Areas.Password
        });
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var parameter = new ResetPasswordParameter(
            $"{issuer}{url}", 
            user, 
            prefix, 
            options.ResetPasswordBody, 
            options.ResetPasswordTitle, 
            options.ResetPasswordLinkExpirationInSeconds,
            viewModel.ReturnUrl);
        try
        {
            await service.SendResetLink(parameter, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            result.SetErrorMessage(Global.CannotSendOtpCode);
            return View(result);
        }

        result.SetSuccessMessage(Global.OtpCodeIsSent);
        return View(result);

        async Task<User> GetUser()
        {
            var login = viewModel.Login;
            if(User.Identity.IsAuthenticated)
            {
                login = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }

            var user = await _authenticationHelper.GetUserByLogin(login, prefix, cancellationToken);
            return user;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Confirm([FromRoute] string prefix, string code, string returnUrl, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var resetPasswordLink = await _grantedTokenHelper.GetResetPasswordLink(code.ToString(), cancellationToken);
        if(resetPasswordLink == null)
        {
            var vm = new SidWorkflowViewModel();
            vm.SetErrorMessage(Global.InvalidResetLink);
            return View(vm);
        }

        var user = await _authenticationHelper.GetUserByLogin(resetPasswordLink.Login, prefix, cancellationToken);
        var options = GetOptions();
        var notificationMode = options.NotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        var destination = service.GetDestination(user);
        var viewModel = new ConfirmResetPasswordViewModel
        {
            Destination = destination,
            Code = code,
            ReturnUrl = returnUrl,
            Realm = _realmStore.Realm
        };
        var result = await BuildWorkflowViewModel(prefix, options.ConfirmResetPasswordWorkflowId, cancellationToken);
        result.SetInput(viewModel);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm([FromRoute] string prefix, ConfirmResetPasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var result = await BuildWorkflowViewModel(prefix, viewModel, cancellationToken);
            var errors = viewModel.Validate(ModelState);
            if (errors.Any())
            {
                result.ErrorMessages = errors;;
                result.SetInput(viewModel);
                return View(result);
            }

            var notificationMode = GetOptions().NotificationMode;
            var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
            var resetPasswordLink = await service.Verify(viewModel.Code, cancellationToken);
            if (resetPasswordLink == null)
            {
                result.SetErrorMessage(Global.OtpCodeIsInvalid);
                result.SetInput(viewModel);
                return View(result);
            }

            var user = await _authenticationHelper.GetUserByLogin(resetPasswordLink.Login, prefix, cancellationToken);
            var credential = user.Credentials.SingleOrDefault(c => c.CredentialType == Constants.Areas.Password && c.IsActive);
            if (credential == null)
            {
                credential = new UserCredential
                {
                    Id = Guid.NewGuid().ToString(),
                    CredentialType = Constants.Areas.Password,
                    IsActive = true
                };
                user.Credentials.Add(credential);
            }

            credential.Value = PasswordHelper.ComputeHash(viewModel.Password, _options.IsPasswordEncodeInBase64);
            _userRepository.Update(user);
            await transaction.Commit(cancellationToken);
            viewModel.IsPasswordUpdated = true;
            result.SetInput(viewModel);
            result.SetSuccessMessage(Global.PasswordIsUpdated);
            return View(result);
        }
    }

    private IdServerPasswordOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(IdServerPasswordOptions).Name);
        return section.Get<IdServerPasswordOptions>() ?? new IdServerPasswordOptions();
    }

    private Task<WorkflowViewModel> BuildWorkflowViewModel(string realm, IStepViewModel viewModel, CancellationToken cancellationToken)
        => BuildWorkflowViewModel(realm, viewModel.WorkflowId, cancellationToken);

    private async Task<WorkflowViewModel> BuildWorkflowViewModel(string realm, string workflowId, CancellationToken cancellationToken)
    {
        var records = await _formStore.GetLatestPublishedVersionByCategory(realm, FormCategories.Authentication, cancellationToken);
        var workflow = await _workflowStore.Get(realm, workflowId, cancellationToken);
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var languages = await _languageRepository.GetAll(cancellationToken);
        var amrs = WorkflowHelper.ExtractAmrs(workflow, records);
        return new SidWorkflowViewModel
        {
            Workflow = workflow,
            FormRecords = records,
            Languages = languages,
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
