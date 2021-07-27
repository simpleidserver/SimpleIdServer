using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI.AuthProviders
{
    public class DynamicAuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationHandlerProvider _handlers;

        public DynamicAuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers)
        {
            _handlers = handlers;
        }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public async Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            if (scheme == null)
            {
            }

            var handler = await _handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                return;
            }

            await handler.ChallengeAsync(properties);
        }

        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }

        public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }
    }
}
