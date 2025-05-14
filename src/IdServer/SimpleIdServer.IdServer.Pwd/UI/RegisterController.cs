// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
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
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Pwd;

[Area(Constants.AreaPwd)]
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
        IWorkflowStore workflowStore,
        ILanguageRepository languageRepository,
        IRealmStore realmStore,
        ITemplateStore templateStore) : base(options, formOptions, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, antiforgery, formStore, workflowStore, languageRepository, realmStore, templateStore)
    {
    }

    protected override string Amr => Constants.AreaPwd;

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string? redirectUrl = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var isAuthenticated = User.Identity.IsAuthenticated;
        var viewModel = new PwdRegisterViewModel();
        viewModel.ReturnUrl = redirectUrl;
        var userRegistrationProgress = await GetRegistrationProgress();
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var res = new SidWorkflowViewModel();
            res.SetErrorMessage(Global.NotAllowedToRegister);
            return View(res);
        }

        if(isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        var result = await BuildViewModel(userRegistrationProgress, viewModel, prefix, cancellationToken);
        result.SetInput(viewModel);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, PwdRegisterViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var isAuthenticated = User.Identity.IsAuthenticated;
        var userRegistrationProgress = await GetRegistrationProgress();
        // 1. When the user is not authenticated then a registration process must exists.
        if (userRegistrationProgress == null && !isAuthenticated)
        {
            var res = new SidWorkflowViewModel();
            res.SetErrorMessage(Resources.Global.NotAllowedToRegister);
            return View(res);
        }

        if (isAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        // 2. Build the view model.
        var result = await BuildViewModel(userRegistrationProgress, viewModel, prefix, cancellationToken);

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

            return await base.CreateUser(result, userRegistrationProgress, viewModel, prefix, Constants.AreaPwd, viewModel.ReturnUrl);
        }

        async Task<IActionResult> UpdateUser()
        {
            using (var transaction = TransactionBuilder.Build())
            {
                var user = await UserRepository.GetBySubject(viewModel.Login, prefix, cancellationToken);
                var passwordCredential = user.Credentials.FirstOrDefault(c => c.CredentialType == UserCredential.PWD);
                if (passwordCredential != null) passwordCredential.Value = PasswordHelper.ComputerHash(passwordCredential, viewModel.Password);
                else
                {
                    var credential = new UserCredential
                    {
                        Id = Guid.NewGuid().ToString(),
                        CredentialType = UserCredential.PWD,
                        HashAlg = Options.PwdHashAlg,
                        IsActive = true
                    };
                    credential.Value = PasswordHelper.ComputerHash(passwordCredential, viewModel.Password);
                    user.Credentials.Add(credential);
                }
                UserRepository.Update(user);
                await transaction.Commit(cancellationToken);
                return await base.UpdateUser(result, userRegistrationProgress, viewModel, Constants.AreaPwd, viewModel.ReturnUrl);
            }
        }
    }

    protected override void EnrichUser(User user, PwdRegisterViewModel viewModel)
    {
        var credential = new UserCredential
        {
            Id = Guid.NewGuid().ToString(),
            CredentialType = UserCredential.PWD,
            HashAlg = Options.PwdHashAlg,
            IsActive = true
        };
        credential.Value = PasswordHelper.ComputerHash(credential, viewModel.Password);
        user.Credentials.Add(credential);
        user.Name = viewModel.Login;
        if (Options.IsEmailUsedDuringAuthentication) user.Email = viewModel.Login;
    }

    protected override WorkflowRecord BuildNewUpdateCredentialWorkflow()
    {
        return StandardPwdRegistrationWorkflows.DefaultWorkflow;
    }
}