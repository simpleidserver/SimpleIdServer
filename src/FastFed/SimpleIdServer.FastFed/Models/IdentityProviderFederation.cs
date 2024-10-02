// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.FastFed.Models;

public class IdentityProviderFederation
{
    public string EntityId { get; set; } = null!;
    public string ProviderMetadataUrl { get; set; } = null;
    public string JwksUri { get; set; } = null!;
    public string DisplayName { get; set; }
    public string IconUri { get; set; }
    public string LogoUri { get; set; }
    public string License { get; set; }
    public DateTime CreateDateTime { get; set; }
    public List<IdentityProviderFederationCapabilities> Capabilities { get; set; }
    public IdentityProviderFederationCapabilities LastCapabilities
    {
        get
        {
            return Capabilities.OrderByDescending(c => c.CreateDateTime).FirstOrDefault();
        }
    }
}