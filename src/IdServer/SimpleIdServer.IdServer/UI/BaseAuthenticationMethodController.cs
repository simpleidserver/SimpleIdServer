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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Jwt;
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
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public abstract class BaseAuthenticationMethodController<T> : BaseAuthenticateController where T : BaseAuthenticateViewModel
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IUserAuthenticationService _authenticationService;
        private readonly IAntiforgery _antiforgery;
        private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
        private readonly ISessionManager _sessionManager;
        private readonly IWorkflowStore _workflowStore;
        private readonly IFormStore _formStore;
        private readonly FormBuilderOptions _formBuilderOptions;

        public BaseAuthenticationMethodController(
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
            IOptions<FormBuilderOptions> formBuilderOptions) : base(clientRepository, userRepository, userSessionRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, authenticationHelper, transactionBuilder, tokenRepository, jwtBuilder, options)
        {
            _configuration = configuration;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _authenticationService = userAuthenticationService;
            _antiforgery = antiforgery;
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _sessionManager = sessionManager;
            _workflowStore = workflowStore;
            _formStore = formStore;
            _formBuilderOptions = formBuilderOptions.Value;
        }

        protected abstract string Amr { get; }
        protected abstract bool IsExternalIdProvidersDisplayed { get; }
        protected IUserAuthenticationService UserAuthenticationService => _authenticationService;

        #region Get Authenticate View

        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            try
            {
                var records = await _formStore.GetAll(cancellationToken);
                var viewModel = await BuildViewModel();
                WorkflowRecord workflow = null;
                if (IsProtected(returnUrl))
                {
                    var acr = await ParseRedirectUrl(viewModel);
                    workflow = await _workflowStore.Get(acr.AuthenticationWorkflow, cancellationToken);
                }
                else
                {
                    viewModel.IsFirstAmr = true;
                    viewModel.RememberLogin = false;
                    workflow = await _workflowStore.Get(Options.DefaultAuthenticationWorkflowId, cancellationToken);
                }

                var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
                var step = workflow.GetStep(Amr);
                var result = new WorkflowViewModel
                {
                    CurrentStepId = step.Id,
                    Workflow = workflow,
                    FormRecords = records,
                    AntiforgeryToken = new AntiforgeryTokenRecord
                    {
                        CookieName=  _formBuilderOptions.AntiforgeryCookieName,
                        CookieValue = tokenSet.CookieToken,
                        FormField = tokenSet.FormFieldName,
                        FormValue = tokenSet.RequestToken
                    }
                };
                result.SetInput(viewModel);
                if(IsMaximumActiveSessionReached()) result.SetErrorMessage(Global.MaximumNumberActiveSessions);
                return View(result);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }

            async Task<T> BuildViewModel()
            {
                var viewModel = Activator.CreateInstance<T>();
                var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
                var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
                viewModel.ReturnUrl = returnUrl;
                viewModel.Realm = prefix;
                viewModel.IsFirstAmr = false;
                viewModel.ExternalIdsProviders = externalIdProviders.Select(e => new ExternalIdProvider
                {
                    AuthenticationScheme = e.Name,
                    DisplayName = e.DisplayName
                }).ToList();
                EnrichViewModel(viewModel);
                return viewModel;
            }

            async Task<AuthenticationContextClassReference> ParseRedirectUrl(T viewModel)
            {
                var query = ExtractQuery(returnUrl);
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var str = prefix ?? Constants.DefaultRealm;
                var client = await ClientRepository.GetByClientId(str, clientId, cancellationToken);
                var loginHint = query.GetLoginHintFromAuthorizationRequest();
                var amrInfo = await ResolveAmrInfo(query, str, client, cancellationToken);
                bool isLoginMissing = amrInfo != null && !string.IsNullOrWhiteSpace(amrInfo.Value.Item1.Login);
                if (amrInfo != null && !string.IsNullOrWhiteSpace(amrInfo.Value.Item1.Login) && TryGetLogin(amrInfo.Value.Item1, out string login))
                {
                    loginHint = login;
                    isLoginMissing = false;
                }

                if (amrInfo != null && amrInfo.Value.Item1.CurrentAmr == amrInfo.Value.Item1.AllAmr.First())
                {
                    viewModel.IsFirstAmr = true;
                    viewModel.RememberLogin = false;
                    viewModel.RegistrationWorkflow = amrInfo.Value.Item2.RegistrationWorkflow;
                }

                viewModel.ClientName = client.ClientName;
                viewModel.Login = loginHint;
                viewModel.LogoUri = client.LogoUri;
                viewModel.TosUri = client.TosUri;
                viewModel.PolicyUri = client.PolicyUri;
                viewModel.IsLoginMissing = isLoginMissing;
                viewModel.IsAuthInProgress = amrInfo != null && !string.IsNullOrWhiteSpace(amrInfo.Value.Item1.Login);
                viewModel.AmrAuthInfo = amrInfo.Value.Item1;
                return amrInfo.Value.Item2;
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
            viewModel.Realm = prefix;
            EnrichViewModel(viewModel);
            await UpdateViewModel(viewModel);
            await ExtractRegistrationWorkflow();

            // 3. Build workflow result
            var workflowResult = await BuildWorkflowViewModel();
            workflowResult.SetInput(viewModel);
            var amrInfo = GetAmrInfo();
            /*
            if (IsProtected(viewModel.ReturnUrl))
            {
                var query = ExtractQuery(viewModel.ReturnUrl);
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var client = await ClientRepository.GetByClientId(prefix, clientId, token);
                var res = await ResolveAmrInfo(query, prefix, client, token);
                amrInfo = res?.Item1;
                viewModel.AmrAuthInfo = amrInfo;
                if (res != null && res.Value.Item1.CurrentAmr == res.Value.Item1.AllAmr.First())
                {
                    viewModel.IsFirstAmr = true;
                    viewModel.RememberLogin = false;
                    viewModel.RegistrationWorkflow = res.Value.Item2.RegistrationWorkflow;
                }
            }
            */

            // 4. Validate view model.
            var errors = viewModel.Validate();
            if (errors.Any())
            {
                workflowResult.ErrorMessages = errors;
                return View(workflowResult);
            }

            if (IsMaximumActiveSessionReached())
            {
                workflowResult.SetErrorMessage(Global.MaximumNumberActiveSessions);
                return View(workflowResult);
            }

            // 5. Try to authenticate the user.
            var result = await CustomAuthenticate(prefix, amrInfo?.UserId, viewModel, token);
            if (result.Errors != null && result.Errors.Any())
            {
                workflowResult.ErrorMessages = result.Errors;
                return View(workflowResult);
            }

            if (result.SuccessMessages != null && result.SuccessMessages.Any())
            {
                workflowResult.SuccessMessages = result.SuccessMessages;
                return View(workflowResult);
            }

            // 6. Validate the credentials.
            CredentialsValidationResult authenticationResult = null;
            if (result.AuthenticatedUser != null) authenticationResult = await _authenticationService.Validate(prefix, result.AuthenticatedUser, viewModel, token);
            else authenticationResult = await _authenticationService.Validate(prefix, amrInfo?.UserId, viewModel, token);
            if (authenticationResult.Status != ValidationStatus.AUTHENTICATE)
            {
                switch (authenticationResult.Status)
                {
                    case ValidationStatus.UNKNOWN_USER:
                        workflowResult.SetErrorMessage(Global.UserDoesntExist);
                        await Bus.Publish(new UserLoginFailureEvent
                        {
                            Realm = prefix,
                            Amr = Amr,
                            Login = viewModel.Login
                        });
                        return View(workflowResult);
                    case ValidationStatus.NOCONTENT:
                        if (!string.IsNullOrWhiteSpace(authenticationResult.ErrorCode) && !string.IsNullOrWhiteSpace(authenticationResult.ErrorMessage))
                            workflowResult.SetErrorMessage(authenticationResult.ErrorMessage);
                        return View(workflowResult);
                    case ValidationStatus.INVALIDCREDENTIALS:
                        using (var transaction = TransactionBuilder.Build())
                        {
                            var options = GetOptions();
                            workflowResult.SetErrorMessage(Global.InvalidCredential);
                            await Bus.Publish(new UserLoginFailureEvent
                            {
                                Realm = prefix,
                                Amr = Amr,
                                Login = viewModel.Login
                            });
                            if(authenticationResult.AuthenticatedUser != null)
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
               viewModel.ReturnUrl,
               Amr,
               authenticationResult.AuthenticatedUser,
               token,
               /*!viewModel.IsFirstAmr ? null : */ viewModel.RememberLogin);

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

            async Task ExtractRegistrationWorkflow()
            {
                if (!IsProtected(viewModel.ReturnUrl)) return;
                var acr = await _authenticationContextClassReferenceRepository.GetByAuthenticationWorkflow(prefix, viewModel.WorkflowId, token);
                viewModel.RegistrationWorkflowId = acr.RegistrationWorkflowId;
            }

            async Task<WorkflowViewModel> BuildWorkflowViewModel()
            {
                var records = await _formStore.GetAll(token);
                var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
                var workflow = await _workflowStore.Get(viewModel.WorkflowId, token);
                var step = workflow.GetStep(Amr);
                var workflowResult = new WorkflowViewModel
                {
                    CurrentStepId = step.Id,
                    Workflow = workflow,
                    FormRecords = records,
                    AntiforgeryToken = new AntiforgeryTokenRecord
                    {
                        CookieName = _formBuilderOptions.AntiforgeryCookieName,
                        CookieValue = tokenSet.CookieToken,
                        FormField = tokenSet.FormFieldName,
                        FormValue = tokenSet.RequestToken
                    }
                };
                return workflowResult;
            }
        }

        protected abstract Task<UserAuthenticationResult> CustomAuthenticate(string prefix, string authenticatedUserId, T viewModel, CancellationToken cancellationToken);

        protected abstract void EnrichViewModel(T viewModel);

        protected async Task UpdateViewModel(T viewModel)
        {
            IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme> externalIdProviders = new List<Microsoft.AspNetCore.Authentication.AuthenticationScheme>();
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

        private bool IsMaximumActiveSessionReached()
        {
            var allTickets = _sessionManager.FetchTicketsFromAllRealm(HttpContext);
            return (allTickets.Count() >= Options.MaxNbActiveSessions);
        }

        protected void SetSuccessMessage(string msg) => ViewBag.SuccessMessage = msg;

        protected abstract bool TryGetLogin(AmrAuthInfo amrInfo, out string login);

        protected async Task<(AmrAuthInfo, AuthenticationContextClassReference)?> ResolveAmrInfo(JsonObject query, string realm, Client client, CancellationToken cancellationToken)
        {
            var amrInfo = GetAmrInfo();
            if (amrInfo != null)
            {
	            var resolvedAcr = await _authenticationContextClassReferenceRepository.GetByName(realm, amrInfo.CurrentAmr, cancellationToken);
                return (amrInfo, resolvedAcr);
            }

            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var acr = await AmrHelper.FetchDefaultAcr(realm, acrValues, requestedClaims, client, cancellationToken);
            if (acr == null) return null;
            return (new AmrAuthInfo(null, null, null, null, acr.AuthenticationMethodReferences, acr.Name, acr.AuthenticationMethodReferences.First()), acr);
        }

        protected AmrAuthInfo GetAmrInfo()
        {
            if (!HttpContext.Request.Cookies.ContainsKey(Constants.DefaultCurrentAmrCookieName)) return null;
            var amr = JsonSerializer.Deserialize<AmrAuthInfo>(HttpContext.Request.Cookies[Constants.DefaultCurrentAmrCookieName]);
            return amr;
        }

        protected UserLockingOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(UserLockingOptions).Name);
            return section.Get<UserLockingOptions>();
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
