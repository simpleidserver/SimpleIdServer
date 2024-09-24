// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class ScimProvisioningConfiguration
{
    public AuthenticationTypes AuthenticationType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string ApiSecret { get; set; }
}

public enum AuthenticationTypes
{
    OAUTH = 0,
    APIKEY = 1
}