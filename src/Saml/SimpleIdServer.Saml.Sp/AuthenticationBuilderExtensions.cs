// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Sp;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddSamlSp(this AuthenticationBuilder builder) 
            => builder.AddSamlSp(SamlSpDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddSamlSp(this AuthenticationBuilder builder, Action<SamlSpOptions> configureOptions)
           => builder.AddSamlSp(SamlSpDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddSamlSp(this AuthenticationBuilder builder, string authenticationScheme, Action<SamlSpOptions> configureOptions)
            => builder.AddSamlSp(authenticationScheme, SamlSpDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddSamlSp(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<SamlSpOptions> configureOptions)
        {
            if (configureOptions != null)
            {
                builder.Services.Configure(configureOptions);
            }
            else
            {
                builder.Services.Configure<SamlSpOptions>((opt) => { });
            }

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SamlSpOptions>, SamlSpPostConfigureOptions>());
            return builder.AddRemoteScheme<SamlSpOptions, SamlSpHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
