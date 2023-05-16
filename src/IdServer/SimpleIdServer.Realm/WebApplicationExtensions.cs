// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Realm.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRealm(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<RealmMiddleware>();
            return webApplication;
        }
    }
}
