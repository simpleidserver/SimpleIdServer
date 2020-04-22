// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddDefaultSCIMAuthorizationPolicy(this AuthorizationOptions opts)
        {
            opts.AddPolicy("QueryScimResource", p => p.RequireAssertion(t => true));
            opts.AddPolicy("AddScimResource", p => p.RequireAssertion(t => true));
            opts.AddPolicy("DeleteScimResource", p => p.RequireAssertion(t => true));
            opts.AddPolicy("UpdateScimResource", p => p.RequireAssertion(t => true));
            opts.AddPolicy("BulkScimResource", p => p.RequireAssertion(t => true));
            opts.AddPolicy("UserAuthenticated", p => p.RequireAssertion(t => true));

            /*
            opts.AddPolicy("QueryScimResource", p => p.RequireClaim("scope", "query_scim_resource"));
            opts.AddPolicy("AddScimResource", p => p.RequireClaim("scope", "add_scim_resource"));
            opts.AddPolicy("DeleteScimResource", p => p.RequireClaim("scope", "delete_scim_resource"));
            opts.AddPolicy("UpdateScimResource", p => p.RequireClaim("scope", "update_scim_resource"));
            opts.AddPolicy("BulkScimResource", p => p.RequireClaim("scope", "bulk_scim_resource"));
            opts.AddPolicy("UserAuthenticated", p => p.RequireAuthenticatedUser());
            */
        }
    }
}