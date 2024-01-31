// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.Domains;
using System;

namespace SimpleIdServer.CredentialIssuer.Builders;

public class CredentialConfigurationClaimBuilder
{
    private readonly CredentialConfigurationClaim _credentialConfigurationClaim;

    private CredentialConfigurationClaimBuilder(CredentialConfigurationClaim credentialConfigurationClaim)
    {
        _credentialConfigurationClaim = credentialConfigurationClaim;
    }

    internal static CredentialConfigurationClaimBuilder New(string name, string sourceUserClaimName)
    {
        return new CredentialConfigurationClaimBuilder(new CredentialConfigurationClaim
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            SourceUserClaimName = sourceUserClaimName
        });
    }

    public CredentialConfigurationClaimBuilder AddTranslation(string name, string locale)
    {
        _credentialConfigurationClaim.Translations.Add(new CredentialConfigurationTranslation
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Locale = locale
        });
        return this; 
    }

    public CredentialConfigurationClaim Build() => _credentialConfigurationClaim;
}
