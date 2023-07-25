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

        public BaseAuthenticationMethodController(
            IOptions<IdServerHostOptions> options,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserTransformer userTransformer,
            IBusControl busControl) : base(clientRepository, userRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, options)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        protected abstract string Amr { get; }
        protected abstract bool IsExternalIdProvidersDisplayed { get; }

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
                if (IsProtected(returnUrl))
                {
                    var query = ExtractQuery(returnUrl);
                    var clientId = query.GetClientIdFromAuthorizationRequest();
                    var str = prefix ?? Constants.DefaultRealm; 
                    var client = await ClientRepository.Query().Include(c => c.Realms).Include(c => c.Translations).FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == str), cancellationToken);
                    var loginHint = query.GetLoginHintFromAuthorizationRequest();
                    var amrInfo = await ResolveAmrInfo(query, prefix, client, cancellationToken);
                    var authenticatedUser = await FetchAuthenticatedUser(str, amrInfo, cancellationToken);
                    bool isLoginMissing = authenticatedUser != null;
                    if (authenticatedUser != null && TryGetLogin(authenticatedUser, out string login))
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
                    viewModel.IsAuthInProgress = authenticatedUser != null;
                    viewModel.AmrAuthInfo = amrInfo;
                    EnrichViewModel(viewModel, authenticatedUser);
                    return View(viewModel);
                }

                return View(viewModel);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        protected abstract void EnrichViewModel(T viewModel, User user);

        protected abstract bool TryGetLogin(User user, out string login);

        #endregion

        #region Submit Credentials

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, T viewModel, CancellationToken token)
        {
            viewModel.Realm = prefix;
            prefix = prefix ?? Constants.DefaultRealm;
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            var amrInfo = GetAmrInfo();
            await UpdateViewModel(viewModel);
            var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, token);
            if(authenticatedUser == null)
            {
                authenticatedUser = await AuthenticateUser(viewModel.Login, prefix, token);
                if(authenticatedUser == null)
                {
                    ModelState.AddModelError("unknown_user", "unknown_user");
                    return View(viewModel);
                }
            }

            viewModel.CheckRequiredFields(authenticatedUser, ModelState);
            if (!ModelState.IsValid) return View(viewModel);
            var validationResult = await ValidateCredentials(viewModel, authenticatedUser, token);
            if (validationResult == ValidationStatus.NOCONTENT) return View(viewModel);
            if (validationResult == ValidationStatus.INVALIDCREDENTIALS)
            {
                await Bus.Publish(new UserLoginFailureEvent
                {
                    Realm = prefix,
                    Amr = Amr,
                    Login = viewModel.Login
                });
                return View(viewModel);
            }

            return await Authenticate(prefix, viewModel.ReturnUrl, Amr, authenticatedUser, token, viewModel.RememberLogin);
        }

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

        protected abstract Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken);

        protected abstract Task<ValidationStatus> ValidateCredentials(T viewModel, User user, CancellationToken cancellationToken);

        #endregion

        protected void SetSuccessMessage(string msg)
        {
            ViewBag.SuccessMessage = msg;
        }

        protected async Task<User> FetchAuthenticatedUser(string realm, AmrAuthInfo amrInfo, CancellationToken cancellationToken)
        {
            if (amrInfo == null || string.IsNullOrWhiteSpace(amrInfo.UserId)) return null;
            return await UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Properties)
                .Include(u => u.Groups)
                .Include(c => c.OAuthUserClaims)
                .Include(u => u.Credentials)
                .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Id == amrInfo.UserId, cancellationToken);
        }

        protected async Task<AmrAuthInfo> ResolveAmrInfo(JsonObject query, string realm, Client client, CancellationToken cancellationToken)
        {
            var amrInfo = GetAmrInfo();
            if (amrInfo != null) return amrInfo;
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var acr = await AmrHelper.FetchDefaultAcr(realm, acrValues, requestedClaims, client, cancellationToken);
            if (acr == null) return null;
            return new AmrAuthInfo(null, acr.AuthenticationMethodReferences, acr.AuthenticationMethodReferences.First());
        }

        protected AmrAuthInfo GetAmrInfo()
        {
            if (!HttpContext.Request.Cookies.ContainsKey(Constants.DefaultCurrentAmrCookieName)) return null;
            var amr = JsonSerializer.Deserialize<AmrAuthInfo>(HttpContext.Request.Cookies[Constants.DefaultCurrentAmrCookieName]);
            return amr;
        }

        protected enum ValidationStatus
        {
            AUTHENTICATE = 0,
            INVALIDCREDENTIALS = 1,
            NOCONTENT = 2
        }
    }
}
