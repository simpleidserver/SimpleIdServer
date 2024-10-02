// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;

public class AuthSchemeProvider
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string SamlMetadataUri { get; set; }
}