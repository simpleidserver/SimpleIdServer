// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.Api.Authorization.ResponseModes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Authorize(Constants.Policies.Authenticated)]
    public class ConsentsController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUserConsentFetcher _userConsentFetcher;
        private readonly IDataProtector _dataProtector;
        private readonly IResponseModeHandler _responseModeHandler;
        private readonly IExtractRequestHelper _extractRequestHelper;

        public ConsentsController(
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IUserConsentFetcher userConsentFetcher,
            IDataProtectionProvider dataProtectionProvider,
            IResponseModeHandler responseModeHandler,
            IExtractRequestHelper extractRequestHelper)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _userConsentFetcher = userConsentFetcher;
            _responseModeHandler = responseModeHandler;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _extractRequestHelper = extractRequestHelper;
        }

        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });

            try
            {
                var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
                var query = unprotectedUrl.GetQueries().ToJsonObject();
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var oauthClient = await _clientRepository.Query().Include(c => c.Scopes).AsNoTracking().FirstAsync(c => c.ClientId == clientId, cancellationToken);
                query = await _extractRequestHelper.Extract(Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
                var scopes = query.GetScopesFromAuthorizationRequest();
                var claims = query.GetClaimsFromAuthorizationRequest();
                var claimDescriptions = new List<string>();
                if (claims != null && claims.Any())
                    claimDescriptions = claims.Select(c => c.Name).ToList();

                return View(new ConsentsIndexViewModel(
                    oauthClient.ClientName,
                    returnUrl,
                    oauthClient.Scopes.Where(c => scopes.Contains(c.Name)).Select(s => s.Name),
                    claimDescriptions));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }
        }

        public async Task<IActionResult> Manage(CancellationToken cancellationToken)
        {
            var nameIdentifier = GetNameIdentifier();
            var user = await _userRepository.Query().Include(u => u.Consents).AsNoTracking().FirstAsync(c => c.Id == nameIdentifier, cancellationToken);
            var result = new List<ConsentViewModel>();
            var clientIds = user.Consents.Select(c => c.ClientId);
            var oauthClients = await _clientRepository.Query().Include(c => c.Translations).AsNoTracking().Where(c => clientIds.Contains(c.ClientId)).ToListAsync(cancellationToken);
            foreach (var consent in user.Consents)
            {
                var oauthClient = oauthClients.Single(c => c.ClientId == consent.ClientId);
                result.Add(new ConsentViewModel(
                    consent.Id,
                    oauthClient.ClientName,
                    consent.Scopes,
                    consent.Claims));
            }

            return View(result);
        }

        public async Task<IActionResult> RejectConsent(string consentId, CancellationToken cancellationToken)
        {
            var nameIdentifier = GetNameIdentifier();
            var user = await _userRepository.Query().Include(u => u.Consents).FirstAsync(c => c.Id == nameIdentifier, cancellationToken);
            if (!user.HasOpenIDConsent(consentId))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request" });

            user.RejectConsent(consentId);
            await _userRepository.SaveChanges(cancellationToken);
            return RedirectToAction("Manage");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Reject(RejectConsentViewModel viewModel, CancellationToken cancellationToken)
        {
            var unprotectedUrl = _dataProtector.Unprotect(viewModel.ReturnUrl);
            var query = unprotectedUrl.GetQueries().ToJsonObject();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var oauthClient = await _clientRepository.Query().AsNoTracking().FirstAsync(c => c.ClientId == clientId, cancellationToken);
            query = await _extractRequestHelper.Extract(Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
            var redirectUri = query.GetRedirectUriFromAuthorizationRequest();
            var state = query.GetStateFromAuthorizationRequest();
            var jObj = new JsonObject
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
        public async Task<IActionResult> Index(ConfirmConsentsViewModel confirmConsentsViewModel, CancellationToken cancellationToken)
        {
            try
            {
                var unprotectedUrl = _dataProtector.Unprotect(confirmConsentsViewModel.ReturnUrl);
                var query = unprotectedUrl.GetQueries().ToJsonObject();
                var nameIdentifier = GetNameIdentifier();
                var user = await _userRepository.Query().Include(u => u.Consents).FirstAsync(c => c.Id == nameIdentifier, cancellationToken);
                var consent = _userConsentFetcher.FetchFromAuthorizationRequest(user, query);
                if (consent == null)
                {
                    consent = _userConsentFetcher.BuildFromAuthorizationRequest(query);
                    user.Consents.Add(consent);
                    await _userRepository.SaveChanges(cancellationToken);
                }

                return Redirect(unprotectedUrl);
            }
            catch (CryptographicException)
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
