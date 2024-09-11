// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.FastFed.Models;

public class IdentityProviderFederation
{
    public string EntityId { get; set; } = null!;
    public string JwksUri { get; set; } = null!;
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