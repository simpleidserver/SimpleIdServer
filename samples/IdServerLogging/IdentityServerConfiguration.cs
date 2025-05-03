// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace IdServerLogging;

public class IdentityServerConfiguration
{
    public string AuthCookieNamePrefix { get; set; }
    public string SessionCookieNamePrefix { get; set; }
    public bool IsRealmEnabled { get; set; }
    public ClientCertificateMode? ClientCertificateMode { get; set; }
    public string Authority { get; set; }
    public bool IsForwardedEnabled { get; set; }
    public bool IsFapiEnabled { get; set; }
    public bool ForceHttps { get; set; }
}
