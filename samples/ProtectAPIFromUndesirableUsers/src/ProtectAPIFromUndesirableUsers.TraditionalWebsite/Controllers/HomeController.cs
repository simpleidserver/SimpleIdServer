// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProtectAPIFromUndesirableUsers.TraditionalWebsite.ViewModels;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jws;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProtectAPIFromUndesirableUsers.TraditionalWebsite.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<string, string> MAPPING_JWT_TO_CLAIM = new Dictionary<string, string>
        {
            { Constants.UserClaims.Subject, ClaimTypes.NameIdentifier },
            { Constants.UserClaims.Name, ClaimTypes.Name },
            { Constants.UserClaims.UniqueName, ClaimTypes.Name }
        };
        private readonly IAuthenticationService _authenticationService;

        public HomeController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Authenticate()
        {
            return View(new AuthenticateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticateViewModel authenticateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(authenticateViewModel);
            }

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{TraditionalWebsiteConstants.BASE_OPENID_URL}/token"),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", TraditionalWebsiteConstants.CLIENT_ID },
                        { "client_secret", TraditionalWebsiteConstants.CLIENT_SECRET },
                        { "grant_type", "password" },
                        { "username", authenticateViewModel.Login },
                        { "password", authenticateViewModel.Password },
                        { "scope", "openid profile" }
                    })
                };
                var httpResult = await httpClient.SendAsync(request);
                if (!httpResult.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("invalid_credentials", "Bad credentials");
                    return View(authenticateViewModel);
                }

                var json = await httpResult.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(json);
                var jwsGeneratorFactory = new JwsGeneratorFactory();
                var idToken = jObj["id_token"].ToString();
                var jwsPayload = jwsGeneratorFactory.BuildJwsGenerator().ExtractPayload(idToken);
                var claimsPrincipal = BuildClaimsPrincipal(jwsPayload);
                var tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = "id_token", Value = idToken }
                };
                var authProperties = new AuthenticationProperties();
                authProperties.StoreTokens(tokens);
                await HttpContext.SignInAsync(claimsPrincipal, authProperties);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var redirectUrl = Url.Action("LoginCallback", "Home", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, properties);
        }

        [HttpGet]
        public IActionResult LoginCallback(string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Disconnect()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        private static ClaimsPrincipal BuildClaimsPrincipal(JwsPayload jwsPayload)
        {
            var claims = new List<Claim>();
            foreach (var kvp in jwsPayload)
            {
                if (MAPPING_JWT_TO_CLAIM.ContainsKey(kvp.Key))
                {
                    claims.Add(new Claim(MAPPING_JWT_TO_CLAIM[kvp.Key], kvp.Value.ToString()));
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, "pwd");
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}