// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.Api.Authorization.ResponseModes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
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
    [Authorize(Constants.AuthenticatedPolicyName)]
    public class ConsentsController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUserConsentFetcher _userConsentFetcher;
        private readonly IDataProtector _dataProtector;
        private readonly IResponseModeHandler _responseModeHandler;
        private readonly IExtractRequestHelper _extractRequestHelper;
        private readonly ITokenRepository _tokenRepository;
        private readonly IBusControl _busControl;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly ILogger<ConsentsController> _logger;
        private readonly IdServerHostOptions _options;

        public ConsentsController(
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IUserConsentFetcher userConsentFetcher,
            IDataProtectionProvider dataProtectionProvider,
            IResponseModeHandler responseModeHandler,
            IExtractRequestHelper extractRequestHelper,
            ITokenRepository tokenRepository,
            IBusControl busControl,
            ITransactionBuilder transactionBuilder,
            ILogger<ConsentsController> logger,
            IOptions<IdServerHostOptions> options)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _userConsentFetcher = userConsentFetcher;
            _responseModeHandler = responseModeHandler;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _extractRequestHelper = extractRequestHelper;
            _tokenRepository = tokenRepository;
            _busControl = busControl;
            _transactionBuilder = transactionBuilder;
            _logger = logger;
            _options = options.Value;
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
                var oauthClient = await _clientRepository.GetByClientId(prefix, clientId, cancellationToken);
                var grantId = query.GetGrantIdFromAuthorizationRequest();
                if(!string.IsNullOrWhiteSpace(grantId))
                    return View(await BuildConsentsFromGrant(query, oauthClient, grantId));

                return View(await BuildConsentsFromAuthRequest(query, oauthClient));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }

            async Task<ConsentsIndexViewModel> BuildConsentsFromGrant(JsonObject query, Client oauthClient, string grantId)
            {
                var nameIdentifier = GetNameIdentifier();
                var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                var grant = user.Consents.First(c => c.Id == grantId);
                var claimDescriptions = new List<string>();
                if (grant.Claims != null && grant.Claims.Any())
                    claimDescriptions = grant.Claims.ToList();

                var scopes = grant.Scopes.Select(s => s.Scope);
                return new ConsentsIndexViewModel(
                    oauthClient.ClientName,
                    returnUrl,
                    oauthClient.LogoUri,
                    oauthClient.Scopes.Where(c => scopes.Contains(c.Name)).Select(s => s.Name),
                    claimDescriptions,
                    grant.AuthorizationDetails);
            }

            async Task<ConsentsIndexViewModel> BuildConsentsFromAuthRequest(JsonObject query, Client oauthClient)
            {
                var authDetails = query.GetAuthorizationDetailsFromAuthorizationRequest();
                query = await _extractRequestHelper.Extract(prefix, Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
                var scopes = query.GetScopesFromAuthorizationRequest();
                var claims = query.GetClaimsFromAuthorizationRequest();
                var claimDescriptions = new List<string>();
                if (claims != null && claims.Any())
                    claimDescriptions = claims.Select(c => c.Name).ToList();

                return new ConsentsIndexViewModel(
                    oauthClient.ClientName,
                    returnUrl,
                    oauthClient.LogoUri,
                    oauthClient.Scopes.Where(c => scopes.Contains(c.Name)).Select(s => s.Name),
                    claimDescriptions,
                    authDetails);
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
            var oauthClient = await _clientRepository.GetByClientId(prefix, clientId, cancellationToken);
            query = await _extractRequestHelper.Extract(prefix, Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
            var redirectUri = query.GetRedirectUriFromAuthorizationRequest();
            var grantId = query.GetGrantIdFromAuthorizationRequest();
            var scopes = query.GetScopesFromAuthorizationRequest();
            var claims = query.GetClaimsFromAuthorizationRequest().Select(c => c.Name);
            if (!string.IsNullOrWhiteSpace(grantId))
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                    var consent = user.Consents.Single(c => c.Id == grantId);
                    user.Consents.Remove(consent);
                    _userRepository.Update(user);
                    await transaction.Commit(cancellationToken);
                    scopes = consent.Scopes.Select(s => s.Scope);
                    claims = consent.Claims;
                }
            }

            var state = query.GetStateFromAuthorizationRequest();
            var jObj = new JsonObject
            {
                { ErrorResponseParameters.Error, ErrorCodes.ACCESS_DENIED },
                { ErrorResponseParameters.ErrorDescription, Global.AccessRevokedByResourceOwner }
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
                Scopes = scopes,
                Claims = claims
            });
            Counters.RejectConsent(clientId, prefix);
            var redirectUrlAuthorizationResponse = new RedirectURLAuthorizationResponse(redirectUri, dic);
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), null, query), prefix, _options);
            _responseModeHandler.Handle(context, redirectUrlAuthorizationResponse, HttpContext);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, ConfirmConsentsViewModel viewModel, CancellationToken cancellationToken)
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    var unprotectedUrl = _dataProtector.Unprotect(viewModel.ReturnUrl);
                    var query = unprotectedUrl.GetQueries().ToJsonObject();
                    var grantId = query.GetGrantIdFromAuthorizationRequest();
                    var clientId = query.GetClientIdFromAuthorizationRequest();
                    var oauthClient = await _clientRepository.GetByClientId(prefix, clientId, cancellationToken);
                    query = await _extractRequestHelper.Extract(prefix, Request.GetAbsoluteUriWithVirtualPath(), query, oauthClient);
                    var nameIdentifier = GetNameIdentifier();
                    var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(grantId))
                    {
                        var consent = user.Consents.Single(c => c.Id == grantId);
                        consent.Accept();
                        await RevokeTokens(grantId, cancellationToken);
                        Counters.AcceptConsent(clientId, prefix);
                        await _busControl.Publish(new ConsentGrantedEvent
                        {
                            UserName = nameIdentifier,
                            ClientId = consent.ClientId,
                            Scopes = consent.Scopes.Select(s => s.Scope),
                            Claims = consent.Claims,
                            Realm = prefix
                        });
                    }
                    else
                    {
                        var consent = _userConsentFetcher.FetchFromAuthorizationRequest(prefix, user, query);
                        if (consent == null)
                        {
                            consent = _userConsentFetcher.BuildFromAuthorizationRequest(prefix, query);
                            user.Consents.Add(consent);
                            Counters.AcceptConsent(clientId, prefix);
                            await _busControl.Publish(new ConsentGrantedEvent
                            {
                                UserName = nameIdentifier,
                                ClientId = consent.ClientId,
                                Scopes = consent.Scopes.Select(s => s.Scope),
                                Claims = consent.Claims,
                                Realm = prefix
                            });
                        }
                    }

                    _userRepository.Update(user);
                    await transaction.Commit(cancellationToken);
                    return Redirect(unprotectedUrl);
                }
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("invalid_request", "invalid_request");
                return await Index(prefix, viewModel.ReturnUrl, true, cancellationToken);
            }

            async Task RevokeTokens(string grantId, CancellationToken cancellationToken)
            {
                var tokens = await _tokenRepository.GetByGrantId(grantId, cancellationToken);
                foreach (var t in tokens)
                    _tokenRepository.Remove(t);
            }
        }

        private string GetNameIdentifier()
        {
            var claimName = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier);
            return claimName.Value;
        }
    }
}
