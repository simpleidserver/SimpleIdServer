// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace SimpleIdServer.IdServer.Startup;

public class IdentityServerConfiguration
{
    public string AuthCookieNamePrefix { get; set; }
    public string SessionCookieNamePrefix { get; set; }
    public bool IsRealmEnabled { get; set; }
    public bool IsClientCertificateEnabled { get; set; }
    public ClientCertificateMode? ClientCertificateMode { get; set; }
    public string Authority { get; set; }
    public bool IsForwardedEnabled { get; set; }
    public bool IsClientCertificateForwarded { get; set; }
    public bool ForceHttps { get; set; }

    /// <summary>
    /// Indicates whether the empty "oth" attribute should be ignored in the JWKS endpoint.
    /// This may help resolve the following error in NextAuth.js when decoding the token:<br />
    /// RSA JWK "oth" (Other Primes Info) Parameter value is not supported.
    /// </summary>
    public bool IgnoreEmptyOthInJwksEntpoint { get; set; } = false;
}
