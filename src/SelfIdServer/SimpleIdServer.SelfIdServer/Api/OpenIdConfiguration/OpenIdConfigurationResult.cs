// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.SelfIdServer.Api.OpenIdConfiguration
{
    public class OpenIdConfigurationResult
    {
        /// <summary>
        /// URL of the Self-Issued OP used by the RP to perform Authentication of the End-User. Can be custom URL scheme, or Universal Links/App links.
        /// </summary>
        public string AuthorizationEndpoint { get; set; } = null!;
        /// <summary>
        /// URL using the https scheme with no query or fragment component that the Self-Issued OP asserts as its Issuer Identifier. 
        /// MUST be identical to the iss Claim value in ID Tokens issued from this Self-Issued OP.
        /// </summary>
        public string Issuer { get; set; } = null!;
        /// <summary>
        /// A JSON array of strings representing supported response types. MUST include id_token.
        /// </summary>
        public List<string> ResponseTypesSupported { get; set; } = new List<string>();
        /// <summary>
        /// A JSON array of strings representing supported scopes. MUST support the openid scope value.
        /// </summary>
        public List<string> ScopesSupported { get; set; } = new List<string>();
        /// <summary>
        /// A JSON array of strings representing supported subject types. Valid values include pairwise and public.
        /// </summary>
        public List<string> SubjectTypesSupported { get; set; } = new List<string>();
        /// <summary>
        /// A JSON array containing a list of the JWS signing algorithms (alg values) supported by the OP for the ID Token to encode the Claims in a JWT [RFC7519]. 
        /// Valid values include RS256, ES256, ES256K, and EdDSA.
        /// </summary>
        public List<string> IdTokenSigningAlgValuesSupported { get; set; } = new List<string>();
        /// <summary>
        /// A JSON array containing a list of the JWS signing algorithms (alg values) supported by the OP for Request Objects, which are described in Section 6.1 of [OpenID.Core].
        /// Valid values include none, RS256, ES256, ES256K, and EdDSA.
        /// </summary>
        public List<string> RequestObjezctSigningAlgValuesSupported { get; set; } = new List<string>();
        /// <summary>
        /// A JSON array of strings representing URI scheme identifiers and optionally method names of supported Subject Syntax Types defined in Section 8. 
        /// When Subject Syntax Type is JWK Thumbprint, a valid value is urn:ietf:params:oauth:jwk-thumbprint defined in [RFC9278]. 
        /// </summary>
        public List<string> SubjectSyntaxTypesSupported { get; set; } = new List<string>();
        /// <summary>
        /// A JSON array of strings containing the list of ID Token types supported by the OP, the default value is attester_signed_id_token. 
        /// </summary>
        public List<string> IdTokenTypesSupported { get; set; } = new List<string>();
    }
}
