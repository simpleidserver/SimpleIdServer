// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.FastFed;
using SimpleIdServer.FastFed.ApplicationProvider;
using SimpleIdServer.FastFed.ApplicationProvider.Resolvers;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using SimpleIdServer.Webfinger.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddFastFedApplicationProvider(this FastFedServicesBuilder builder, Action<ProviderStoreChooser> cbChooser, Action<FastFedApplicationProviderOptions> callback = null, Action<IBusRegistrationConfigurator> massTransitOptions = null)
    {
        if (callback == null) builder.Services.Configure<FastFedApplicationProviderOptions>(o => { });
        else builder.Services.Configure(callback);
        builder.Services.AddTransient<IFastFedService, FastFedService>();
        builder.Services.AddTransient<IWebfingerClientFactory, WebfingerClientFactory>();
        builder.Services.AddTransient<IWebfingerUrlResolver, WebfingerUrlResolver>();
        builder.Services.AddMassTransit(o =>
        {
            if (massTransitOptions != null) massTransitOptions(o);
            else
            {
                o.UsingInMemory();
            }
        });
        cbChooser(new ProviderStoreChooser(builder.Services));
        return builder;
    }
}
