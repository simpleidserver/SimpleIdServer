// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.Api.Authorization.ResponseModes;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.ExternalEvents;
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
        private readonly IBusControl _busControl;

        public ConsentsController(
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IUserConsentFetcher userConsentFetcher,
            IDataProtectionProvider dataProtectionProvider,
            IResponseModeHandler responseModeHandler,
            IExtractRequestHelper extractRequestHelper,
            IBusControl busControl)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _userConsentFetcher = userConsentFetcher;
            _responseModeHandler = responseModeHandler;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _extractRequestHelper = extractRequestHelper;
            _busControl = busControl;
        }

        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, bool isProtected = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });

            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
                var query = unprotectedUrl.GetQueries().ToJsonObject();
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var oauthClient = await _clientRepository.Query().Include(c => c.Translations).Include(c => c.Realms).Include(c => c.Scopes).Include(c => c.SerializedJsonWebKeys).AsNoTracking().FirstAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
                query = await _extractRequestHelper.Extract(prefix, Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
                var scopes = query.GetScopesFromAuthorizationRequest();
                var claims = query.GetClaimsFromAuthorizationRequest();
                var claimDescriptions = new List<string>();
                if (claims != null && claims.Any())
                    claimDescriptions = claims.Select(c => c.Name).ToList();

                return View(new ConsentsIndexViewModel(
                    oauthClient.ClientName,
                    returnUrl,
                    oauthClient.LogoUri,
                    oauthClient.Scopes.Where(c => scopes.Contains(c.Name)).Select(s => s.Name),
                    claimDescriptions));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Reject([FromRoute] string prefix, RejectConsentViewModel viewModel, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var nameIdentifier = GetNameIdentifier();
            var unprotectedUrl = _dataProtector.Unprotect(viewModel.ReturnUrl);
            var query = unprotectedUrl.GetQueries().ToJsonObject();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var oauthClient = await _clientRepository.Query().Include(c => c.Realms).AsNoTracking().FirstAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
            query = await _extractRequestHelper.Extract(prefix, Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
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
            await _busControl.Publish(new ConsentRevokedEvent
            {
                UserName = nameIdentifier,
                ClientId = clientId,
                Realm = prefix,
                Scopes = query.GetScopesFromAuthorizationRequest(),
                Claims = query.GetClaimsFromAuthorizationRequest().Select(c => c.Name)
            });
            var redirectUrlAuthorizationResponse = new RedirectURLAuthorizationResponse(redirectUri, dic);
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), null, query), prefix);
            _responseModeHandler.Handle(context, redirectUrlAuthorizationResponse, HttpContext);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, ConfirmConsentsViewModel confirmConsentsViewModel, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var unprotectedUrl = _dataProtector.Unprotect(confirmConsentsViewModel.ReturnUrl);
                var query = unprotectedUrl.GetQueries().ToJsonObject();
                var nameIdentifier = GetNameIdentifier();
                var user = await _userRepository.Query().Include(u => u.Realms).Include(u => u.Consents).FirstAsync(c => c.Name == nameIdentifier && c.Realms.Any(r => r.Name == prefix), cancellationToken);
                var consent = _userConsentFetcher.FetchFromAuthorizationRequest(prefix, user, query);
                if (consent == null)
                {
                    consent = _userConsentFetcher.BuildFromAuthorizationRequest(prefix, query);
                    user.Consents.Add(consent);
                    await _busControl.Publish(new ConsentGrantedEvent
                    {
                        UserName = nameIdentifier,
                        ClientId = consent.ClientId,
                        Scopes = consent.Scopes,
                        Claims = consent.Claims,
                        Realm = prefix
                    });
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
