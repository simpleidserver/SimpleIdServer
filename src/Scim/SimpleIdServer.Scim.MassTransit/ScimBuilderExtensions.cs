// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using MassTransit.MessageData;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.MassTransit;
using System;
using System.Linq;

namespace SimpleIdServer.Scim;

public static class ScimBuilderExtensions
{
    public static ScimBuilder PublishLargeMessage(this ScimBuilder builder)
    {
        builder.Services.Configure<ScimMassTransitOptions>(o =>
        {
            o.IsBigMessagePublished = true;
        });
        return builder;
    }

    public static ScimBuilder ConfigureInMemoryMassTransit(this ScimBuilder builder)
    {
        CheckMassTransitIsRegistered(builder);
        var repository = new InMemoryMessageDataRepository();
        builder.Services.AddSingleton<IMessageDataRepository>(repository);
        builder.Services.AddMassTransit((o) =>
        {
            o.UsingInMemory((context, cfg) =>
            {
                cfg.UseMessageData(repository);
                cfg.ConfigureEndpoints(context);
            });
        });
        return builder;
    }

    public static ScimBuilder ConfigureMassTransit(this ScimBuilder builder, Action<IBusRegistrationConfigurator> cb)
    {
        CheckMassTransitIsRegistered(builder);
        builder.Services.AddMassTransit(x =>
        {
            cb(x);
        });
        return builder;
    }

    private static void CheckMassTransitIsRegistered(ScimBuilder builder)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IBus)))
        {
            throw new InvalidOperationException("MassTransit is already configured by the AddScim operation. To disable this configuration, set the skipMassTransitRegistration parameter to true.");
        }
    }
}
