// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAutomaticConfiguration(this WebApplicationBuilder builder, Action<AutomaticConfigurationOptions> callback)
    {
        var options = new AutomaticConfigurationOptions();
        callback(options);
        builder.Services.AddSingleton(options);
        var configurationBuilder = (IConfigurationBuilder)builder.Configuration;
        configurationBuilder.Add(new AutomaticConfigurationSource(options, options.KeyValueConnector));
        return builder;
    }
}
