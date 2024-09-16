// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed.Models;

public class IdentityProviderFederationCapabilities
{
    public string Id { get; set; }
    public List<string> AuthenticationProfiles { get; set; }
    public List<string> ProvisioningProfiles { get; set; }
    public double ExpirationDateTime { get; set; }
    public IdentityProviderStatus Status { get; set; }
    public DateTime CreateDateTime { get; set; }
    public List<CapabilitySettings> Configurations { get; set; }

    public bool IsExpired
    {
        get
        {
            return Status == IdentityProviderStatus.WHITELISTED && DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= ExpirationDateTime;
        }
    }
}
