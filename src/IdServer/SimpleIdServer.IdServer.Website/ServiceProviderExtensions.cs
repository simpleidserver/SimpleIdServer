// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Website.Infrastructures;

namespace System;

public static class ServiceProviderExtensions
{
    public static IServiceProvider AddSIDWebsite(this IServiceProvider serviceProvider)
    {
        RealmRouter._serviceProvider = serviceProvider;
        return serviceProvider;
    }
}