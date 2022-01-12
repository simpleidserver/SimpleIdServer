using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationOptionsException
    {
        public static void AddDefaultUMAAuthoriztionPolicy(this AuthorizationOptions opts)
        {
            opts.AddDefaultOAUTHAuthorizationPolicy();
            opts.AddPolicy("ManageResources", p => p.RequireClaim("scope", "manage_resources"));
            opts.AddPolicy("ManageRequests", p => p.RequireClaim("scope", "manage_requests"));
        }
    }
}
