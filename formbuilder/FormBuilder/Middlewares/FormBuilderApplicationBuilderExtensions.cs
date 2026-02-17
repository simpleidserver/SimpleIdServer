// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Middlewares;

namespace Microsoft.AspNetCore.Builder;

public static class FormBuilderApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFormBuilder(this IApplicationBuilder app)
    {
        app.UseMiddleware<HttpRequestStateMiddleware>();
        return app;
    }
}
