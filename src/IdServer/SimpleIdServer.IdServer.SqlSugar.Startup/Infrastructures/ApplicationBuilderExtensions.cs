// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.SqlSugar.Startup.Infrastructures;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder SetHttpsScheme(this IApplicationBuilder app)
    {
        return app.UseMiddleware<HttpsMiddleware>();
    }
}
