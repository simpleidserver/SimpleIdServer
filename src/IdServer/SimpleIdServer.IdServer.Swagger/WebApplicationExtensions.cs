// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseSIDSwagger(this WebApplication app, SwaggerOptions options)
    {
        app.UseMiddleware<SIDSwaggerMiddleware>(options);
        return app;
    }

    public static WebApplication UseSIDSwagger(
        this WebApplication app,
        Action<SwaggerOptions> setupAction = null)
    {
        SwaggerOptions options;
        using (var scope = app.Services.CreateScope())
        {
            options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerOptions>>().Value;
            setupAction?.Invoke(options);
        }

        return app.UseSIDSwagger(options);
    }

    public static WebApplication UseSIDSwaggerUI(this WebApplication app, Action<SwaggerUIOptions> setupAction = null)
    {
        SwaggerUIOptions options;
        using (var scope = app.Services.CreateScope())
        {
            options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerUIOptions>>().Value;
            setupAction?.Invoke(options);
        }

        // To simplify the common case, use a default that will work with the SwaggerMiddleware defaults
        if (options.ConfigObject.Urls == null)
        {
            var hostingEnv = app.Services.GetRequiredService<IWebHostEnvironment>();
            options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{hostingEnv.ApplicationName} v1", Url = "v1/swagger.json" } };
        }

        return app.UseSIDSwaggerUI(options);
    }

    public static WebApplication UseSIDSwaggerUI(this WebApplication app, SwaggerUIOptions options)
    {
        app.UseMiddleware<SidSwaggerUIMiddleware>(options);
        return app;
    }
}
