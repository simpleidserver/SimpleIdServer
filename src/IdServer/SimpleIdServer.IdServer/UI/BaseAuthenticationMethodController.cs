// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Captcha;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.AuthProviders;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public abstract class BaseAuthenticationMethodController<T> : BaseAuthenticateController where T : BaseAuthenticateViewModel
    {
        private readonly ITemplateStore _templateStore;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IUserAuthenticationService _authenticationService;
        private readonly IAntiforgery _antiforgery;
        private readonly ISessionManager _sessionManager;
        private readonly ILanguageRepository _languageRepository;
        private readonly ICaptchaValidatorFactory _captchaValidatorFactory;
        private readonly FormBuilderOptions _formBuilderOptions;

        public BaseAuthenticationMethodController(
            ITemplateStore templateStore,
            IConfiguration configuration,
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IUserAuthenticationService userAuthenticationService,
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            ITransactionBuilder transactionBuilder,
            IJwtBuilder jwtBuilder,
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer,
            IBusControl busControl,
            IAntiforgery antiforgery,
            IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
            ISessionManager sessionManager,
            IWorkflowStore workflowStore,
            IFormStore formStore,
            ILanguageRepository languageRepository,
            IAcrHelper acrHelper,
            IWorkflowHelper workflowHelper,
            ICaptchaValidatorFactory captchaValidatorFactory,
            IOptions<FormBuilderOptions> formBuilderOptions) : base(clientRepository, userRepository, userSessionRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, authenticationHelper, transactionBuilder, tokenRepository, jwtBuilder, workflowStore, formStore, acrHelper, authenticationContextClassReferenceRepository, workflowHelper, options)
        {
            _templateStore = templateStore;
            _configuration = configuration;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _authenticationService = userAuthenticationService;
            _antiforgery = antiforgery;
            _sessionManager = sessionManager;
            _languageRepository = languageRepository;
            _captchaValidatorFactory = captchaValidatorFactory;
            _formBuilderOptions = formBuilderOptions.Value;
        }

        protected abstract bool IsExternalIdProvidersDisplayed { get; }
        protected IUserAuthenticationService UserAuthenticationService => _authenticationService;
        protected abstract string Amr { get; }

        #region Get Authenticate View

        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                List<FormRecord> forms;
                var acrInfo = await AcrHelper.GetAcr(cancellationToken);
                string loginHint = null;
                var viewModel = await BuildViewModel(acrInfo);
                WorkflowRecord workflow = null;
                if (IsProtected(returnUrl))
                {
                    var acrResult = await ParseRedirectUrl(viewModel, acrInfo);
                    workflow = acrResult.Workflow;
                    forms = acrResult.Forms;
                }
                else
                {
                    viewModel.RememberLogin = false;
                    var acr = await AcrRepository.GetByName(prefix, Options.DefaultAcrValue, cancellationToken);
                    workflow = await WorkflowStore.Get(prefix, acr.AuthenticationWorkflow, cancellationToken);
                    forms = await FormStore.GetLatestPublishedVersionByCategory(prefix, FormCategories.Authentication, cancellationToken);
                }

                if (IsFirstStep(workflow, forms))
                    acrInfo = null;

                if (acrInfo != null && !string.IsNullOrWhiteSpace(acrInfo.Login) && TryGetLogin(acrInfo, out string login))
                    viewModel.Login = login;
                viewModel.IsAuthInProgress = acrInfo?.Login != null && !string.IsNullOrWhiteSpace(acrInfo?.Login);
                var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
                var result = await this.BuildViewModel(prefix, workflow, forms, cancellationToken);
                result.SetInput(viewModel);
                if(IsMaximumActiveSessionReached()) result.SetErrorMessage(AuthFormErrorMessages.MaximumNumberActiveSessions);
                if(acrInfo != null && acrInfo.UserId != null && string.IsNullOrWhiteSpace(viewModel.Login)) result.SetErrorMessage(AuthFormErrorMessages.MissingLogin);
                return View(result);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }

            async Task<T> BuildViewModel(AcrAuthInfo acrInfo)
            {
                var viewModel = Activator.CreateInstance<T>();
                var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
                var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
                viewModel.ReturnUrl = returnUrl;
                viewModel.AuthUrl = UriHelper.GetDisplayUrl(Request);
                viewModel.Realm = Options.RealmEnabled ? prefix : string.Empty;
                viewModel.ExternalIdsProviders = externalIdProviders.Select(e => new ExternalIdProvider
                {
                    AuthenticationScheme = e.Name,
                    DisplayName = e.DisplayName
                }).ToList();
                EnrichViewModel(viewModel);
                return viewModel;
            }

            async Task<AcrResult> ParseRedirectUrl(T viewModel, AcrAuthInfo acrInfo)
            {
                var query = ExtractQuery(returnUrl);
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var client = await ClientRepository.GetByClientId(prefix, clientId, cancellationToken);
                var loginHint = query.GetLoginHintFromAuthorizationRequest();
                var acr = await ResolveAcr(acrInfo, query, prefix, client, cancellationToken);
                viewModel.Login = loginHint;
                viewModel.ClientName = client.ClientName;
                viewModel.LogoUri = client.LogoUri;
                viewModel.TosUri = client.TosUri;
                viewModel.PolicyUri = client.PolicyUri;
                return acr;
            }
        }

        #endregion

        #region Submit Credentials

        [HttpPost]
        public async virtual Task<IActionResult> Index([FromRoute] string prefix, T viewModel, CancellationToken token)
        {
            // 1. Validate antiforgery.
            prefix = prefix ?? Constants.DefaultRealm;
            if (viewModel == null) return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            var checkResult = await CheckAntiforgery();
            if (checkResult != null) return checkResult;
            // 2. Build view model
            viewModel.Realm = Options.RealmEnabled ? prefix : string.Empty;
            EnrichViewModel(viewModel);
            await UpdateViewModel(viewModel);

            // 3. Build workflow result
            var workflowResult = await BuildWorkflowViewModel();
            AcrAuthInfo acrInfo = null;
            if (!IsFirstStep(workflowResult.Workflow, workflowResult.FormRecords))
                acrInfo = await AcrHelper.GetAcr(token);
            viewModel.IsAuthInProgress = acrInfo?.Login != null && !string.IsNullOrWhiteSpace(acrInfo?.Login);
            workflowResult.SetInput(viewModel);

            // 4. Validate view model.
            var errors = viewModel.Validate();
            if (errors.Any())
            {
                workflowResult.SetErrorMessages(errors);
                return View(workflowResult);
            }

            if (acrInfo != null && !string.IsNullOrWhiteSpace(acrInfo.UserId) && !TryGetLogin(acrInfo, out string login))
            {
                viewModel.Login = null;
                workflowResult.SetErrorMessage(AuthFormErrorMessages.MissingLogin);
                return View(workflowResult);
            }

            if (IsMaximumActiveSessionReached())
            {
                workflowResult.SetErrorMessage(AuthFormErrorMessages.MaximumNumberActiveSessions);
                return View(workflowResult);
            }

            // 5. Validate Captcha.
            if(!await _captchaValidatorFactory.Validate(viewModel, token))
            {
                workflowResult.SetErrorMessage(AuthFormErrorMessages.InvalidCaptcha);
                return View(workflowResult);
            }

            // 6. Try to authenticate the user.
            var result = await CustomAuthenticate(prefix, acrInfo?.UserId, viewModel, token);
            if (result.Errors != null && result.Errors.Any())
            {
                workflowResult.SetErrorMessages(result.Errors);
                return View(workflowResult);
            }

            if (result.SuccessMessages != null && result.SuccessMessages.Any())
            {
                workflowResult.SetSuccessMessages(result.SuccessMessages);
                return View(workflowResult);
            }

            // 7. Validate the credentials.
            CredentialsValidationResult authenticationResult = null;
            if (result.AuthenticatedUser != null) authenticationResult = await _authenticationService.Validate(prefix, result.AuthenticatedUser, viewModel, token);
            else authenticationResult = await _authenticationService.Validate(prefix, acrInfo?.UserId, viewModel, token);
            if (authenticationResult.Status != ValidationStatus.AUTHENTICATE)
            {
                switch (authenticationResult.Status)
                {
                    case ValidationStatus.UNKNOWN_USER:
                        workflowResult.SetErrorMessage(AuthFormErrorMessages.UserDoesntExist);
                        await Bus.Publish(new UserLoginFailureEvent
                        {
                            Realm = prefix,
                            Amr = Amr,
                            Login = viewModel.Login
                        });
                        Counters.AuthFailure(GetClientId(viewModel.ReturnUrl), prefix);
                        return View(workflowResult);
                    case ValidationStatus.NOCONTENT:
                        if (!string.IsNullOrWhiteSpace(authenticationResult.ErrorCode) && !string.IsNullOrWhiteSpace(authenticationResult.ErrorMessage))
                            workflowResult.SetErrorMessage(authenticationResult.ErrorMessage);
                        return View(workflowResult);
                    case ValidationStatus.INVALIDCREDENTIALS:
                        using (var transaction = TransactionBuilder.Build())
                        {
                            var options = GetOptions();
                            workflowResult.SetErrorMessage(AuthFormErrorMessages.InvalidCredential);
                            await Bus.Publish(new UserLoginFailureEvent
                            {
                                Realm = prefix,
                                Amr = Amr,
                                Login = viewModel.Login
                            });
                            Counters.AuthFailure(GetClientId(viewModel.ReturnUrl), prefix);
                            if (authenticationResult.AuthenticatedUser != null)
                            {
                                authenticationResult.AuthenticatedUser.LoginAttempt(options.MaxLoginAttempts, options.LockTimeInSeconds);
                                UserRepository.Update(authenticationResult.AuthenticatedUser);
                            }

                            await transaction.Commit(token);
                            return View(workflowResult);
                        }
                }
            }

            authenticationResult.AuthenticatedUser.ResetLoginAttempt();
            return await Authenticate(prefix,
               viewModel,
               Amr,
               authenticationResult.AuthenticatedUser,
               token,
               acrInfo?.RememberLogin ?? viewModel.RememberLogin,
               authenticationResult.IsTemporaryCredential);

            async Task<IActionResult> CheckAntiforgery()
            {
                try
                {
                    await _antiforgery.ValidateRequestAsync(this.HttpContext);
                    return null;
                }
                catch (AntiforgeryValidationException)
                {
                    if (!User.Identity.IsAuthenticated) return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = Global.InvalidAntiForgeryToken });
                    var returnUrl = viewModel.ReturnUrl;
                    if (!IsProtected(returnUrl)) return Redirect(returnUrl);
                    var unprotectedUrl = Unprotect(returnUrl);
                    return Redirect(unprotectedUrl);
                }
            }

            async Task<WorkflowViewModel> BuildWorkflowViewModel()
            {
                var records = await FormStore.GetLatestPublishedVersionByCategory(prefix, FormCategories.Authentication, token);
                var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
                var workflow = await WorkflowStore.Get(prefix, viewModel.WorkflowId, token);
                var workflowResult = await BuildViewModel(prefix, workflow, records, token);
                return workflowResult;
            }
        }

        protected abstract Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, T viewModel, CancellationToken cancellationToken);

        protected abstract void EnrichViewModel(T viewModel);

        protected async Task UpdateViewModel(T viewModel)
        {
            IEnumerable<AuthenticationScheme> externalIdProviders = new List<AuthenticationScheme>();
            if (IsExternalIdProvidersDisplayed)
            {
                var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
                externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
            }

            viewModel.ExternalIdsProviders = externalIdProviders.Select(e => new ExternalIdProvider
            {
                AuthenticationScheme = e.Name,
                DisplayName = e.DisplayName
            }).ToList();
        }

        #endregion

        private bool IsFirstStep(WorkflowRecord workflow, List<FormRecord> forms)
        {
            var firstStep = workflow.GetFirstStep();
            return forms.Single(f => f.CorrelationId == firstStep.FormRecordCorrelationId).Name == Amr;
        }

        private async Task<WorkflowViewModel> BuildViewModel(string realm, WorkflowRecord workflow, List<FormRecord> records, CancellationToken cancellationToken)
        {
            realm = realm ?? Constants.DefaultRealm;
            var template = await _templateStore.GetActive(realm, cancellationToken);
            var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
            var workflowStepFormIds = workflow.Steps.Select(s => s.FormRecordCorrelationId);
            var filteredRecords = records.Where(r => workflowStepFormIds.Contains(r.CorrelationId)).ToList();
            var record = filteredRecords.Single(r => r.Name == Amr);
            var step = workflow.GetStep(record.CorrelationId);
            var languages = await _languageRepository.GetAll(cancellationToken);
            var result = new SidWorkflowViewModel
            {
                CurrentStepId = step.Id,
                Workflow = workflow,
                FormRecords = filteredRecords,
                Languages = languages,
                Template = template,
                AntiforgeryToken = new AntiforgeryTokenRecord
                {
                    CookieName = _formBuilderOptions.AntiforgeryCookieName,
                    CookieValue = tokenSet.CookieToken,
                    FormField = tokenSet.FormFieldName,
                    FormValue = tokenSet.RequestToken
                }
            };
            return result;
        }

        private bool IsMaximumActiveSessionReached()
        {
            var allTickets = _sessionManager.FetchTicketsFromAllRealm(HttpContext);
            return (allTickets.Count() >= Options.MaxNbActiveSessions);
        }

        protected abstract bool TryGetLogin(AcrAuthInfo amrInfo, out string login);

        protected async Task<AcrResult> ResolveAcr(AcrAuthInfo acrInfo, JsonObject query, string realm, Client client, CancellationToken cancellationToken)
        {
            if (acrInfo != null) return await AmrHelper.Get(realm, FormCategories.Authentication, new List<string> { acrInfo.CurrentAcr }, cancellationToken);
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var acr = await AmrHelper.FetchDefaultAcr(realm, FormCategories.Authentication, acrValues, requestedClaims, client, cancellationToken);
            return acr;
        }

        protected UserLockingOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(UserLockingOptions).Name);
            return section.Get<UserLockingOptions>() ?? new UserLockingOptions();
        }
    }

    public record UserAuthenticationResult
    {
        private UserAuthenticationResult()
        {
            
        }

        private UserAuthenticationResult(List<string> errors)
        {
            Errors = errors;
        }

        private UserAuthenticationResult(User authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
        }

        public List<string> Errors { get; private set; }
        public List<string> SuccessMessages { get; private set; }
        public User AuthenticatedUser { get; private set; }

        public static UserAuthenticationResult Ok(User authenticatedUser = null) => new UserAuthenticationResult(authenticatedUser);
        public static UserAuthenticationResult Success(List<string> successMessages) => new UserAuthenticationResult { SuccessMessages = successMessages };
        public static UserAuthenticationResult Success(string successMessage) => Success(new List<string> { successMessage });
        public static UserAuthenticationResult Error(List<string> errors) => new UserAuthenticationResult(errors);
        public static UserAuthenticationResult Error(string error) => Error(new List<string> { error });
    }
}
