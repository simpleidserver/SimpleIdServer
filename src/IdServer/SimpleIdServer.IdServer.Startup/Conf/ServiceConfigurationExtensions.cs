// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Startup.Conf;

public static class ServiceConfigurationExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
    }
}
