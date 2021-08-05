// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SimpleIdServer.Saml.Sp.Events;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Sp
{
    public class SamlSpOptions : RemoteAuthenticationOptions
    {
        public SamlSpOptions()
        {
            CallbackPath = new PathString("/saml2-signin");
            SPName = SamlSpDefaults.DisplayName;
            SPId = "urn:sp";
            CanonicalizationMethod = CanonicalizationMethods.C14;
            Events = new SamlSpEvents();
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
        /// <summary>
        /// Gets or sets the SamlSpEvents used to handle authentication events.
        /// </summary>
        public new SamlSpEvents Events
        {
            get { return (SamlSpEvents)base.Events; }
            set { base.Events = value; }
        }
        public string SignOutScheme { get; set; }
        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}
