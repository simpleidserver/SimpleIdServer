// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.IdServer.Federation;
using SimpleIdServer.IdServer.Federation.Builders;
using SimpleIdServer.IdServer.Federation.Helpers;
using SimpleIdServer.IdServer.Helpers;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdBuilderExtensions
{
    public static IdServerBuilder AddOpenidFederation(this IdServerBuilder builder, Action<OpenidFederationOptions> cb = null)
    {
        if (cb != null) builder.Services.Configure(cb);
        else builder.Services.Configure(cb);
        builder.Services.AddTransient<IOpenidProviderFederationEntityBuilder, OpenidProviderFederationEntityBuilder>();
        builder.Services.RemoveAll<IClientHelper>();
        builder.Services.AddTransient<IClientHelper, FederationClientHelper>();
        return builder;
    }
}
