using SimpleIdServer.OpenBankingApi.Infrastructure.Authorizations;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationOptionsExtensions
    {
        public static AuthorizationOptions AddOpenBankingAuthorization(this AuthorizationOptions authorizationOptions, string authenticationScheme)
        {
            authorizationOptions.AddPolicy("accounts", p =>
            {
                p.AddAuthenticationSchemes(authenticationScheme);
                p.Requirements.Add(new MtlsAccessTokenRequirement(new string[] { "accounts" }));
            });
            return authorizationOptions;
        }
    }
}
