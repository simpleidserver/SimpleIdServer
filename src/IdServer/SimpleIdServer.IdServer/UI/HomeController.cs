// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using QRCoder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.AuthProviders;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUmaPendingRequestRepository _pendingRequestRepository;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<IdServerHostOptions> options, IUserRepository userRepository, IClientRepository clientRepository, 
            IUmaPendingRequestRepository pendingRequestRepository, IAuthenticationSchemeProvider authenticationSchemeProvider,
            ILogger<HomeController> logger)
        {
            _options = options.Value;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _pendingRequestRepository = pendingRequestRepository;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.Query().Include(u => u.Consents).Include(u => u.ExternalAuthProviders).FirstOrDefaultAsync(u => u.Name == nameIdentifier, cancellationToken);
            var consents = await GetConsents();
            var pendingRequests = await GetPendingRequest();
            var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
            return View(new ProfileViewModel
            {
                Name = user.Name,
                HasOtpKey = !string.IsNullOrWhiteSpace(user.OTPKey),
                Consents = consents,
                PendingRequests = pendingRequests,
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
                var clientIds = user.Consents.Select(c => c.ClientId);
                var oauthClients = await _clientRepository.Query().Include(c => c.Translations).AsNoTracking().Where(c => clientIds.Contains(c.ClientId)).ToListAsync(cancellationToken);
                foreach (var consent in user.Consents)
                {
                    var oauthClient = oauthClients.Single(c => c.ClientId == consent.ClientId);
                    consents.Add(new ConsentViewModel(
                        consent.Id,
                        oauthClient.ClientName,
                        oauthClient.ClientUri,
                        consent.Scopes,
                        consent.Claims));
                }

                return consents;
            }

            async Task<List<PendingRequestViewModel>> GetPendingRequest()
            {
                var pendingRequestLst = await _pendingRequestRepository.Query().Include(p => p.Resource).ThenInclude(p => p.Translations).Where(r => r.Owner == nameIdentifier || r.Requester == nameIdentifier).ToListAsync(cancellationToken);
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
        public async Task<IActionResult> RejectConsent(string consentId, CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.Query().Include(u => u.Consents).FirstAsync(c => c.Name == nameIdentifier, cancellationToken);
            if (!user.HasOpenIDConsent(consentId))
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
        public async Task<IActionResult> LinkCallback(CancellationToken cancellationToken)
        {
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

                if (await _userRepository.Query().Include(u => u.ExternalAuthProviders).AnyAsync(u => u.ExternalAuthProviders.Any(p => p.Subject == sub && p.Scheme == scheme), cancellationToken))
                {
                    _logger.LogError("a local account has already been linked to the external authentication provider {authProviderScheme}", scheme);
                    await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
                    return RedirectToAction("Index", "Errors", new { code = ErrorCodes.INVALID_REQUEST, message = "a local account has already been linked to the external authentication provider" });
                }

                var user = await _userRepository.Query()
                    .Include(u => u.ExternalAuthProviders)
                    .FirstAsync(u => u.Name == nameIdentifier, cancellationToken);
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
        public async Task<IActionResult> Unlink(UnlinkProfileViewModel viewModel, CancellationToken cancellationToken)
        {
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", message = "Request cannot be empty" });

            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.Query()
                .Include(u => u.ExternalAuthProviders)
                .FirstAsync(u => u.Name == nameIdentifier, cancellationToken);
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
        public async Task<IActionResult> GetOTP(CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Name == nameIdentifier, cancellationToken);
            if (string.IsNullOrWhiteSpace(user.OTPKey)) return new NoContentResult();
            var alg = Enum.GetName(typeof(OTPAlgs), _options.OTPAlg).ToLowerInvariant();
            var url = $"otpauth://{alg}/{_options.OTPIssuer}:{user.Name}?secret={user.OTPKey}&issuer={_options.OTPIssuer}&algorithm=SHA1";
            if (_options.OTPAlg == OTPAlgs.HOTP)
                url = $"{url}&counter={user.OTPCounter}";
            if(_options.OTPAlg == OTPAlgs.TOTP)
            {
                url = $"{url}&period={_options.TOTPStep}";
            }

            var bitmap = GetQRCode();
            return GetImage(bitmap);

            Bitmap GetQRCode()
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                return qrCode.GetGraphic(20);
            }

            IActionResult GetImage(Bitmap result)
            {
                byte[] payload = null;
                using (var stream = new MemoryStream())
                {
                    result.Save(stream, ImageFormat.Png);
                    payload = stream.ToArray();
                }

                return File(payload, "image/png");
            }
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
        public async Task<IActionResult> Disconnect()
        {
            Response.Cookies.Delete(_options.SessionCookieName);
            await HttpContext.SignOutAsync();
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
