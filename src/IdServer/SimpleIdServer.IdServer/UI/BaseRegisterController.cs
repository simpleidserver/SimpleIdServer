// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout;
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
        ITemplateStore templateStore) : base(tokenRepository, jwtBuilder)
    {
        Options = options.Value;
        FormOptions = formOptions.Value;
        DistributedCache = distributedCache;
        UserRepository = userRepository;
        TransactionBuilder = transactionBuilder;
        Antiforgery = antiforgery;
        FormStore = formStore;
        WorkflowStore = workflowStore;
        LanguageRepository = languageRepository;
        RealmStore = realmStore;
        TemplateStore = templateStore;
    }

    protected IdServerHostOptions Options { get; }
    protected FormBuilderOptions FormOptions { get; }
    protected IDistributedCache DistributedCache { get; }
    protected IUserRepository UserRepository { get; }
    protected ITransactionBuilder TransactionBuilder { get; }
    protected IAntiforgery Antiforgery { get; }
    protected IFormStore FormStore { get; }
    protected IWorkflowStore WorkflowStore { get; }
    protected ILanguageRepository LanguageRepository { get; }
    protected abstract string Amr { get; }
    private IRealmStore RealmStore { get; }
    protected ITemplateStore TemplateStore { get; }

    protected async Task<UserRegistrationProgress> GetRegistrationProgress()
    {
        var cookieName = Options.GetRegistrationCookieName(RealmStore.Realm);
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
                viewModel.ReturnUrl = registrationProgress.RedirectUrl ?? redirectUrl;
                result.SetInput(viewModel);
                return View(result);
            }
        }

        registrationProgress.User = user;
        var json = JsonConvert.SerializeObject(registrationProgress);
        await DistributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        return RedirectToAction("Index", "Register", new { area = nextAmr });
    }

    protected async Task<IActionResult> UpdateUser(WorkflowViewModel result, UserRegistrationProgress registrationProgress, TViewModel viewModel, string amr, string redirectUrl)
    {
        var nextAmr = GetNextAmr(result, viewModel);
        if (IsLastStep(nextAmr) || registrationProgress == null)
        {
            result.SetSuccessMessage(Global.UserIsUpdated);
            viewModel.IsUpdated = true;
            viewModel.ReturnUrl = registrationProgress?.RedirectUrl ?? redirectUrl;
            result.SetInput(viewModel);
            return View(result);
        }

        var json = JsonConvert.SerializeObject(registrationProgress);
        await DistributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        return RedirectToAction("Index", "Register", new { area = nextAmr });
    }

    protected async Task<WorkflowViewModel> BuildViewModel(UserRegistrationProgress registrationProgress, TViewModel viewModel, string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var tokenSet = Antiforgery.GetAndStoreTokens(HttpContext);
        var records = await FormStore.GetLatestPublishedVersionByCategory(prefix, FormCategories.Registration, cancellationToken);
        WorkflowRecord workflow = null;
        if(registrationProgress.UpdateOneCredential)
        {
            workflow = BuildNewUpdateCredentialWorkflow();
        }
        else
        {
            workflow = await WorkflowStore.Get(prefix, registrationProgress.WorkflowId, cancellationToken);
        }

        var template = await TemplateStore.GetActive(prefix, cancellationToken);
        var workflowFormIds = workflow.Steps.Select(s => s.FormRecordCorrelationId);
        var filteredRecords = records.Where(r => workflowFormIds.Contains(r.CorrelationId));
        var record = filteredRecords.Single(r => r.Name == Amr);
        var step = workflow.GetStep(record.CorrelationId);
        var languages = await LanguageRepository.GetAll(cancellationToken);
        var result = new SidWorkflowViewModel
        {
            CurrentStepId = step.Id,
            Workflow = workflow,
            FormRecords = records,
            Languages = languages,
            Template = template,
            AntiforgeryToken = new AntiforgeryTokenRecord
            {
                CookieName = FormOptions.AntiforgeryCookieName,
                CookieValue = tokenSet.CookieToken,
                FormField = tokenSet.FormFieldName,
                FormValue = tokenSet.RequestToken
            }
        };
        viewModel.Realm = Options.UseRealm ? prefix : null;
        viewModel.UpdateOneCredential = registrationProgress.UpdateOneCredential;
        return result;
    }

    protected abstract void EnrichUser(User user, TViewModel viewModel);

    protected abstract WorkflowRecord BuildNewUpdateCredentialWorkflow();

    private string GetNextAmr(WorkflowViewModel result, TViewModel viewModel)
    {
        var nextStepId = result.GetNextStepId(viewModel);
        var nextStep = result.Workflow.Steps.Single(r => r.Id == nextStepId);
        if (nextStep.IsEmptyStep)
        {
            return FormBuilder.Constants.EmptyStep.Name;
        }

        var formRecord = result.FormRecords.Single(rec => rec.CorrelationId == nextStep.FormRecordCorrelationId);
        return formRecord.Name;
    }

    private bool IsLastStep(string stepName)
        => stepName == FormBuilder.Constants.EmptyStep.Name;
}