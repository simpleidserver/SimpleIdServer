// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.IdServer.UI
{
    public class HomeController : Controller
    {
        private readonly IdServerHostOptions _options;

        public HomeController(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Index() => View();

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
            var scheme = "oidc";
            var returnUrl = Url.Action(nameof(Index));
            var items = new Dictionary<string, string>
            {
                { "scheme", scheme },
                { "returnUrl", returnUrl }
            };
            var props = new AuthenticationProperties(items)
            {
                RedirectUri = returnUrl
            };
            return Challenge(props, scheme);
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
