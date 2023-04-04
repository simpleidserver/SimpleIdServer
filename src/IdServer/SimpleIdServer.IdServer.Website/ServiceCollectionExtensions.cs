// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.Website;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSIDWebsite(this IServiceCollection services, Action<IdServerWebsiteOptions>? callbackOptions = null, Action<DbContextOptionsBuilder>? action = null)
        {
            services.AddFluxor(o =>
            {
                o.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly);
                o.UseReduxDevTools(rdt =>
                {
                    rdt.Name = "SimpleIdServer";
                });
            });
            services.AddStore(action, ServiceLifetime.Transient);
            services.AddScoped<IOTPQRCodeGenerator, OTPQRCodeGenerator>();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ContextMenuService>();
            services.AddScoped<TooltipService>();
            services.AddTransient<ICertificateAuthorityStore, CertificateAuthorityStore>();
            if (callbackOptions == null) services.Configure<IdServerWebsiteOptions>((o) => { });
            else services.Configure(callbackOptions);
            return services;
        }
    }
}
