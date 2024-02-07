// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.Domains;
using System;

namespace SimpleIdServer.CredentialIssuer.Builders;

public class CredentialConfigurationBuilder
{
    private readonly CredentialConfiguration _credentialConfiguration;

    private CredentialConfigurationBuilder(CredentialConfiguration credentialConfiguration)
    {
        _credentialConfiguration = credentialConfiguration;
    }

    public static CredentialConfigurationBuilder New(
        string format, 
        string type, 
        string jsonLdContext, 
        string baseUrl,
        string scope = null,
        string id = null)
    {
        return new CredentialConfigurationBuilder(new CredentialConfiguration
        {
            Id = id ?? Guid.NewGuid().ToString(),
            ServerId = $"{type}_{format}",
            Format = format,
            Type = type,
            JsonLdContext = jsonLdContext,
            BaseUrl = baseUrl,
            Scope = scope,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow,
        });
    }

    public CredentialConfigurationBuilder AddDisplay(
        string name,
        string locale,
        string logoUrl = null,
        string logoAltText = null,
        string description = null,
        string backgroundColor = null,
        string textColor = null)
    {
        _credentialConfiguration.Displays.Add(new CredentialConfigurationTranslation
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Locale = locale,
            LogoUrl = logoUrl,
            LogoAltText = logoAltText,
            Description = description,
            BackgroundColor = backgroundColor,
            TextColor = textColor

        });
        return this;
    }

    public CredentialConfigurationBuilder AddClaim(
        string name,
        string sourceUserClaimName,
        Action<CredentialConfigurationClaimBuilder> callback = null)
    {
        var builder = CredentialConfigurationClaimBuilder.New(name, sourceUserClaimName);
        if (callback != null) callback(builder);
        _credentialConfiguration.Claims.Add(builder.Build());
        return this;
    }

    public CredentialConfiguration Build() => _credentialConfiguration;
}
