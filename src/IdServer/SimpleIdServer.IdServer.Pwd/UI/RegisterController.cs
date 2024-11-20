﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Pwd;

[Area(Constants.Areas.Password)]
public class RegisterController : BaseRegisterController<PwdRegisterViewModel>
{
    public RegisterController(
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache, 
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder) : base(options, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null)
    {
        prefix = prefix ?? Constants.Prefix;
        var viewModel = new PwdRegisterViewModel();
        var isAuthenticated = User.Identity.IsAuthenticated;
        viewModel.RedirectUrl = redirectUrl;
        var userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            viewModel.IsNotAllowed = true;
            return View(viewModel);
        }

        if(isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        viewModel.Amr = userRegistrationProgress?.Amr;
        viewModel.Steps = userRegistrationProgress?.Steps;
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, PwdRegisterViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.Prefix;
        var isAuthenticated = User.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        var userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            viewModel.IsNotAllowed = true;
            return View(viewModel);
        }

        viewModel.Amr = userRegistrationProgress?.Amr;
        viewModel.Steps = userRegistrationProgress?.Steps;
        viewModel.Validate(ModelState);
        if (!ModelState.IsValid) return View(viewModel);
        if (!isAuthenticated) return await CreateUser();
        return await UpdateUser();

        async Task<IActionResult> CreateUser()
        {
            var isUserExists = await UserRepository.IsSubjectExists(viewModel.Login, prefix, cancellationToken);
            if(isUserExists)
            {
                ModelState.AddModelError("user_exists", "user_exists");
                return View(viewModel);
            }

            return await base.CreateUser(userRegistrationProgress, viewModel, prefix, Constants.Areas.Password, viewModel.RedirectUrl);
        }

        async Task<IActionResult> UpdateUser()
        {
            using (var transaction = TransactionBuilder.Build())
            {
                var user = await UserRepository.GetBySubject(viewModel.Login, prefix, cancellationToken);
                var passwordCredential = user.Credentials.FirstOrDefault(c => c.CredentialType == UserCredential.PWD);
                if (passwordCredential != null) passwordCredential.Value = PasswordHelper.ComputeHash(viewModel.Password, Options.IsPasswordEncodeInBase64);
                else user.Credentials.Add(new UserCredential
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = PasswordHelper.ComputeHash(viewModel.Password, Options.IsPasswordEncodeInBase64),
                    CredentialType = UserCredential.PWD,
                    IsActive = true
                });
                UserRepository.Update(user);
                await transaction.Commit(cancellationToken);
                return await base.UpdateUser(userRegistrationProgress, viewModel, Constants.Areas.Password, viewModel.RedirectUrl);
            }
        }
    }

    protected override void EnrichUser(User user, PwdRegisterViewModel viewModel)
    {
        user.Credentials.Add(new UserCredential
        {
            Id = Guid.NewGuid().ToString(),
            CredentialType = "pwd",
            IsActive = true,
            Value = PasswordHelper.ComputeHash(viewModel.Password, Options.IsPasswordEncodeInBase64)
        });
        user.Name = viewModel.Login;
        if (Options.IsEmailUsedDuringAuthentication) user.Email = viewModel.Login;
    }
}