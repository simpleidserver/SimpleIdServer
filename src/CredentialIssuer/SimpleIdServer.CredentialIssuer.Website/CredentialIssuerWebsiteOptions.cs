// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.CredentialIssuer.Website;

public class CredentialIssuerWebsiteOptions
{
    public string IdServerBaseUrl { get; set; } = "https://localhost:5001";
    public string CredentialIssuerUrl { get; set; } = "https://localhost:5005";
}
