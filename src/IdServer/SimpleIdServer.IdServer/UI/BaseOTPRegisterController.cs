// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
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

    public BaseOTPRegisterController(
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache, 
        IUserRepository userRepository, 
        IEnumerable<IOTPAuthenticator> otpAuthenticators, 
        IConfiguration configuration, 
        IUserNotificationService userNotificationService,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(options, distributedCache, userRepository, tokenRepository, jwtBuilder)
    {
        _userRepository = userRepository;
        _otpAuthenticators = otpAuthenticators;
        _configuration = configuration;
        _userNotificationService = userNotificationService;
    }

    protected abstract string Amr { get; }


    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.Prefix;
        var viewModel = Activator.CreateInstance<TViewModel>();
        var isAuthenticated = User.Identity.IsAuthenticated;
        var registrationProgress = await GetRegistrationProgress();
        if (registrationProgress == null && !isAuthenticated)
        {
            viewModel.IsNotAllowed = true;
            return View(viewModel);
        }

        if (isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.NameIdentifier = nameIdentifier;
            var authenticatedUser = await _userRepository.GetBySubject(nameIdentifier, prefix, CancellationToken.None);
            Enrich(viewModel, authenticatedUser);
        }

        viewModel.Amr = registrationProgress?.Amr;
        viewModel.Steps = registrationProgress?.Steps;
        viewModel.RedirectUrl = redirectUrl;
        return View(viewModel);
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
            viewModel.IsNotAllowed = true;
            return View(viewModel);
        }

        viewModel.Amr = userRegistrationProgress?.Amr;
        viewModel.Steps = userRegistrationProgress?.Steps;
        viewModel.Validate(ModelState);
        if (!ModelState.IsValid) return View(viewModel);
        var emailIsTaken = await IsUserExists(viewModel.Value, prefix);
        if (emailIsTaken)
        {
            ModelState.AddModelError("value_exists", "value_exists");
            return View(viewModel);
        }

        var options = GetOptions();
        var optAlg = options.OTPAlg;
        var otpAuthenticator = _otpAuthenticators.First(o => o.Alg == optAlg);
        if (viewModel.Action == "SENDCONFIRMATIONCODE")
        {
            return await SendConfirmationCode();
        }

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
            ModelState.AddModelError("invalid_confirmation_code", "invalid_confirmation_code");
            return View(viewModel);
        }

        var key = enteredOtpCode.ToString();
        var value = await DistributedCache.GetStringAsync(key);
        await DistributedCache.RemoveAsync(key);
        viewModel.Value = value;
        if (User.Identity.IsAuthenticated)
        {
            return await UpdateAuthenticatedUser();
        }

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
            await _userNotificationService.Send("One Time Password", string.Format(options.HttpBody, otpCode), new Dictionary<string, string>(), viewModel.Value);
            await DistributedCache.SetStringAsync(otpCode, viewModel.Value, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(2)
            });
            viewModel.IsOTPCodeSent = true;
            return View(viewModel);
        }

        async Task<IActionResult> UpdateAuthenticatedUser()
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var authenticatedUser = await _userRepository.GetBySubject(nameIdentifier, prefix, CancellationToken.None);
            BuildUser(authenticatedUser, viewModel);
            authenticatedUser.UpdateDateTime = DateTime.UtcNow;
            if (authenticatedUser.ActiveOTP == null)
                authenticatedUser.GenerateTOTP();

            await _userRepository.SaveChanges(CancellationToken.None);
            return await base.UpdateUser(userRegistrationProgress, viewModel, Amr, viewModel.RedirectUrl);
        }

        async Task<IActionResult> RegisterUser()
        {
            return await base.CreateUser(userRegistrationProgress, viewModel, prefix, Amr, viewModel.RedirectUrl);
        }
    }

    protected abstract void Enrich(TViewModel viewModel, User user);

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
