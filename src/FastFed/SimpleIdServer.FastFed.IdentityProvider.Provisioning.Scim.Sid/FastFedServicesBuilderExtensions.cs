// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.Sid;
using System;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddSidScimProvisioning(this FastFedServicesBuilder builder, Action<IBusRegistrationConfigurator> massTransitOptions = null)
    {
        builder.Services.AddMassTransit((o) =>
        {
            o.AddConsumer<SidIntegrationEventsConsumer>();
            if (massTransitOptions != null)
            {
                massTransitOptions(o);
            }
            else
            {
                o.UsingInMemory();
            }
        });
        return builder;
    }
}
