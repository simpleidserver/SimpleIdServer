using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests.Middlewares
{
    public class CustomAuthenticationHandlerOptions : AuthenticationSchemeOptions
    {
        public string LoginPath { get; set; }
    }
}