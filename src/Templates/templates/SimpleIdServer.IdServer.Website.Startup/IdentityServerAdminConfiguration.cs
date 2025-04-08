// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Website.Startup;

public class IdentityServerAdminConfiguration
{
    public bool ForceHttps
    {
        get; set;
    }

    public bool IsRealmEnabled
    {
        get; set;
    }

    public string DataProtectionPath
    {
        get; set;
    }

    public string ScimBaseUrl
    {
        get; set;
    }
    
    public string IdserverBaseUrl
    {
        get; set;
    }

    public string ClientId
    {
        get; set;
    }

    public string ClientSecret
    {
        get; set;
    }

    public List<string> Scopes
    {
        get; set;
    }

    public bool IgnoreCertificateError
    {
        get; set;
    }
}
