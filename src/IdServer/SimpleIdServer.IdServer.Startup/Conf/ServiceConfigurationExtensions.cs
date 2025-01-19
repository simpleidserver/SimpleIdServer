// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Startup.Conf;

public static class ServiceConfigurationExtensions
{
    public static void ConfigureKestrel(this IServiceCollection services, IdentityServerConfiguration config)
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureHttpsDefaults(o =>
            {
                o.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                if (config.ClientCertificateMode != null) o.ClientCertificateMode = config.ClientCertificateMode.Value;
            });
        });
    }

    public static void ConfigureForwardedHeaders(this IServiceCollection services, IdentityServerConfiguration config)
    {
        if (config.IsForwardedEnabled)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }
    }
    
    public static void ConfigureClientCertificateForwarding(this IServiceCollection services, IdentityServerConfiguration config)
    {
        if (config.IsClientCertificateForwarded)
        {
            services.AddCertificateForwarding(options =>
            {
                options.CertificateHeader = "ssl-client-cert";
                options.HeaderConverter = (headerValue) =>
                {
                    System.Console.WriteLine(headerValue);
                    X509Certificate2? clientCertificate = null;

                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        clientCertificate = X509Certificate2.CreateFromPem(WebUtility.UrlDecode(headerValue));
                    }

                    return clientCertificate!;
                };
            });
        }
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));
    }

    public static void ConfigureRazorAndLocalization(this IServiceCollection services)
    {
        services.AddRazorPages()
            .AddRazorRuntimeCompilation();
        services.AddLocalization();
    }
}
