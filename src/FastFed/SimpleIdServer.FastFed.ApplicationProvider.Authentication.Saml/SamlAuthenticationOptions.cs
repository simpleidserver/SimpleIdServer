// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Authentication.Saml;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml;

public class SamlAuthenticationOptions
{
    public string SpId { get; set; } = "samlApplicationProvider";
    public string SamlMetadataUri { get; set; }
    public SamlEntrepriseMappingsResult Mappings { get; set; }
    public int? CacheSamlAuthProvidersInSeconds { get; set; }
}
