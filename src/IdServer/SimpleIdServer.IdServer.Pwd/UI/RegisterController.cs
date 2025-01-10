// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Pwd;

[Area(Constants.Areas.Password)]
public class RegisterController : BaseRegisterController<PwdRegisterViewModel>
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
        IWorkflowStore workflowStore) : base(options, formOptions, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, antiforgery, formStore, workflowStore)
    {
    }

    protected override string Amr => Constants.Areas.Password;

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null)
    {
        prefix = prefix ?? Constants.Prefix;
        var isAuthenticated = User.Identity.IsAuthenticated;
        var viewModel = new PwdRegisterViewModel();
        viewModel.RedirectUrl = redirectUrl;
        var userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var res = new WorkflowViewModel();
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        if(isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        var result = await BuildViewModel(userRegistrationProgress, viewModel, prefix);
        result.SetInput(viewModel);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, PwdRegisterViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.Prefix;
        var isAuthenticated = User.Identity.IsAuthenticated;
        var userRegistrationProgress = await GetRegistrationProgress();
        // 1. When the user is not authenticated then a registration process must exists.
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var res = new WorkflowViewModel();
            res.SetErrorMessage(Resources.Global.NotAllowedToRegister);
            return View(res);
        }

        if (isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        // 2. Build the view model.
        var result = await BuildViewModel(userRegistrationProgress, viewModel, prefix);

        // 3. Check the incoming request is valid.
        var errors = viewModel.Validate();
        if (errors.Any())
        {
            result.SetInput(viewModel);
            result.ErrorMessages = errors;
            return View(result);
        }

        if (!isAuthenticated) return await CreateUser();
        return await UpdateUser();

        async Task<IActionResult> CreateUser()
        {
            // 4. Check a user already exists.
            var isUserExists = await UserRepository.IsSubjectExists(viewModel.Login, prefix, cancellationToken);
            if(isUserExists)
            {
                result.SetInput(viewModel);
                result.SetErrorMessage(Global.UserWithSameLoginAlreadyExists);
                return View(result);
            }

            return await base.CreateUser(result, userRegistrationProgress, viewModel, prefix, Constants.Areas.Password, viewModel.RedirectUrl);
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
                return await base.UpdateUser(result, userRegistrationProgress, viewModel, Constants.Areas.Password, viewModel.RedirectUrl);
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