// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.UIs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public abstract class BaseRegisterController<TViewModel> : BaseController where TViewModel : IRegisterViewModel
{
    public BaseRegisterController(
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache,
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        Options = options.Value;
        DistributedCache = distributedCache;
        UserRepository = userRepository;
        TransactionBuilder = transactionBuilder;
    }

    protected IdServerHostOptions Options { get; }
    protected IDistributedCache DistributedCache { get; }
    protected IUserRepository UserRepository { get; }
    protected ITransactionBuilder TransactionBuilder { get; }

    protected async Task<UserRegistrationProgress> GetRegistrationProgress()
    {
        var cookieName = Options.GetRegistrationCookieName();
        if (!Request.Cookies.ContainsKey(cookieName)) return null;
        var cookieValue = Request.Cookies[cookieName];
        var json = await DistributedCache.GetStringAsync(cookieValue);
        if (string.IsNullOrWhiteSpace(json)) return null;
        var registrationProgress = JsonConvert.DeserializeObject<UserRegistrationProgress>(json);
        return registrationProgress;
    }

    protected async Task<IActionResult> CreateUser(WorkflowViewModel result, UserRegistrationProgress registrationProgress, TViewModel viewModel, string prefix, string amr, string redirectUrl)
    {
        var user = registrationProgress.User ?? new Domains.User
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        EnrichUser(user, viewModel);
        var nextAmr = GetNextAmr(result, viewModel);
        if (IsLastStep(nextAmr))
        {
            using (var transaction = TransactionBuilder.Build())
            {
                user.Realms.Add(new Domains.RealmUser
                {
                    RealmsName = prefix
                });
                UserRepository.Add(user);
                await transaction.Commit(CancellationToken.None);
                result.SetSuccessMessage(Global.UserIsCreated);
                viewModel.IsCreated = true;
                viewModel.RedirectUrl = registrationProgress.RedirectUrl ?? redirectUrl;
                result.SetInput(viewModel);
                return View(result);
            }
        }

        registrationProgress.User = user;
        var json = JsonConvert.SerializeObject(registrationProgress);
        await DistributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        return RedirectToAction("Index", "Register", new { area = registrationProgress.Amr });
    }

    protected async Task<IActionResult> UpdateUser(WorkflowViewModel result, UserRegistrationProgress registrationProgress, TViewModel viewModel, string amr, string redirectUrl)
    {
        var nextAmr = GetNextAmr(result, viewModel);
        if (IsLastStep(nextAmr) || registrationProgress == null)
        {
            result.SetSuccessMessage(Global.UserIsUpdated);
            viewModel.IsUpdated = true;
            viewModel.RedirectUrl = registrationProgress?.RedirectUrl ?? redirectUrl;
            result.SetInput(viewModel);
            return View(result);
        }

        var json = JsonConvert.SerializeObject(registrationProgress);
        await DistributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        return RedirectToAction("Index", "Register", new { area = nextAmr });
    }

    protected abstract void EnrichUser(User user, TViewModel viewModel);

    private string GetNextAmr(WorkflowViewModel result, TViewModel viewModel)
    {
        var nextStepId = result.GetNextStepId(viewModel);
        var formRecord = result.FormRecords.Single(rec => rec.Id == result.Workflow.Steps.Single(r => r.Id == nextStepId).FormRecordId);
        return formRecord.Name;
    }

    private bool IsLastStep(string stepName)
        => stepName == FormBuilder.Constants.EmptyStep.Name;
}