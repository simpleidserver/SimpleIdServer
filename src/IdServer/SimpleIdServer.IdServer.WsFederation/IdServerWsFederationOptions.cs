// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.WsFederation
{
    public class IdServerWsFederationOptions
    {
        /// <summary>
        /// Default JWK Identifier used to sign the response.
        /// </summary>
        public string DefaultKid { get; set; } = "wsFedKid";
        /// <summary>
        /// Default token type.
        /// </summary>
        public string DefaultTokenType { get; set; } = WsFederationConstants.TokenTypes.Saml2TokenProfile11;
        /// <summary>
        /// Default identifier format.
        /// </summary>
        public string DefaultNameIdentifierFormat { get; set; } = WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString;
    }
}
