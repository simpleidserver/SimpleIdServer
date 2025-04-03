// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Website;

public class IdServerWebsiteOptions
{
    public string DefaultLanguage { get; set; } = "en";
    public string ScimUrl { get; set; } = "https://localhost:5003";
    internal bool IsReamEnabled { get; set; } = true;
    internal string Issuer { get; set; } = "https://localhost:5001";
    internal bool ForceHttps { get; set; } = false;
}
