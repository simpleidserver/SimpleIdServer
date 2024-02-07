// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests.Middlewares;

public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationHandlerOptions>
{
    public const string AuthenticationScheme = "Test";

    public CustomAuthenticationHandler(
        IOptionsMonitor<CustomAuthenticationHandlerOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "user"),
            new Claim(ClaimTypes.Name, "user"),
            new Claim("sub", "user"),
            new Claim("scope", "university_degree")
        };
        if(Options.ScenarioContext.ContainsKey("credentialIdentifier"))
        {
            var authDetails = new JsonObject();
            var arr = new JsonArray();
            arr.Add(Options.ScenarioContext["credentialIdentifier"]);
            authDetails.Add("credential_identifiers", arr);
            claims.Add(new Claim("authorization_details", authDetails.ToJsonString()));
        }

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
