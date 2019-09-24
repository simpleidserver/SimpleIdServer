// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Options
{
    public class SoftwareStatementTrustedParty
    {
        public SoftwareStatementTrustedParty(string iss, string jwksUrl)
        {
            Iss = iss;
            JwksUrl = jwksUrl;
        }

        /// <summary>
        /// Get the issuer.
        /// </summary>
        public string Iss { get; }
        /// <summary>
        /// Get the JWKS url.
        /// </summary>
        public string JwksUrl { get; }
    }
}