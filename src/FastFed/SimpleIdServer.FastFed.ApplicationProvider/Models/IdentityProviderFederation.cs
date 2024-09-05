// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed.ApplicationProvider.Models;

public class IdentityProviderFederation
{
    public string EntityId { get; set; } = null!;
    public string JwksUri { get; set; } = null!;
    public List<string> AuthenticationProfiles { get; set; }
    public List<string> ProvisioningProfiles { get; set; }
    /// <summary>
    /// After which the whitelisting will be considered expired. I
    /// </summary>
    public double ExpirationDateTime { get; set; }
    public IdentityProviderStatus Status { get; set; }
    public DateTime CreateDateTime { get; set; }

    public bool IsExpired
    {
        get
        {
            return Status == IdentityProviderStatus.WHITELISTED && DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= ExpirationDateTime;
        }
    }

    public void Whitelist(string jwksUri, List<string> authenticationProfiles, List<string> provisioningProfiles, double expirationDateTime)
    {
        JwksUri = jwksUri;
        AuthenticationProfiles = authenticationProfiles;
        ProvisioningProfiles = provisioningProfiles;
        ExpirationDateTime = expirationDateTime;
    }
}