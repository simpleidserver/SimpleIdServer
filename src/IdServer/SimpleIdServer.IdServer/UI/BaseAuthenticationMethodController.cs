// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.AuthProviders;
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
            IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository) : base(clientRepository, userRepository, userSessionRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, authenticationHelper, transactionBuilder, tokenRepository, jwtBuilder, options)
        {
            _configuration = configuration;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _authenticationService = userAuthenticationService;
            _antiforgery = antiforgery;
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
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
                IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme> externalIdProviders = new List<Microsoft.AspNetCore.Authentication.AuthenticationScheme>();
                if (IsExternalIdProvidersDisplayed)
                {
                    var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
                    externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
                }

                var viewModel = Activator.CreateInstance<T>();
                viewModel.ReturnUrl = returnUrl;
                viewModel.Realm = prefix;
                viewModel.IsFirstAmr = false;
                viewModel.ExternalIdsProviders = externalIdProviders.Select(e => new ExternalIdProvider
                {
                    AuthenticationScheme = e.Name,
                    DisplayName = e.DisplayName
                }).ToList();
                EnrichViewModel(viewModel);
                if (IsProtected(returnUrl))
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
                    return View(viewModel);
                }
                else
                {
                    viewModel.IsFirstAmr = true;
                    viewModel.RememberLogin = false;
                }

                return View(viewModel);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        #endregion

        #region Submit Credentials

        [HttpPost]
        public async virtual Task<IActionResult> Index([FromRoute] string prefix, T viewModel, CancellationToken token)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(this.HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = Global.InvalidAntiForgeryToken });
                }

                var returnUrl = viewModel.ReturnUrl;
                if (!IsProtected(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                var unprotectedUrl = Unprotect(returnUrl);
                return Redirect(unprotectedUrl);
            }

            viewModel.Realm = prefix;
            prefix = prefix ?? Constants.DefaultRealm;
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            AmrAuthInfo amrInfo = null;
            if (IsProtected(viewModel.ReturnUrl))
            {
                var query = ExtractQuery(viewModel.ReturnUrl);
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var client = await ClientRepository.GetByClientId(prefix, clientId, token);
                var res = await ResolveAmrInfo(query, prefix, client, token);
                amrInfo = res?.Item1;
                viewModel.AmrAuthInfo = amrInfo;
            }

            EnrichViewModel(viewModel);
            await UpdateViewModel(viewModel);
            viewModel.Validate(ModelState);
            if (!ModelState.IsValid) return View(viewModel);
            var result = await CustomAuthenticate(prefix, amrInfo?.UserId, viewModel, token);
            if (result.ActionResult != null) return result.ActionResult;
            CredentialsValidationResult authenticationResult = null;
            if (result.AuthenticatedUser != null) authenticationResult = await _authenticationService.Validate(prefix, result.AuthenticatedUser, viewModel, token);
            else authenticationResult = await _authenticationService.Validate(prefix, amrInfo?.UserId, viewModel, token);
            if (authenticationResult.Status != ValidationStatus.AUTHENTICATE)
            {
                switch (authenticationResult.Status)
                {
                    case ValidationStatus.UNKNOWN_USER:
                        ModelState.AddModelError("unknown_user", "unknown_user");
                        await Bus.Publish(new UserLoginFailureEvent
                        {
                            Realm = prefix,
                            Amr = Amr,
                            Login = viewModel.Login
                        });
                        return View(viewModel);
                    case ValidationStatus.NOCONTENT:
                        if (!string.IsNullOrWhiteSpace(authenticationResult.ErrorCode) && !string.IsNullOrWhiteSpace(authenticationResult.ErrorMessage)) ModelState.AddModelError(authenticationResult.ErrorCode, authenticationResult.ErrorMessage);
                        return View(viewModel);
                    case ValidationStatus.INVALIDCREDENTIALS:
                        using (var transaction = TransactionBuilder.Build())
                        {
                            var options = GetOptions();
                            ModelState.AddModelError("invalid_credential", "invalid_credential");
                            await Bus.Publish(new UserLoginFailureEvent
                            {
                                Realm = prefix,
                                Amr = Amr,
                                Login = viewModel.Login
                            });
                            authenticationResult.AuthenticatedUser.LoginAttempt(options.MaxLoginAttempts, options.LockTimeInSeconds);
                            UserRepository.Update(authenticationResult.AuthenticatedUser);
                            await transaction.Commit(token);
                            return View(viewModel);
                        }
                }
            }

            authenticationResult.AuthenticatedUser.ResetLoginAttempt();
            return await Authenticate(prefix,
               viewModel.ReturnUrl,
               Amr,
               authenticationResult.AuthenticatedUser,
               token,
               !viewModel.IsFirstAmr ? null : viewModel.RememberLogin);
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
        private UserAuthenticationResult(IActionResult result)
        {
            ActionResult = result;
        }

        private UserAuthenticationResult(User authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
        }

        public IActionResult ActionResult { get; set; }
        public User AuthenticatedUser { get; set; }

        public static UserAuthenticationResult Ok(User authenticatedUser = null) => new UserAuthenticationResult(authenticatedUser);
        public static UserAuthenticationResult Error(IActionResult result) => new UserAuthenticationResult(result);
    }
}
