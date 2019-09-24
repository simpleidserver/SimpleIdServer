using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests.Middlewares
{
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationHandlerOptions>
    {
        private readonly ScenarioContext _scenarioContext;

        public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ScenarioContext scenarioContext) : base(options, logger, encoder, clock)
        {
            _scenarioContext = scenarioContext;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_scenarioContext.ContainsKey("anonymous"))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Thierry Habart")
            };
            var authInstant = new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString());
            var nameIdentifier = new Claim(ClaimTypes.NameIdentifier, "administrator");
            if (_scenarioContext.ContainsKey("authInstant"))
            {
                authInstant = new Claim(ClaimTypes.AuthenticationInstant, (_scenarioContext.Get<DateTime>("authInstant")).ToString());
            }

            if (_scenarioContext.ContainsKey("nameIdentifier"))
            {
                nameIdentifier = new Claim(ClaimTypes.NameIdentifier, _scenarioContext.Get<string>("nameIdentifier"));
            }

            claims.Add(authInstant);
            claims.Add(nameIdentifier);
            if (!claims.Any())
            {
                return Task.FromResult(AuthenticateResult.NoResult());
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
}
