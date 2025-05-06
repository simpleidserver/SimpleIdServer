// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Website;

public class IdserverAdminBuilder
{
    private readonly WebApplicationBuilder _builder;
    private readonly IServiceCollection _services;
    private readonly IDataProtectionBuilder _dataprotectionBuilder;
    private readonly AdminOpenidAuth _adminOpenidAuth;
    private readonly AdminCookieAuth _adminCookieAuth;
    private readonly AdminAuthz _adminAuthz;

    internal IdserverAdminBuilder(WebApplicationBuilder builder, IServiceCollection services, IDataProtectionBuilder dataprotectionBuilder, AdminOpenidAuth adminOpenidAuth, AdminCookieAuth adminCookieAuth, AdminAuthz adminAuthz)
    {
        _builder = builder;
        _services = services;
        _dataprotectionBuilder = dataprotectionBuilder;
        _adminOpenidAuth = adminOpenidAuth;
        _adminCookieAuth = adminCookieAuth;
        _adminAuthz = adminAuthz;
    }

    internal WebApplicationBuilder Builder => _builder;

    /// <summary>
    /// Configures the data protection system to persist keys in a specific directory on the file system.
    /// This ensures that encryption keys are preserved between application restarts.
    /// </summary>
    /// <param name="directoryPath">The absolute path to the directory where data protection keys will be stored</param>
    /// <returns>The IdserverAdminBuilder instance for method chaining</returns>
    public IdserverAdminBuilder PersistDataprotection(string directoryPath)
    {
        _dataprotectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(directoryPath));
        return this;
    }

    /// <summary>
    /// Forces HTTPS for all communications with the IdServer admin interface.
    /// When enabled, all HTTP requests will be automatically redirected to HTTPS.
    /// This improves security by ensuring encrypted communication.
    /// </summary>
    /// <returns>The IdserverAdminBuilder instance for method chaining.</returns>
    public IdserverAdminBuilder ForceHttps()
    {
        _services.Configure<IdServerWebsiteOptions>(o =>
        {
            o.ForceHttps = true;
        });
        return this;
    }

    /// <summary>
    /// Enables realm mode for the IdServer admin interface.
    /// When realm mode is enabled, admins can only manage resources assigned to their realm.
    /// This creates a logical separation between different sets of resources.
    /// </summary>
    /// <returns>The IdserverAdminBuilder instance for chaining.</returns>

    public IdserverAdminBuilder EnableRealm()
    {
        _adminOpenidAuth.UseRealm = true;
        _services.Configure<IdServerWebsiteOptions>(o =>
        {
            o.IsReamEnabled = true;
        });
        return this;
    }

    /// <summary>
    /// Updates the name of the authentication cookie used by the admin interface.
    /// This allows customizing the cookie identifier for better tracking and management.
    /// </summary>
    /// <param name="cookieName">The new name to be used for the authentication cookie</param>
    /// <returns>The IdserverAdminBuilder instance for method chaining</returns>
    public IdserverAdminBuilder UpdateCookieName(string cookieName)
    {
        _adminCookieAuth.CookieName = cookieName;
        return this;
    }

    /// <summary>
    /// Updates the OpenID Connect authentication configuration for the admin interface.
    /// </summary>
    /// <param name="clientId">The client identifier registered with the OpenID Connect provider. If null or empty, keeps existing value.</param>
    /// <param name="clientSecret">The client secret for authentication with the OpenID Connect provider. If null or empty, keeps existing value.</param>
    /// <param name="scopes">List of OAuth scopes to request. If null, defaults to ["openid", "profile", "role"].</param>
    /// <param name="IgnoreCertificateError">When true, certificate validation errors will be ignored. Default is false.</param>
    /// <returns>The IdserverAdminBuilder instance for method chaining.</returns>
    public IdserverAdminBuilder UpdateOpenid(string clientId, string clientSecret, List<string> scopes, bool IgnoreCertificateError = false)
    {
        _services.Configure<IdServerWebsiteOptions>(o =>
        {
            o.ClientId = clientId;
            o.ClientSecret = clientSecret;
            o.IgnoreCertificateError = IgnoreCertificateError;
        });
        scopes = scopes ?? new List<string> { "openid", "profile", "role" };
        _adminOpenidAuth.ClientId = clientId;
        _adminOpenidAuth.ClientSecret = clientSecret;
        _adminOpenidAuth.Scopes = scopes;
        _adminOpenidAuth.IgnoreCertificateError = IgnoreCertificateError;
        return this;
    }

    /// <summary>
    /// Updates the list of roles required for accessing the admin interface.
    /// Users must have at least one of these roles to be authorized.
    /// </summary>
    /// <param name="roles">List of role names that grant access to the admin interface</param>
    /// <returns>The IdserverAdminBuilder instance for method chaining</returns>
    public IdserverAdminBuilder UpdateAuthzRequiredRoles(List<string> roles)
    {
        _adminAuthz.RequiredRoles = roles;
        return this;
    }
}
