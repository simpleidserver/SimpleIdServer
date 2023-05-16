// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCredentialIssuer(this IServiceCollection services, Action<CredentialIssuerOptions> callback)
        {
            if (callback != null) services.Configure(callback);
            else services.Configure<CredentialIssuerOptions>(o => { });
            return services;
        }
    }
}
