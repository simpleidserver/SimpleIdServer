// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseModes;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI
{
    [Authorize("IsConnected")]
    public class ConsentsController : Controller
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly IOAuthClientRepository _oauthClientRepository;
        private readonly IUserConsentFetcher _userConsentFetcher;
        private readonly IDataProtector _dataProtector;
        private readonly IResponseModeHandler _responseModeHandler;
        private readonly ITranslationHelper _translationHelper;
        private readonly IExtractRequestHelper _extractRequestHelper;

        public ConsentsController(
            IOAuthUserRepository oauthUserRepository, 
            IOAuthClientRepository oauthClientRepository, 
            IUserConsentFetcher userConsentFetcher, 
            IDataProtectionProvider dataProtectionProvider, 
            IResponseModeHandler responseModeHandler,
            ITranslationHelper translationHelper,
            IExtractRequestHelper extractRequestHelper)
        {
            _oauthUserRepository = oauthUserRepository;
            _oauthClientRepository = oauthClientRepository;
            _userConsentFetcher = userConsentFetcher;
            _responseModeHandler = responseModeHandler;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _translationHelper = translationHelper;
            _extractRequestHelper = extractRequestHelper;
        }

        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }

            try
            {
                var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
                var query = unprotectedUrl.GetQueries().ToJObj();
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var oauthClient = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
                query = await _extractRequestHelper.Extract(Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
                var scopes = query.GetScopesFromAuthorizationRequest();
                var claims = query.GetClaimsFromAuthorizationRequest();
                var claimDescriptions = new List<string>();
                if (claims != null && claims.Any())
                {
                    claimDescriptions = claims.Select(c => c.Name).ToList();
                }

                return View(new ConsentsIndexViewModel(
                    _translationHelper.Translate(oauthClient.ClientNames, oauthClient.ClientId),
                    returnUrl,
                    oauthClient.AllowedScopes.Where(c => scopes.Contains(c.Name)).Select(s => s.Name),
                    claimDescriptions));
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }
        }

        public async Task<IActionResult> Manage(CancellationToken cancellationToken)
        {
            var nameIdentifier = GetNameIdentifier();
            var user = await _oauthUserRepository.FindOAuthUserByLogin(nameIdentifier, cancellationToken);
            var result = new List<ConsentViewModel>();
            var oauthClients = await _oauthClientRepository.FindOAuthClientByIds(user.Consents.Select(c => c.ClientId), cancellationToken);
            foreach(var consent in user.Consents)
            {
                var oauthClient = oauthClients.Single(c => c.ClientId == consent.ClientId);
                result.Add(new ConsentViewModel(
                    consent.Id,
                    _translationHelper.Translate(oauthClient.ClientNames, oauthClient.ClientId),
                    consent.Scopes.Select(s => s.Name),
                    consent.Claims));
            }

            return View(result);
        }

        public async Task<IActionResult> RejectConsent(string consentId, CancellationToken cancellationToken)
        {
            var nameIdentifier = GetNameIdentifier();
            var user = await _oauthUserRepository.FindOAuthUserByLogin(nameIdentifier, cancellationToken);
            if (!user.HasOpenIDConsent(consentId))
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request" });
            }

            user.RejectConsent(consentId);
            await _oauthUserRepository.Update(user, cancellationToken);
            await _oauthUserRepository.SaveChanges(cancellationToken);
            return RedirectToAction("Manage");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Reject(RejectConsentViewModel viewModel, CancellationToken cancellationToken)
        {
            var unprotectedUrl = _dataProtector.Unprotect(viewModel.ReturnUrl);
            var query = unprotectedUrl.GetQueries().ToJObj();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var oauthClient = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
            query = await _extractRequestHelper.Extract(Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
            var redirectUri = query.GetRedirectUriFromAuthorizationRequest();
            var state = query.GetStateFromAuthorizationRequest();
            var jObj = new JObject
            {
                { ErrorResponseParameters.Error, ErrorCodes.ACCESS_DENIED },
                { ErrorResponseParameters.ErrorDescription, ErrorMessages.ACCESS_REVOKED_BY_RESOURCE_OWNER }
            };
            if (!string.IsNullOrWhiteSpace(state))
            {
                jObj.Add(ErrorResponseParameters.State, state);
            }

            var dic = jObj.ToEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var redirectUrlAuthorizationResponse = new RedirectURLAuthorizationResponse(redirectUri, dic);
            _responseModeHandler.Handle(query, redirectUrlAuthorizationResponse, HttpContext);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConfirmConsentsViewModel confirmConsentsViewModel, CancellationToken token)
        { 
            try
            {
                var unprotectedUrl = _dataProtector.Unprotect(confirmConsentsViewModel.ReturnUrl);
                var query = unprotectedUrl.GetQueries().ToJObj();
                var nameIdentifier = GetNameIdentifier();
                var user = await _oauthUserRepository.FindOAuthUserByLogin(nameIdentifier, token);
                var consent = _userConsentFetcher.FetchFromAuthorizationRequest(user, query);
                if (consent == null)
                {
                    consent = _userConsentFetcher.BuildFromAuthorizationRequest(query);
                    user.Consents.Add(consent);
                    await _oauthUserRepository.Update(user, token);
                    await _oauthUserRepository.SaveChanges(token);
                }

                return Redirect(unprotectedUrl);
            }
            catch(CryptographicException)
            {
                ModelState.AddModelError("invalid_request", "invalid_request");
                return View(confirmConsentsViewModel);
            }
        }

        private string GetNameIdentifier()
        {
            var claimName = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier);
            return claimName.Value;
        }
    }
}