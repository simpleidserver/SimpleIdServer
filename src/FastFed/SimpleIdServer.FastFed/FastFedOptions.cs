// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.FastFed;

public class FastFedOptions
{
    public string ProviderDomain { get; set; }
    public IdProviderOptions IdProvider { get; set; }
    public AppProviderOptions AppProvider { get; set; }
}

public class AppProviderOptions
{
    public Domains.Capabilities Capabilities { get; set; }
    public Domains.DisplaySettings DisplaySettings { get; set; }
    public Domains.ProviderContactInformation ContactInformation { get; set; }
}

public class IdProviderOptions
{
    public Domains.Capabilities Capabilities { get; set; }
    public Domains.DisplaySettings DisplaySettings { get; set; }
    public Domains.ProviderContactInformation ContactInformation { get; set; }
    public string JwksUri { get; set; }
}