﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.AuthProviders;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.IdServer.UI
{
    public class HomeController : Controller
    {
        private readonly IdServerHostOptions _options;
        private readonly IUserHelper _userHelper;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUmaPendingRequestRepository _pendingRequestRepository;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IOTPQRCodeGenerator _otpQRCodeGenerator;
        private readonly IBusControl _busControl;
        private readonly IEnumerable<IAuthenticationMethodService> _authenticationMethodServices;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IDistributedCache _distributedCache;
        private readonly ISessionHelper _sessionHelper;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<IdServerHostOptions> options,
            IUserHelper userHelper,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IUmaPendingRequestRepository pendingRequestRepository,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IOTPQRCodeGenerator otpQRCodeGenerator,
            IBusControl busControl,
            IEnumerable<IAuthenticationMethodService> authenticationMethodServices,
            IUserSessionResitory userSessionRepository,
            IRecurringJobManager recurringJobManager,
            IDistributedCache distributedCache,
            ISessionHelper sessionHelper,
            ITransactionBuilder transactionBuilder,
            ILogger<HomeController> logger)
        {
            _options = options.Value;
            _userHelper = userHelper;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _pendingRequestRepository = pendingRequestRepository;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _otpQRCodeGenerator = otpQRCodeGenerator;
            _busControl = busControl;
            _authenticationMethodServices = authenticationMethodServices;
            _userSessionRepository = userSessionRepository;
            _recurringJobManager = recurringJobManager;
            _distributedCache = distributedCache;
            _sessionHelper = sessionHelper;
            _transactionBuilder = transactionBuilder;
            _logger = logger;
        }

        [HttpGet]
        public virtual IActionResult Index() => View();

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> Profile([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var viewModel = new ProfileViewModel();
            await Build(prefix, viewModel, cancellationToken);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> UpdatePicture([FromResult] string prefix, IFormFile file, CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var issuer = Request.GetAbsoluteUriWithVirtualPath();
                var realm = prefix ?? Constants.DefaultRealm;
                var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var user = await _userRepository.GetBySubject(nameIdentifier, realm, cancellationToken);
                _userHelper.UpdatePicture(user, file, issuer);
                _userRepository.Update(user);
                await transaction.Commit(cancellationToken);
                return new NoContentResult();
            }
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> RejectConsent([FromRoute] string prefix, string consentId, CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                if (!_userHelper.HasOpenIDConsent(user, consentId))
                    return RedirectToAction("Index", "Errors", new { code = "invalid_request" });

                user.RejectConsent(consentId);
                _userRepository.Update(user);
                await transaction.Commit(cancellationToken);
                return RedirectToAction("Profile");
            }
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> RejectUmaPendingRequest(string ticketId, CancellationToken cancellationToken)
        {
            using(var transaction = _transactionBuilder.Build())
            {
                var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var pendingRequests = await _pendingRequestRepository.GetByPermissionTicketId(ticketId, cancellationToken);
                if (!pendingRequests.Any())
                    return NotFound();

                var pendingRequest = pendingRequests.First();
                if (pendingRequest.Owner != nameIdentifier)
                    return Unauthorized();

                if (pendingRequest.Status != UMAPendingRequestStatus.TOBECONFIRMED)
                    return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Pending request is not ready to be rejected" });

                pendingRequest.Reject();
                _pendingRequestRepository.Update(pendingRequest);
                await transaction.Commit(cancellationToken);
                return RedirectToAction("Profile");
            }
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> ConfirmUmaPendingRequest(string ticketId, CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var pendingRequests = await _pendingRequestRepository.GetByPermissionTicketId(ticketId, cancellationToken);
                if (!pendingRequests.Any())
                    return NotFound();

                var pendingRequest = pendingRequests.First();
                if (pendingRequest.Owner != nameIdentifier)
                    return Unauthorized();

                if (pendingRequest.Status != UMAPendingRequestStatus.TOBECONFIRMED)
                    return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Pending request is not ready to be confirmed" });

                pendingRequest.Confirm();
                _pendingRequestRepository.Update(pendingRequest);
                await transaction.Commit(cancellationToken);
                return RedirectToAction("Profile");
            }
        }

        #region Account Linking

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public virtual IActionResult Link(string scheme, string returnUrl)
        {
            if(string.IsNullOrWhiteSpace(scheme))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Authentication Scheme is missing" });

            var items = new Dictionary<string, string>
            {
                { ExternalAuthenticateController.SCHEME_NAME, scheme }
            };
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                items.Add(ExternalAuthenticateController.RETURN_URL_NAME, returnUrl);
            }

            var props = new AuthenticationProperties(items)
            {
                RedirectUri = Url.Action(nameof(LinkCallback)),
            };
            return Challenge(props, scheme);
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> LinkCallback([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var result = await HttpContext.AuthenticateAsync(Constants.DefaultExternalCookieAuthenticationScheme);
            if (result == null || !result.Succeeded)
            {
                if (result.Failure != null)
                    _logger.LogError(result.Failure.ToString());

                return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = Global.BadExternalAuthentication });
            }

            return await LinkProfile(result, cancellationToken);

            async Task<IActionResult> LinkProfile(AuthenticateResult authResult, CancellationToken cancellationToken)
            {
                var scheme = authResult.Properties.Items[ExternalAuthenticateController.SCHEME_NAME];
                var principal = authResult.Principal;
                var sub = UserTransformer.GetClaim(principal, JwtRegisteredClaimNames.Sub) ?? UserTransformer.GetClaim(principal, ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(sub))
                {
                    _logger.LogError("no subject can be extracted from the external authentication provider {authProviderScheme}", scheme);
                    await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                    return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = Global.BadExternalAuthenticationUser });
                }

                var isExternalAccountExists = await _userRepository.IsExternalAuthProviderExists(scheme, sub, prefix, cancellationToken);
                if (isExternalAccountExists)
                {
                    _logger.LogError("a local account has already been linked to the external authentication provider {authProviderScheme}", scheme);
                    await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                    return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = "a local account has already been linked to the external authentication provider" });
                }

                using (var transaction = _transactionBuilder.Build())
                {
                    var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                    if (!user.ExternalAuthProviders.Any(p => p.Subject == sub && p.Scheme == scheme))
                    {
                        user.AddExternalAuthProvider(scheme, sub);
                        _userRepository.Update(user);
                        await transaction.Commit(cancellationToken);
                        _logger.LogInformation("user account {userId} has been linked to the external account {authProviderScheme} with external subject {authProviderSubject}", nameIdentifier, scheme, sub);
                    }
                }

                await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                return RedirectToAction("Profile", "Home");
            }
        }

        [HttpPost]
        [Authorize(Constants.Policies.Authenticated)]
        [ValidateAntiForgeryToken]
        public async virtual Task<IActionResult> Unlink([FromRoute] string prefix, UnlinkProfileViewModel viewModel, CancellationToken cancellationToken)
        {
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Request cannot be empty" });

            using (var transaction = _transactionBuilder.Build())
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                var externalAuthProvider = user.ExternalAuthProviders.SingleOrDefault(p => p.Subject == viewModel.Subject && p.Scheme == viewModel.Scheme);
                if (externalAuthProvider == null)
                    return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Cannot unlink an unknown profile" });

                user.ExternalAuthProviders.Remove(externalAuthProvider);
                _userRepository.Update(user);
                await transaction.Commit(cancellationToken);
                return RedirectToAction("Profile", "Home");
            }
        }

        #endregion

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> GetOTP([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
            if (user.ActiveOTP == null) return new NoContentResult();
            var payload = _otpQRCodeGenerator.GenerateQRCode(user);
            return File(payload, "image/png");
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async virtual Task<IActionResult> RegisterCredential([FromRoute] string prefix, string name, string redirectUrl)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var cookieName = _options.GetRegistrationCookieName();
            if (Request.Cookies.ContainsKey(cookieName)) Response.Cookies.Delete(cookieName);
            var registrationProgress = new UserRegistrationProgress
            {
                RegistrationProgressId = Guid.NewGuid().ToString(),
                Amr = name,
                WorkflowName = name,
                Realm = prefix,
                Steps = new List<string> { name },
                RedirectUrl = redirectUrl
            };
            Response.Cookies.Append(cookieName, registrationProgress.RegistrationProgressId, new CookieOptions
            {
                Secure = true
            });
            await _distributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, Newtonsoft.Json.JsonConvert.SerializeObject(registrationProgress));
            return Redirect(Url.Action("Index", "Register", new { area = name, redirectUrl = redirectUrl }));
        }

        [HttpPost]
        public IActionResult SwitchLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(ReplaceUILocale(culture, returnUrl));
        }

        [HttpGet]
        public virtual async Task<IActionResult> Disconnect([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index");
            var sessionCookieName = _options.GetSessionCookieName();
            var subject = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var kvp = Request.Cookies.SingleOrDefault(c => c.Key == sessionCookieName);
            IEnumerable<string> frontChannelLogouts = new List<string>();
            if (!string.IsNullOrWhiteSpace(kvp.Key))
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    var activeSession = await _userSessionRepository.GetById(kvp.Value, prefix, cancellationToken);
                    var targetedClients = await _clientRepository.GetByClientIdsAndExistingFrontchannelLogoutUri(prefix, activeSession.ClientIds, cancellationToken);
                    frontChannelLogouts = targetedClients.Select(c => BuildFrontChannelLogoutUrl(c, kvp.Value));
                    if (activeSession != null && !activeSession.IsClientsNotified)
                    {
                        activeSession.State = UserSessionStates.Rejected;
                        _userSessionRepository.Update(activeSession);
                        await transaction.Commit(cancellationToken);
                        _recurringJobManager.Trigger(nameof(UserSessionJob));
                    }
                }
            }

            Response.Cookies.Delete(sessionCookieName);
            await HttpContext.SignOutAsync();
            await _busControl.Publish(new UserLogoutSuccessEvent
            {
                UserName = nameIdentifier,
                Realm = prefix
            });
            return View(new DisconnectViewModel
            {
                FrontChannelLogoutUrls = frontChannelLogouts
            });
        }

        protected async Task Build(string prefix, ProfileViewModel viewModel, CancellationToken cancellationToken)
        {
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
            var consents = await GetConsents();
            var pendingRequests = await GetPendingRequest();
            var methodServices = _authenticationMethodServices.Where(a => a.Capabilities.HasFlag(AuthenticationMethodCapabilities.USERREGISTRATION)).Select(a => new AuthenticationMethodViewModel
            {
                Name = a.Name,
                Amr = a.Amr,
                IsCredentialExists = a.IsCredentialExists(user)
            });
            var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
            viewModel.Name = user.Name;
            var claimPicture = user.OAuthUserClaims.FirstOrDefault(c => c.Name == Constants.StandardClaims.Picture.Name);
            if (claimPicture != null)
                viewModel.Picture = claimPicture.Value;
            viewModel.HasOtpKey = user.ActiveOTP != null;
            viewModel.Consents = consents;
            viewModel.PendingRequests = pendingRequests;
            viewModel.AuthenticationMethods = methodServices;
            viewModel.Profiles = GetProfiles();
            viewModel.ExternalIdProviders = externalIdProviders.Select(e => new ExternalIdProvider
            {
                AuthenticationScheme = e.Name,
                DisplayName = e.DisplayName
            });

            async Task<List<ConsentViewModel>> GetConsents()
            {
                var consents = new List<ConsentViewModel>();
                var filteredConsents = user.Consents.Where(c => c.Realm == prefix);
                var clientIds = filteredConsents.Select(c => c.ClientId).ToList();
                var oauthClients = await _clientRepository.GetByClientIds(prefix, clientIds, cancellationToken);
                foreach (var consent in filteredConsents)
                {
                    var oauthClient = oauthClients.SingleOrDefault(c => c.ClientId == consent.ClientId);
                    if (oauthClient == null) continue;
                    consents.Add(new ConsentViewModel(
                        consent.Id,
                        string.IsNullOrWhiteSpace(oauthClient.ClientName) ? oauthClient.ClientId : oauthClient.ClientName,
                        oauthClient.ClientUri,
                        consent.Scopes.Select(s => s.Scope),
                        consent.Claims,
                        consent.AuthorizationDetails.Select(s => s.Type)));
                }

                return consents;
            }

            async Task<List<PendingRequestViewModel>> GetPendingRequest()
            {
                var pendingRequestLst = await _pendingRequestRepository.GetByUsername(prefix, nameIdentifier, cancellationToken);
                var result = new List<PendingRequestViewModel>();
                foreach (var pendingRequest in pendingRequestLst)
                {
                    result.Add(new PendingRequestViewModel
                    {
                        TicketId = pendingRequest.TicketId,
                        CreateDateTime = pendingRequest.CreateDateTime,
                        Requester = pendingRequest.Requester,
                        ResourceDescription = pendingRequest.Resource.Description,
                        ResourceName = pendingRequest.Resource.Name,
                        Scopes = pendingRequest.Scopes,
                        Owner = pendingRequest.Owner,
                        Status = pendingRequest.Status
                    });
                }

                return result;
            }

            IEnumerable<ExternalAuthProviderViewModel> GetProfiles()
            {
                return user.ExternalAuthProviders.Select(a => new ExternalAuthProviderViewModel
                {
                    CreateDateTime = a.CreateDateTime,
                    Scheme = a.Scheme,
                    Subject = a.Subject
                });
            }
        }

        private static string ReplaceUILocale(string culture, string returnUrl)
        {
            var splitted = returnUrl.Split('?');
            if (splitted.Count() != 2)
                return returnUrl;

            var query = splitted.Last();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var queryDictionary = HttpUtility.ParseQueryString(query);
                if (queryDictionary.AllKeys.Contains(AuthorizationRequestParameters.UILocales))
                    queryDictionary[AuthorizationRequestParameters.UILocales] = culture;
                else
                    queryDictionary.Add(AuthorizationRequestParameters.UILocales, culture);

                var queryStr = string.Join("&", queryDictionary.AllKeys.Select(_ => $"{_}={queryDictionary[_]}"));
                returnUrl = $"{splitted.First()}?{queryStr}";
            }

            return returnUrl;
        }
        protected string BuildFrontChannelLogoutUrl(Client client, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(client.FrontChannelLogoutUri))
                return null;

            var url = client.FrontChannelLogoutUri;
            if (client.FrontChannelLogoutSessionRequired)
            {
                var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
                url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
                {
                    { JwtRegisteredClaimNames.Iss, issuer },
                    { JwtRegisteredClaimNames.Sid, sessionId }
                });
            }

            return url;
        }
    }
}
