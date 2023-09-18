// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public abstract class BaseOTPRegisterController<TOptions> : BaseController where TOptions : IOTPRegisterOptions
{
    private readonly IUserRepository _userRepository;
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
    private readonly IConfiguration _configuration;
    private readonly IUserNotificationService _userNotificationService;

    public BaseOTPRegisterController(IUserRepository userRepository, IEnumerable<IOTPAuthenticator> otpAuthenticators, IConfiguration configuration, IUserNotificationService userNotificationService)
    {
        _userRepository = userRepository;
        _otpAuthenticators = otpAuthenticators;
        _configuration = configuration;
        _userNotificationService = userNotificationService;
    }

    protected IUserRepository UserRepository => _userRepository;


    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.Prefix;
        var viewModel = new OTPRegisterViewModel();
        if (User.Identity.IsAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.NameIdentifier = nameIdentifier;
            var authenticatedUser = await _userRepository.Query().AsNoTracking().Include(u => u.OAuthUserClaims).Include(u => u.Realms).SingleAsync(u => u.Name == nameIdentifier && u.Realms.Any(r => r.RealmsName == prefix));
            Enrich(viewModel, authenticatedUser);
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromRoute] string prefix, OTPRegisterViewModel viewModel)
    {
        prefix = prefix ?? Constants.Prefix;
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
                ModelState.AddModelError("user_exists", "user_exists");
                return View(viewModel);
            }

            var authenticatedUser = await _userRepository.Query().Include(u => u.Realms).Include(c => c.OAuthUserClaims).FirstAsync(u => u.Realms.Any(r => r.RealmsName == prefix) && u.Name == nameIdentifier);
            BuildUser(authenticatedUser, viewModel);
            authenticatedUser.UpdateDateTime = DateTime.UtcNow;
            if (authenticatedUser.ActiveOTP == null)
                authenticatedUser.GenerateTOTP();

            await _userRepository.SaveChanges(CancellationToken.None);
            viewModel.IsUpdated = true;
            return View(viewModel);
        }

        async Task<IActionResult> RegisterUser()
        {
            var emailIsTaken = await IsUserExists(viewModel.Value, prefix);
            if (emailIsTaken)
            {
                ModelState.AddModelError("user_exists", "user_exists");
                return View(viewModel);
            }

            var user = UserBuilder.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), realm: null)
                .GenerateRandomTOTPKey()
                .Build();
            BuildUser(user, viewModel);
            _userRepository.Add(user);
            await _userRepository.SaveChanges(CancellationToken.None);
            viewModel.IsRegistered = true;
            return View(viewModel);
        }
    }

    protected abstract void Enrich(OTPRegisterViewModel viewModel, User user);

    protected abstract Task<bool> IsUserExists(string value, string prefix);

    protected abstract void BuildUser(User user, OTPRegisterViewModel viewModel);

    private TOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(TOptions).Name);
        return section.Get<TOptions>();
    }
}
