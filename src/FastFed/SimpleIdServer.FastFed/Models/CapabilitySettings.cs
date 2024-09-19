// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.Models;

public class CapabilitySettings
{
    public string Id { get; set; }
    public string ProfileName { get; set; }
    public bool IsAuthenticationProfile { get; set; }
    public string IdProviderConfiguration { get; set; }
    public string AppProviderHandshakeRegisterConfiguration { get; set; }
    public string AppProviderConfiguration { get; set; }
    public string CapabilitiesId { get; set; }
}