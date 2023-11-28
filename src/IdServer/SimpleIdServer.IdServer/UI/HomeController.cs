// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.AuthProviders;
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
        private readonly ICredentialTemplateRepository _credentialTemplateRepository;
        private readonly IBusControl _busControl;
        private readonly IEnumerable<IAuthenticationMethodService> _authenticationMethodServices;
        private readonly ISessionHelper _sessionHelper;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<IdServerHostOptions> options,
            IUserHelper userHelper,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IUmaPendingRequestRepository pendingRequestRepository,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IOTPQRCodeGenerator otpQRCodeGenerator,
            ICredentialTemplateRepository credentialTemplateRepository,
            IBusControl busControl,
            IEnumerable<IAuthenticationMethodService> authenticationMethodServices,
            ISessionHelper sessionHelper,
            IUserSessionResitory userSessionRepository,
            ILogger<HomeController> logger)
        {
            _options = options.Value;
            _userHelper = userHelper;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _pendingRequestRepository = pendingRequestRepository;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _otpQRCodeGenerator = otpQRCodeGenerator;
            _credentialTemplateRepository = credentialTemplateRepository;
            _busControl = busControl;
            _authenticationMethodServices = authenticationMethodServices;
            _sessionHelper = sessionHelper;
            _userSessionRepository = userSessionRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> Profile([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
            var consents = await GetConsents();
            var pendingRequests = await GetPendingRequest();
            var methodServices = _authenticationMethodServices.Select(a => new AuthenticationMethodViewModel
            {
                Name = a.Name,
                Amr = a.Amr,
                IsCredentialExists = a.IsCredentialExists(user)
            });
            var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
            return View(new ProfileViewModel
            {
                Name = user.Name,
                HasOtpKey = user.ActiveOTP != null,
                Consents = consents,
                PendingRequests = pendingRequests,
                AuthenticationMethods = methodServices,
                Profiles = GetProfiles(),
                ExternalIdProviders = externalIdProviders.Select(e => new ExternalIdProvider
                {
                    AuthenticationScheme = e.Name,
                    DisplayName = e.DisplayName
                })
            });

            async Task<List<ConsentViewModel>> GetConsents()
            {
                var consents = new List<ConsentViewModel>();
                var filteredConsents = user.Consents.Where(c => c.Realm == prefix);
                var clientIds = filteredConsents.Select(c => c.ClientId);
                var oauthClients = await _clientRepository.Query().Include(c => c.Translations).Include(r => r.Realms).AsNoTracking().Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == prefix)).ToListAsync(cancellationToken);
                foreach (var consent in filteredConsents)
                {
                    var oauthClient = oauthClients.Single(c => c.ClientId == consent.ClientId);
                    consents.Add(new ConsentViewModel(
                        consent.Id,
                        oauthClient.ClientName,
                        oauthClient.ClientUri,
                        consent.Scopes.Select(s => s.Scope),
                        consent.Claims,
                        consent.AuthorizationDetails.Select(s => s.Type)));
                }

                return consents;
            }

            async Task<List<PendingRequestViewModel>> GetPendingRequest()
            {
                var pendingRequestLst = await _pendingRequestRepository.Query().Include(p => p.Resource).ThenInclude(p => p.Translations).Where(r => (r.Owner == nameIdentifier || r.Requester == nameIdentifier) && r.Realm == prefix).ToListAsync(cancellationToken);
                var result = new List<PendingRequestViewModel>();
                foreach(var pendingRequest in pendingRequestLst)
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

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> Credentials([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var credentials = await GetCredentialTemplates();
            return View(new CredentialsViewModel
            {
                Credentials = credentials,
                ClientIds = await GetClients()
            });

            async Task<ICollection<CredentialTemplateViewModel>> GetCredentialTemplates()
            {
                var credentialTemplates = await _credentialTemplateRepository.Query().Include(c => c.DisplayLst).Include(c => c.Realms).AsNoTracking().Where(c => c.Realms.All(r => r.Name == prefix)).ToListAsync(cancellationToken);
                return credentialTemplates.Select(c => new CredentialTemplateViewModel
                {
                    Id = c.TechnicalId,
                    Display = c.Display
                }).ToList();
            }

            async Task<IEnumerable<string>> GetClients()
            {
                var clients = await _clientRepository.Query().Include(c => c.Realms).Where(c => c.ClientType == ClientTypes.WALLET && c.Realms.All(r => r.Name == prefix)).ToListAsync(cancellationToken);
                return clients.Select(c => c.ClientId);
            }
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> RejectConsent([FromRoute] string prefix, string consentId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
            if (!_userHelper.HasOpenIDConsent(user, consentId))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request" });

            user.RejectConsent(consentId);
            await _userRepository.SaveChanges(cancellationToken);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> RejectUmaPendingRequest(string ticketId, CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var pendingRequest = await _pendingRequestRepository.Query().FirstOrDefaultAsync(p => p.TicketId == ticketId, cancellationToken);
            if (pendingRequest == null)
                return NotFound();

            if (pendingRequest.Owner != nameIdentifier)
                return Unauthorized();

            if (pendingRequest.Status != UMAPendingRequestStatus.TOBECONFIRMED)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Pending request is not ready to be rejected" });

            pendingRequest.Reject();
            await _pendingRequestRepository.SaveChanges(cancellationToken);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> ConfirmUmaPendingRequest(string ticketId, CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var pendingRequest = await _pendingRequestRepository.Query().FirstOrDefaultAsync(p => p.TicketId == ticketId, cancellationToken);
            if (pendingRequest == null)
                return NotFound();

            if (pendingRequest.Owner != nameIdentifier)
                return Unauthorized();

            if (pendingRequest.Status != UMAPendingRequestStatus.TOBECONFIRMED)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Pending request is not ready to be confirmed" });

            pendingRequest.Confirm();
            await _pendingRequestRepository.SaveChanges(cancellationToken);
            return RedirectToAction("Profile");
        }

        #region Account Linking

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public IActionResult Link(string scheme)
        {
            if(string.IsNullOrWhiteSpace(scheme))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Authentication Scheme is missing" });

            var items = new Dictionary<string, string>
            {
                { ExternalAuthenticateController.SCHEME_NAME, scheme }
            };
            var props = new AuthenticationProperties(items)
            {
                RedirectUri = Url.Action(nameof(LinkCallback)),
            };
            return Challenge(props, scheme);
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> LinkCallback([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var result = await HttpContext.AuthenticateAsync(Constants.DefaultExternalCookieAuthenticationScheme);
            if (result == null || !result.Succeeded)
            {
                if (result.Failure != null)
                    _logger.LogError(result.Failure.ToString());

                return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = ErrorMessages.BAD_EXTERNAL_AUTHENTICATION });
            }

            return await LinkProfile(result, cancellationToken);

            async Task<IActionResult> LinkProfile(AuthenticateResult authResult, CancellationToken cancellationToken)
            {
                var scheme = authResult.Properties.Items[ExternalAuthenticateController.SCHEME_NAME];
                var principal = authResult.Principal;
                var sub = ExternalAuthenticateController.GetClaim(principal, JwtRegisteredClaimNames.Sub) ?? ExternalAuthenticateController.GetClaim(principal, ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(sub))
                {
                    _logger.LogError("no subject can be extracted from the external authentication provider {authProviderScheme}", scheme);
                    await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                    return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = ErrorMessages.BAD_EXTERNAL_AUTHENTICATION_USER });
                }

                var isExternalAccountExists = await _userRepository.IsExternalAuthProviderExists(scheme, sub, prefix, cancellationToken);
                if (isExternalAccountExists)
                {
                    _logger.LogError("a local account has already been linked to the external authentication provider {authProviderScheme}", scheme);
                    await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                    return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = "a local account has already been linked to the external authentication provider" });
                }

                var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
                if (!user.ExternalAuthProviders.Any(p => p.Subject == sub && p.Scheme == scheme))
                {
                    user.AddExternalAuthProvider(scheme, sub);
                    await _userRepository.SaveChanges(cancellationToken);
                    _logger.LogInformation("user account {userId} has been linked to the external account {authProviderScheme} with external subject {authProviderSubject}", nameIdentifier, scheme, sub);
                }

                await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                return RedirectToAction("Profile", "Home");
            }
        }

        [HttpPost]
        [Authorize(Constants.Policies.Authenticated)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlink([FromRoute] string prefix, UnlinkProfileViewModel viewModel, CancellationToken cancellationToken)
        {
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Request cannot be empty" });

            prefix = prefix ?? Constants.DefaultRealm;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetBySubject(nameIdentifier, prefix, cancellationToken);
            var externalAuthProvider = user.ExternalAuthProviders.SingleOrDefault(p => p.Subject == viewModel.Subject && p.Scheme == viewModel.Scheme);
            if(externalAuthProvider == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Cannot unlink an unknown profile" });

            user.ExternalAuthProviders.Remove(externalAuthProvider);
            _userRepository.Update(user);
            await _userRepository.SaveChanges(cancellationToken);
            return RedirectToAction("Profile", "Home");
        }

        #endregion

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> GetOTP([FromRoute] string prefix, CancellationToken cancellationToken)
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
        public IActionResult RegisterCredential([FromRoute] string prefix, string name)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var cookieName = _options.GetRegistrationCookieName();
            if (Request.Cookies.ContainsKey(cookieName)) Response.Cookies.Delete(cookieName);
            return Redirect(Url.Action("Index", "Register", new { area = name }));
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
        public IActionResult Authenticate()
        {
            var returnUrl = Url.Action(nameof(Index));
            var items = new Dictionary<string, string>
            {
                { "scheme", Constants.DefaultOIDCAuthenticationScheme },
                { "returnUrl", returnUrl }
            };
            var props = new AuthenticationProperties(items)
            {
                RedirectUri = returnUrl
            };
            return Challenge(props, Constants.DefaultOIDCAuthenticationScheme);
        }

        [HttpGet]
        public IActionResult ChooseSession()
        {
            var returnUrl = Url.Action(nameof(Index));
            var items = new Dictionary<string, string>
            {
                { "scheme", Constants.DefaultOIDCAuthenticationScheme },
                { "returnUrl", returnUrl },
                { "prompt", PromptParameters.SelectAccount }
            };
            var props = new AuthenticationProperties(items)
            {
                RedirectUri = returnUrl
            };
            return Challenge(props, Constants.DefaultOIDCAuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> Disconnect([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var sessionCookieName = _options.GetSessionCookieName();
            if (User.Identity == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index");
            var subject = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var kvp = Request.Cookies.SingleOrDefault(c => c.Key == sessionCookieName);
            if(!string.IsNullOrWhiteSpace(kvp.Key))
            {
                var activeSession = await _userSessionRepository.GetById(kvp.Value, prefix, cancellationToken);
                if(activeSession != null && activeSession.IsActive())
                {
                    var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
                    await _sessionHelper.Revoke(subject, activeSession, issuer, cancellationToken);
                    activeSession.State = UserSessionStates.Rejected;
                    await _userSessionRepository.SaveChanges(cancellationToken);
                }
            }

            Response.Cookies.Delete(sessionCookieName);
            await HttpContext.SignOutAsync();
            await _busControl.Publish(new UserLogoutSuccessEvent
            {
                UserName = nameIdentifier,
                Realm = prefix
            });
            return RedirectToAction("Index");
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
    }
}
