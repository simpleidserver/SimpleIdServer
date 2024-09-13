// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs;

public class FastFedDiscoveryConfirmViewModel
{
    public string EntityId { get; set; }
    public IdentityProviderFederation IdProviderFederation { get; set; }
}