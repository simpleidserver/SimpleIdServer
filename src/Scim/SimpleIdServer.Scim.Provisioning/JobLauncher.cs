// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using GreenPipes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Provisioning.Consumers;
using SimpleIdServer.Scim.Provisioning.Provisioner;
using System;

namespace SimpleIdServer.Scim.Provisioning
{
    public class JobLauncher
    {
        public static void Launch(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();
            RegisterProvisioning(serviceCollection, configuration);
            RegisterMassTransit(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            StartBus(serviceProvider);
        }

        private static void RegisterProvisioning(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddScimStoreEF(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(configuration.GetConnectionString("db"));
            });
            services.AddTransient<IProvisioner, HTTPProvisioner>();
        }

        private static void RegisterMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<RepresentationAddedEventConsumer>();
                x.AddConsumer<RepresentationUpdatedEventConsumer>();
                x.AddConsumer<RepresentationRemovedEventConsumer>();
                x.AddConsumer<OpenIdUserAddedEventConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("representation-added", e =>
                    {
                        e.UseMessageRetry(r => r.Immediate(5));
                        e.ConfigureConsumer<RepresentationAddedEventConsumer>(context);
                    });
                    cfg.ReceiveEndpoint("representation-updated", e =>
                    {
                        e.UseMessageRetry(r => r.Immediate(5));
                        e.ConfigureConsumer<RepresentationUpdatedEventConsumer>(context);
                    });
                    cfg.ReceiveEndpoint("representation-removed", e =>
                    {
                        e.UseMessageRetry(r => r.Immediate(5));
                        e.ConfigureConsumer<RepresentationRemovedEventConsumer>(context);
                    });
                    cfg.ReceiveEndpoint("user-added", e =>
                    {
                        e.UseMessageRetry(r => r.Immediate(5));
                        e.ConfigureConsumer<OpenIdUserAddedEventConsumer>(context);
                    });
                });
            });
        }

        private static void StartBus(IServiceProvider serviceProvider)
        {
            var busControl = serviceProvider.GetService<IBusControl>();
            busControl.Start();
        }
    }
}
