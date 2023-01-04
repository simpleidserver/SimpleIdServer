// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Middlewares
{
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationHandlerOptions>
    {
        public const string AuthenticationScheme = "Test";
        private readonly CustomAuthenticationHandlerOptions _options;

        public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Options.ScenarioContext.IsUserAuthenticationEnabled())
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user"),
                new Claim(ClaimTypes.Name, "user")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationTicket = new AuthenticationTicket(
                                             claimsPrincipal,
                                             new AuthenticationProperties(),
                                             CookieAuthenticationDefaults.AuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }

    public class CustomAuthenticationHandlerOptions : AuthenticationSchemeOptions
    {
        public ScenarioContext ScenarioContext { get; set; }
    }
}
