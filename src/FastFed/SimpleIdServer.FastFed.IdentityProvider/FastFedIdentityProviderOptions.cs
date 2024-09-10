// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.IdentityProvider;

public class FastFedIdentityProviderOptions
{
    public string ProviderDomain { get; set; }
    public FastFed.Models.Capabilities Capabilities { get; set; }
    public FastFed.Models.DisplaySettings DisplaySettings { get; set; }
    public FastFed.Models.ProviderContactInformation ContactInformation { get; set; }
}
