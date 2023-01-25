// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
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

        public HomeController(IOptions<IdServerHostOptions> options, IUserRepository userRepository)
        {
            _options = options.Value;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == nameIdentifier, cancellationToken);
            return View(new ProfileViewModel
            {
                Id = user.Id,
                HasOtpKey = !string.IsNullOrWhiteSpace(user.OTPKey)
            });
        }

        [HttpGet]
        [Authorize(Constants.Policies.Authenticated)]
        public async Task<IActionResult> GetOTP(CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == nameIdentifier, cancellationToken);
            if (string.IsNullOrWhiteSpace(user.OTPKey)) return new NoContentResult();
            var alg = Enum.GetName(typeof(OTPAlgs), _options.OTPAlg).ToLowerInvariant();
            var url = $"otpauth://{alg}/{_options.OTPIssuer}:{user.Id}?secret={user.OTPKey}&issuer={_options.OTPIssuer}&algorithm=SHA1";
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
