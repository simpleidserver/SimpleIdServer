// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Provisioning.Scim;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public class ScimProvisioningOptions
{
    public string ScimServiceUri { get; set; }
    public string Scope { get; set; }
    public string TokenEndpoint { get; set; }
    public ScimEntrepriseMappingsResult Mappings { get; set; }
}