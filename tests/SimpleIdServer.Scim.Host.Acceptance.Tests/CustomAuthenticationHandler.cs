using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new Claim("scope", "query_scim_resource"),
                new Claim("scope", "add_scim_resource"),
                new Claim("scope", "delete_scim_resource"),
                new Claim("scope", "update_scim_resource"),
                new Claim("scope", "bulk_scim_resource")
            };
            var claimsIdentity = new ClaimsIdentity(claims, SCIMConstants.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationTicket = new AuthenticationTicket(
                                             claimsPrincipal,
                                             new AuthenticationProperties(),
                                             SCIMConstants.AuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }
}
