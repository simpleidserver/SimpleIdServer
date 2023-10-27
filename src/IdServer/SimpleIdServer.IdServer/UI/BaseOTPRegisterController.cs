// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public abstract class BaseOTPRegisterController<TOptions> : BaseRegisterController<OTPRegisterViewModel> where TOptions : IOTPRegisterOptions
{
    private readonly IUserRepository _userRepository;
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
    private readonly IConfiguration _configuration;
    private readonly IUserNotificationService _userNotificationService;

    public BaseOTPRegisterController(IOptions<IdServerHostOptions> options, IDistributedCache distributedCache, IUserRepository userRepository, IUserClaimsService userClaimsService, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, IUserNotificationService userNotificationService) : base(options, distributedCache, userRepository, userClaimsService)
    {
        _userRepository = userRepository;
        _otpAuthenticators = otpAuthenticators;
        _configuration = configuration;
        _userNotificationService = userNotificationService;
    }

    protected abstract string Amr { get; }


    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.Prefix;
        var viewModel = new OTPRegisterViewModel();
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
            var authenticatedUser = await _userRepository.Query().AsNoTracking()
                .Include(u => u.Realms).SingleAsync(u => u.Name == nameIdentifier && u.Realms.Any(r => r.RealmsName == prefix));
            var userClaims = await UserClaimsService.Get(authenticatedUser.Id, CancellationToken.None);
            Enrich(viewModel, authenticatedUser, userClaims);
        }

        viewModel.Amr = registrationProgress?.Amr;
        viewModel.Steps = registrationProgress?.Steps;
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, OTPRegisterViewModel viewModel)
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
        var options = GetOptions();
        var optAlg = options.OTPAlg;
        var otpAuthenticator = _otpAuthenticators.First(o => o.Alg == optAlg);
        if (viewModel.Action == "SENDCONFIRMATIONCODE")
        {
            return await SendConfirmationCode();
        }

        var enteredOtpCode = long.Parse(viewModel.OTPCode);
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
            await _userNotificationService.Send(string.Format(options.HttpBody, otpCode), viewModel.Value);
            viewModel.IsOTPCodeSent = true;
            return View(viewModel);
        }

        async Task<IActionResult> UpdateAuthenticatedUser()
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userExists = await IsUserExists(viewModel.Value, prefix);
            if (userExists)
            {
                ModelState.AddModelError("value_exists", "value_exists");
                return View(viewModel);
            }

            var authenticatedUser = await _userRepository.Query().Include(u => u.Realms).Include(c => c.Credentials).FirstAsync(u => u.Realms.Any(r => r.RealmsName == prefix) && u.Name == nameIdentifier);
            var userClaims = await UserClaimsService.Get(authenticatedUser.Id, CancellationToken.None);
            BuildUser(authenticatedUser, userClaims, viewModel);
            authenticatedUser.UpdateDateTime = DateTime.UtcNow;
            if (authenticatedUser.ActiveOTP == null)
                authenticatedUser.GenerateTOTP();

            await _userRepository.SaveChanges(CancellationToken.None);
            return await base.UpdateUser(userRegistrationProgress, viewModel, Amr);
        }

        async Task<IActionResult> RegisterUser()
        {
            var emailIsTaken = await IsUserExists(viewModel.Value, prefix);
            if (emailIsTaken)
            {
                ModelState.AddModelError("value_exists", "value_exists");
                return View(viewModel);
            }

            return await base.CreateUser(userRegistrationProgress, viewModel, prefix, Amr);
        }
    }

    protected abstract void Enrich(OTPRegisterViewModel viewModel, User user, ICollection<UserClaim> claims);

    protected abstract Task<bool> IsUserExists(string value, string prefix);

    protected abstract void BuildUser(User user, ICollection<UserClaim> claims, OTPRegisterViewModel viewModel);

    protected override void EnrichUser(User user, ICollection<UserClaim> claims, OTPRegisterViewModel viewModel)
    {
        BuildUser(user, claims, viewModel);
        if (user.ActiveOTP == null) user.GenerateTOTP();
    }

    private TOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(TOptions).Name);
        return section.Get<TOptions>();
    }
}
