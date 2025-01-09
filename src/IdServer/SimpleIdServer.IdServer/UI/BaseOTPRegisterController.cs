// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public abstract class BaseOTPRegisterController<TOptions, TViewModel> : BaseRegisterController<TViewModel> where TOptions : IOTPRegisterOptions
    where TViewModel : OTPRegisterViewModel
{
    private readonly FormBuilderOptions _formBuilderOptions;
    private readonly IUserRepository _userRepository;
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
    private readonly IConfiguration _configuration;
    private readonly IUserNotificationService _userNotificationService;
    private readonly IAntiforgery _antiforgery;
    private readonly IFormStore _formStore;
    private readonly IWorkflowStore _workflowStore;
    private readonly ILogger<BaseOTPRegisterController<TOptions, TViewModel>> _logger;

    public BaseOTPRegisterController(
        ILogger<BaseOTPRegisterController<TOptions, TViewModel>> logger,
        IOptions<IdServerHostOptions> options, 
        IOptions<FormBuilderOptions> formOptions,
        IDistributedCache distributedCache, 
        IUserRepository userRepository, 
        ITransactionBuilder transactionBuilder,
        IEnumerable<IOTPAuthenticator> otpAuthenticators, 
        IConfiguration configuration, 
        IUserNotificationService userNotificationService,
        IAntiforgery antiforgery,
        IFormStore formStore,
        IWorkflowStore workflowStore,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(options, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder)
    {
        _logger = logger;
        _formBuilderOptions = formOptions.Value;
        _userRepository = userRepository;
        _otpAuthenticators = otpAuthenticators;
        _configuration = configuration;
        _userNotificationService = userNotificationService;
        _antiforgery = antiforgery;
        _formStore = formStore;
        _workflowStore = workflowStore;
    }

    protected abstract string Amr { get; }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null)
    {
        prefix = prefix ?? Constants.Prefix;
        var isAuthenticated = User.Identity.IsAuthenticated;
        var registrationProgress = await GetRegistrationProgress();
        if (registrationProgress == null && !isAuthenticated)
        {
            var res = new WorkflowViewModel();
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        var viewModel = Activator.CreateInstance<TViewModel>();
        var result = await BuildViewModel(registrationProgress, prefix, isAuthenticated, viewModel);
        viewModel.RedirectUrl = redirectUrl;
        result.SetInput(viewModel);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, TViewModel viewModel)
    {
        prefix = prefix ?? Constants.Prefix;
        var isAuthenticated = User.Identity.IsAuthenticated;
        UserRegistrationProgress userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var res = new WorkflowViewModel();
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        // 1. Check parameters.
        var result = await BuildViewModel(userRegistrationProgress, prefix, isAuthenticated, viewModel);
        var errorMessages = viewModel.Validate();
        if (errorMessages.Any())
        {
            result.ErrorMessages = errorMessages;
            result.SetInput(viewModel);
            return View(result);
        }

        // 2. Check the user exists.
        var emailIsTaken = await IsUserExists(viewModel.Value, prefix);
        if (emailIsTaken)
        {
            result.SetErrorMessage(string.Format(Global.UserWithSameClaimAlreadyExists, Amr));
            result.SetInput(viewModel);
            return View(result);
        }

        // 3. Send the confirmation code.
        var options = GetOptions();
        var optAlg = options.OTPAlg;
        var otpAuthenticator = _otpAuthenticators.First(o => o.Alg == optAlg);
        if (viewModel.Action == "SENDCONFIRMATIONCODE")
        {
            return await SendConfirmationCode();
        }

        // 4. Check the confirmation code.
        var enteredOtpCode = viewModel.OTPCode;
        var isOtpCodeCorrect = otpAuthenticator.Verify(enteredOtpCode, new UserCredential
        {
            OTPAlg = optAlg,
            Value = options.OTPValue,
            OTPCounter = options.OTPCounter,
            CredentialType = UserCredential.OTP,
            IsActive = true
        });
        if (!isOtpCodeCorrect)
        {
            result.SetErrorMessage(Global.OtpCodeIsInvalid);
            result.SetInput(viewModel);
            return View(result);
        }

        // 5. Update the user.
        var key = enteredOtpCode.ToString();
        var value = await DistributedCache.GetStringAsync(key);
        await DistributedCache.RemoveAsync(key);
        viewModel.Value = value;
        if (User.Identity.IsAuthenticated) return await UpdateAuthenticatedUser();

        // 6. Register a user.
        return await RegisterUser();

        async Task<IActionResult> SendConfirmationCode()
        {
            var otpCode = otpAuthenticator.GenerateOtp(new Domains.UserCredential
            {
                OTPAlg = optAlg,
                Value = options.OTPValue,
                OTPCounter = options.OTPCounter,
                CredentialType = UserCredential.OTP,
                IsActive = true
            });
            try
            {
                await _userNotificationService.Send("One Time Password", string.Format(options.HttpBody, otpCode), new Dictionary<string, string>(), viewModel.Value);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                result.SetErrorMessage(Global.ImpossibleToSendOtpCode);
                result.SetInput(viewModel);
                return View(result);
            }

            await DistributedCache.SetStringAsync(otpCode, viewModel.Value, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(2)
            });
            result.SetSuccessMessage(Global.OtpCodeIsSent);
            result.SetInput(viewModel);
            return View(result);
        }

        async Task<IActionResult> UpdateAuthenticatedUser()
        {
            using (var transaction = TransactionBuilder.Build())
            {
                var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var authenticatedUser = await _userRepository.GetBySubject(nameIdentifier, prefix, CancellationToken.None);
                BuildUser(authenticatedUser, viewModel);
                authenticatedUser.UpdateDateTime = DateTime.UtcNow;
                if (authenticatedUser.ActiveOTP == null)
                    authenticatedUser.GenerateTOTP();

                _userRepository.Update(authenticatedUser);
                await transaction.Commit(CancellationToken.None);
                return await base.UpdateUser(result, userRegistrationProgress, viewModel, Amr, viewModel.RedirectUrl);
            }
        }

        async Task<IActionResult> RegisterUser()
        {
            return await base.CreateUser(result, userRegistrationProgress, viewModel, prefix, Amr, viewModel.RedirectUrl);
        }
    }

    protected abstract void Enrich(TViewModel viewModel, User user);

    protected async Task<WorkflowViewModel> BuildViewModel(UserRegistrationProgress registrationProcess, string prefix, bool isAuthenticated, TViewModel viewModel)
    {
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var records = await _formStore.GetAll(CancellationToken.None);
        var workflow = await _workflowStore.Get(registrationProcess.WorkflowId, CancellationToken.None);
        var workflowFormIds = workflow.Steps.Select(s => s.FormRecordId);
        var filteredRecords = records.Where(r => workflowFormIds.Contains(r.Id));
        var record = filteredRecords.Single(r => r.Name == Amr);
        var step = workflow.GetStep(record.Id);
        var result = new WorkflowViewModel
        {
            CurrentStepId = step.Id,
            Workflow = workflow,
            FormRecords = records,
            AntiforgeryToken = new AntiforgeryTokenRecord
            {
                CookieName = _formBuilderOptions.AntiforgeryCookieName,
                CookieValue = tokenSet.CookieToken,
                FormField = tokenSet.FormFieldName,
                FormValue = tokenSet.RequestToken
            }
        };
        viewModel.Realm = prefix;
        if (isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var authenticatedUser = await _userRepository.GetBySubject(nameIdentifier, prefix, CancellationToken.None);
            Enrich(viewModel, authenticatedUser);
        }

        return result;
    }

    protected abstract Task<bool> IsUserExists(string value, string prefix);

    protected abstract void BuildUser(User user, TViewModel viewModel);

    protected override void EnrichUser(User user, TViewModel viewModel)
    {
        BuildUser(user, viewModel);
        if (user.ActiveOTP == null) user.GenerateTOTP();
    }

    private TOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(TOptions).Name);
        return section.Get<TOptions>();
    }
}
