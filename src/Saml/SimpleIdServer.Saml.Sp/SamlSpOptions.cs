// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Sp
{
    public class SamlSpOptions
    {
        public SamlSpOptions()
        {
            SPName = "SP Saml2";
            SPId = "urn:sp";
            CanonicalizationMethod = CanonicalizationMethods.C14;
        }

        /// <summary>
        /// Unique identifier of the Service Provider (SP).
        /// </summary>
        public string SPId { get; set; }
        /// <summary>
        /// Name of the Service Provider.
        /// </summary>
        public string SPName { get; set; }
        /// <summary>
        /// Certificate used to sign request.
        /// </summary>
        public X509Certificate2 SigningCertificate { get; set; }
        /// <summary>
        /// Indicates whether the AuthnRequest messages sent by this service provider will be signed.
        /// </summary>
        public bool AuthnRequestSigned { get; set; }
        /// <summary>
        /// Want assertion signed.
        /// </summary>
        public bool WantAssertionSigned { get; set; }
        /// <summary>
        /// Default Signature Algorithm.
        /// </summary>
        public SignatureAlgorithms? SignatureAlg { get; set; }
        /// <summary>
        /// Default Canonicalization Method.
        /// </summary>
        public CanonicalizationMethods CanonicalizationMethod { get; set; }
        /// <summary>
        /// Metadata URL of the IdentityProvider.
        /// </summary>
        public string IdpMetadataUrl { get; set; }
    }
}
