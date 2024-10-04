// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml.Sid;

public class FastFedSidSamlAuthenticationOptions
{
    public string SidBaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
