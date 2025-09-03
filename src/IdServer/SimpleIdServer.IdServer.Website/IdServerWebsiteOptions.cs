// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Website;

public class IdServerWebsiteOptions
{
    public string DefaultLanguage { get; set; } = "en";
    public string ScimUrl { get; set; } = "https://localhost:5003";
    public IdServerWebsiteFeatures Features { get; set; } = new IdServerWebsiteFeatures();
    internal bool IsReamEnabled { get; set; } = false;
    internal string Issuer { get; set; } = "https://localhost:5001";
    internal bool ForceHttps { get; set; } = false;
    internal string ClientId { get; set; }
    internal string ClientSecret { get; set; }
    internal bool IgnoreCertificateError { get; set; }
}

public class IdServerWebsiteFeatures
{
    public bool IsFederationEntitiesEnabled
    {
        get; set;
    } = true;

    public bool IsMigrationsEnabled
    {
        get; set;
    } = true;
}