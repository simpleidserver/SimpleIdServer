// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.WsFederation
{
    public class WsFederationOptions
    {
        /// <summary>
        /// Certificate used to sign message or for back-channel TLS authentication (or both).
        /// </summary>
        public X509SecurityKey?  SigningCertificate { get; set; }
    }
}
