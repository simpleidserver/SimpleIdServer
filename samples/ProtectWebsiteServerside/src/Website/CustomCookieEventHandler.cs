using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;

namespace Website
{
    public class CustomCookieEventHandler : CookieAuthenticationEvents
    {
        private readonly IDistributedCache _distributedCache;

        public CustomCookieEventHandler(IDistributedCache distributedCache) 
        { 
            _distributedCache = distributedCache;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.Principal.Identity.IsAuthenticated)
            {
                var subject = context.Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var sessionId = context.Principal.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
                var str = await _distributedCache.GetStringAsync($"{subject}_{sessionId}");
                if(!string.IsNullOrWhiteSpace(str))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }
    }
}
