// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Swagger;
using Swashbuckle.AspNetCore.ReDoc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the application to use the SID Swagger middleware with the provided SwaggerOptions.
    /// </summary>
    public static WebApplication UseSidSwagger(this WebApplication app, SwaggerOptions options)
    {
        app.UseMiddleware<SIDSwaggerMiddleware>(options);
        return app;
    }

    /// <summary>
    /// Configures the application to use the SID Swagger middleware using a setup action for SwaggerOptions.
    /// </summary>
    public static WebApplication UseSidSwagger(this WebApplication app, Action<SwaggerOptions> setupAction = null)
    {
        SwaggerOptions options;
        using (var scope = app.Services.CreateScope())
        {
            options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerOptions>>().Value;
            setupAction?.Invoke(options);
        }

        return app.UseSidSwagger(options);
    }

    /// <summary>
    /// Configures the application to use the SID Swagger UI middleware with the specified SwaggerUIOptions.
    /// </summary>
    public static WebApplication UseSidSwaggerUi(this WebApplication app, SwaggerUIOptions options)
    {
        app.UseMiddleware<SidSwaggerUIMiddleware>(options);
        return app;
    }

    /// <summary>
    /// Configures the application to use the SID Swagger UI middleware using a setup action for SwaggerUIOptions.
    /// </summary>
    public static WebApplication UseSidSwaggerUi(this WebApplication app, Action<SwaggerUIOptions> setupAction = null)
    {
        SwaggerUIOptions options;
        using (var scope = app.Services.CreateScope())
        {
            options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerUIOptions>>().Value;
            setupAction?.Invoke(options);
        }

        if (options.ConfigObject.Urls == null)
        {
            var hostingEnv = app.Services.GetRequiredService<IWebHostEnvironment>();
            options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{hostingEnv.ApplicationName} v1", Url = "v1/swagger.json" } };
        }

        if (string.IsNullOrWhiteSpace(options.OAuthConfigObject.ClientId))
        {
            options.OAuthClientId("swaggerClient");
        }

        if (string.IsNullOrWhiteSpace(options.OAuthConfigObject.ClientSecret))
        {
            options.OAuthClientSecret("password");
        }

        return app.UseSidSwaggerUi(options);
    }

    /// <summary>
    /// Configures the application to use the SID ReDoc middleware with the specified ReDocOptions.
    /// </summary>
    public static WebApplication UseSidRedoc(this WebApplication app, ReDocOptions options)
    {
        app.UseMiddleware<SIDReDocMiddleware>(options);
        return app;
    }

    /// <summary>
    /// Configures the application to use the SID ReDoc middleware using a setup action for ReDocOptions.
    /// </summary>
    public static WebApplication UseSidRedoc(this WebApplication app, Action<ReDocOptions> setupAction = null)
    {
        ReDocOptions options;
        using (var scope = app.Services.CreateScope())
        {
            options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ReDocOptions>>().Value;
            setupAction?.Invoke(options);
        }

        if (options.SpecUrl == null)
        {
            options.SpecUrl = "../swagger/v1/swagger.json";
        }

        app.UseSidRedoc(options);
        return app;
    }
}
