// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID;
using SimpleIdServer.Saml;
using SimpleIdServer.Saml.Sp;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenIDUma.Full.Startup.Converters
{
    public class SamlSpOptionsLite
    {
        /// <summary>
        /// Unique identifier of the Service Provider (SP).
        /// </summary>
        public string SPId { get; set; }
        /// <summary>
        /// Name of the Service Provider.
        /// </summary>
        public string SPName { get; set; }
        /// <summary>
        /// Metadata URL of the IdentityProvider.
        /// </summary>
        public string IdpMetadataUrl { get; set; }


        public static SamlSpOptionsLite Create(SamlSpOptions opts)
        {
            return new SamlSpOptionsLite
            {
                SPId = opts.SPId,
                SPName = opts.SPName,
                IdpMetadataUrl = opts.IdpMetadataUrl
            };
        }

        public SamlSpOptions ToOptions()
        {
            var certificate = new X509Certificate2(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "localhost.pfx"), "password");
            return new SamlSpOptions
            {
                SignInScheme = SIDOpenIdConstants.ExternalAuthenticationScheme,
                SPId = SPId,
                SPName = SPName,
                SigningCertificate = certificate,
                AuthnRequestSigned = true,
                WantAssertionSigned = true,
                SignatureAlg = SignatureAlgorithms.RSASHA256,
                CanonicalizationMethod = CanonicalizationMethods.C14,
                IdpMetadataUrl = IdpMetadataUrl,
                BaseUrl = "https://localhost:60000"
            };
        }
    }
}
