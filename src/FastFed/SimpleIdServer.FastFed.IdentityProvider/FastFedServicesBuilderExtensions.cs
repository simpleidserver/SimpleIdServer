﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed;
using SimpleIdServer.FastFed.IdentityProvider;
using SimpleIdServer.FastFed.IdentityProvider.Services;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddFastFedIdentityProvider(this FastFedServicesBuilder builder, Action<ProviderStoreChooser> cbChooser = null, Action<FastFedIdentityProviderOptions> callback = null)
    {
        if (callback == null) builder.Services.Configure<FastFedIdentityProviderOptions>(o => { });
        else builder.Services.Configure(callback);
        if (cbChooser != null) cbChooser(new ProviderStoreChooser(builder.Services));
        builder.Services.AddTransient<IFastFedService, FastFedService>();
        return builder;
    }
}