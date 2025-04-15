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
using SimpleIdServer.IdServer.Helpers;
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
    private readonly IUserRepository _userRepository;
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
    private readonly IConfiguration _configuration;
    private readonly IUserNotificationService _userNotificationService;
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
        IJwtBuilder jwtBuilder,
        ILanguageRepository languageRepository,
        IRealmStore realmStore,
        ITemplateStore templateStore) : base(options, formOptions, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, antiforgery, formStore, workflowStore, languageRepository, realmStore, templateStore)
    {
        _logger = logger;
        _userRepository = userRepository;
        _otpAuthenticators = otpAuthenticators;
        _configuration = configuration;
        _userNotificationService = userNotificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var isAuthenticated = User.Identity.IsAuthenticated;
        var registrationProgress = await GetRegistrationProgress();
        if (registrationProgress == null && !isAuthenticated)
        {
            var template = await TemplateStore.GetActive(prefix, cancellationToken);
            var res = new SidWorkflowViewModel
            {
                Template = template
            };
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        var viewModel = Activator.CreateInstance<TViewModel>();
        await UpdateViewModel(prefix, isAuthenticated, viewModel, cancellationToken);
        var result = await BuildViewModel(registrationProgress, prefix, isAuthenticated, viewModel, cancellationToken);
        viewModel.ReturnUrl = redirectUrl;
        result.SetInput(viewModel);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, TViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var isAuthenticated = User.Identity.IsAuthenticated;
        UserRegistrationProgress userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var template = await TemplateStore.GetActive(prefix, cancellationToken);
            var res = new SidWorkflowViewModel
            {
                Template = template,
            };
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        // 1. Check parameters.
        var result = await BuildViewModel(userRegistrationProgress, prefix, isAuthenticated, viewModel, cancellationToken);
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
                return await base.UpdateUser(result, userRegistrationProgress, viewModel, Amr, viewModel.ReturnUrl);
            }
        }

        async Task<IActionResult> RegisterUser()
        {
            return await base.CreateUser(result, userRegistrationProgress, viewModel, prefix, Amr, viewModel.ReturnUrl);
        }
    }

    protected abstract void Enrich(TViewModel viewModel, User user);

    protected async Task<WorkflowViewModel> BuildViewModel(UserRegistrationProgress registrationProcess, string prefix, bool isAuthenticated, TViewModel viewModel, CancellationToken cancellationToken)
    {
        var result = await BuildViewModel(registrationProcess, viewModel, prefix, cancellationToken);
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
        return section.Get<TOptions>() ?? Activator.CreateInstance<TOptions>();
    }

    private async Task UpdateViewModel(string prefix, bool isAuthenticated, TViewModel viewModel, CancellationToken cancellationToken)
    {
        if(!isAuthenticated)
        {
            return;
        }

        var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var authenticatedUser = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
        Enrich(viewModel, authenticatedUser);
    }
}
