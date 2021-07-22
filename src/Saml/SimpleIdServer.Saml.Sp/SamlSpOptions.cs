// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Sp
{
    public class SamlSpOptions
    {
        public SamlSpOptions()
        {
            Issuer = "http://simpleidserver-saml-sp.com";
        }

        /// <summary>
        /// Issuer.
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// Certificate used to sign request.
        /// </summary>
        public X509Certificate2 SigningCertificate { get; set; }
        /// <summary>
        /// Indicates whether the AuthnRequest messages sent by this service provider will be signed.
        /// </summary>
        public bool AuthnRequestSigned { get; set; }
    }
}
