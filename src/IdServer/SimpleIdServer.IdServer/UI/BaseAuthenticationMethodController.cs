// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
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
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IUserAuthenticationService _authenticationService;

        public BaseAuthenticationMethodController(
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IUserAuthenticationService userAuthenticationService,
            IDataProtectionProvider dataProtectionProvider,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IUserTransformer userTransformer,
            IBusControl busControl) : base(clientRepository, userRepository, userSessionRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, authenticationHelper, tokenRepository, jwtBuilder, options)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _authenticationService = userAuthenticationService;
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
                    var client = await ClientRepository.Query().Include(c => c.Realms).Include(c => c.Translations).FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == str), cancellationToken);
                    var loginHint = query.GetLoginHintFromAuthorizationRequest();
                    var amrInfo = await ResolveAmrInfo(query, str, client, cancellationToken);
                    bool isLoginMissing = amrInfo != null && !string.IsNullOrWhiteSpace(amrInfo.Login);
                    if (amrInfo != null && !string.IsNullOrWhiteSpace(amrInfo.Login) && TryGetLogin(amrInfo, out string login))
                    {
                        loginHint = login;
                        isLoginMissing = false;
                    }

                    viewModel.ClientName = client.ClientName;
                    viewModel.Login = loginHint;
                    viewModel.LogoUri = client.LogoUri;
                    viewModel.TosUri = client.TosUri;
                    viewModel.PolicyUri = client.PolicyUri;
                    viewModel.IsLoginMissing = isLoginMissing;
                    viewModel.IsAuthInProgress = amrInfo != null && !string.IsNullOrWhiteSpace(amrInfo.Login);
                    viewModel.AmrAuthInfo = amrInfo;
                    return View(viewModel);
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
        [ValidateAntiForgeryToken]
        public async virtual Task<IActionResult> Index([FromRoute] string prefix, T viewModel, CancellationToken token)
        {
            viewModel.Realm = prefix;
            prefix = prefix ?? Constants.DefaultRealm;
            if (viewModel == null) 
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            var amrInfo = GetAmrInfo();
            await UpdateViewModel(viewModel);
            viewModel.CheckRequiredFields(ModelState);
            if (!ModelState.IsValid) return View(viewModel);
            var result = await CustomAuthenticate(prefix, amrInfo?.UserId, viewModel, token);
            if (result.ActionResult != null) return result.ActionResult;
            CredentialsValidationResult authenticationResult = null;
            if (result.AuthenticatedUser != null) authenticationResult = await _authenticationService.Validate(prefix, result.AuthenticatedUser, viewModel, token);
            else authenticationResult = await _authenticationService.Validate(prefix, amrInfo?.UserId, viewModel, token);
            if (authenticationResult.Status != ValidationStatus.AUTHENTICATE)
            {
                switch(authenticationResult.Status)
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
                        if(!string.IsNullOrWhiteSpace(authenticationResult.ErrorCode) && !string.IsNullOrWhiteSpace(authenticationResult.ErrorMessage)) ModelState.AddModelError(authenticationResult.ErrorCode, authenticationResult.ErrorMessage);
                        return View(viewModel);
                    case ValidationStatus.INVALIDCREDENTIALS:
                        ModelState.AddModelError("invalid_credential", "invalid_credential");
                        await Bus.Publish(new UserLoginFailureEvent
                        {
                            Realm = prefix,
                            Amr = Amr,
                            Login = viewModel.Login
                        });
                        return View(viewModel);
                }
            }

            return await Authenticate(prefix, viewModel.ReturnUrl, Amr, authenticationResult.AuthenticatedUser, token, viewModel.RememberLogin);
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

            var amrInfo = GetAmrInfo();
            viewModel.AmrAuthInfo = amrInfo;
            viewModel.ExternalIdsProviders = externalIdProviders.Select(e => new ExternalIdProvider
            {
                AuthenticationScheme = e.Name,
                DisplayName = e.DisplayName
            }).ToList();
        }

        #endregion

        protected void SetSuccessMessage(string msg) => ViewBag.SuccessMessage = msg;

        protected abstract bool TryGetLogin(AmrAuthInfo amrInfo, out string login);

        protected async Task<AmrAuthInfo> ResolveAmrInfo(JsonObject query, string realm, Client client, CancellationToken cancellationToken)
        {
            var amrInfo = GetAmrInfo();
            if (amrInfo != null) return amrInfo;
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var acr = await AmrHelper.FetchDefaultAcr(realm, acrValues, requestedClaims, client, cancellationToken);
            if (acr == null) return null;
            return new AmrAuthInfo(null, null, null, null, acr.AuthenticationMethodReferences, acr.AuthenticationMethodReferences.First());
        }

        protected AmrAuthInfo GetAmrInfo()
        {
            if (!HttpContext.Request.Cookies.ContainsKey(Constants.DefaultCurrentAmrCookieName)) return null;
            var amr = JsonSerializer.Deserialize<AmrAuthInfo>(HttpContext.Request.Cookies[Constants.DefaultCurrentAmrCookieName]);
            return amr;
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
