// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleIdServer.OpenId;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomOpenIdConnectExtensions
    {
        /// <summary>
        /// Adds OpenId Connect authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
        /// The default scheme is specified by <see cref="OpenIdConnectDefaults.AuthenticationScheme"/>.
        /// <para>
        /// OpenID Connect is an identity layer on top of the OAuth 2.0 protocol. It allows clients
        /// to request and receive information about authenticated sessions and end-users.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCustomOpenIdConnect(this AuthenticationBuilder builder)
            => builder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Adds OpenId Connect authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
        /// The default scheme is specified by <see cref="OpenIdConnectDefaults.AuthenticationScheme"/>.
        /// <para>
        /// OpenID Connect is an identity layer on top of the OAuth 2.0 protocol. It allows clients
        /// to request and receive information about authenticated sessions and end-users.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="OpenIdConnectOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCustomOpenIdConnect(this AuthenticationBuilder builder, Action<CustomOpenIdConnectOptions> configureOptions)
            => builder.AddCustomOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Adds OpenId Connect authentication to <see cref="AuthenticationBuilder"/> using the specified scheme.
        /// <para>
        /// OpenID Connect is an identity layer on top of the OAuth 2.0 protocol. It allows clients
        /// to request and receive information about authenticated sessions and end-users.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="OpenIdConnectOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCustomOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, Action<CustomOpenIdConnectOptions> configureOptions)
            => builder.AddCustomOpenIdConnect(authenticationScheme, OpenIdConnectDefaults.DisplayName, configureOptions);

        /// <summary>
        /// Adds OpenId Connect authentication to <see cref="AuthenticationBuilder"/> using the specified scheme.
        /// <para>
        /// OpenID Connect is an identity layer on top of the OAuth 2.0 protocol. It allows clients
        /// to request and receive information about authenticated sessions and end-users.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">A display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="OpenIdConnectOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCustomOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<CustomOpenIdConnectOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<CustomOpenIdConnectOptions>, CustomOpenIdConnectConfigureOptions>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CustomOpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            return builder.AddRemoteScheme<CustomOpenIdConnectOptions, CustomOpenIdConnectHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
