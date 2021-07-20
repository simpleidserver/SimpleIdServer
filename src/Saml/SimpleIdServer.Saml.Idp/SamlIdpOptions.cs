// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Saml.Idp
{
    public class SamlIdpOptions
    {
        public SamlIdpOptions()
        {
            DefaultAuthnContextClassRef = "urn:oasis:names:tc:SAML:2.0:ac:classes:Password";
            Issuer = "http://simpleidserver.com";
        }

        /// <summary>
        /// Default Authentication Context Class used to authenticate the user.
        /// </summary>
        public string DefaultAuthnContextClassRef { get; set; }
        /// <summary>
        /// Issue.
        /// </summary>
        public string Issuer { get; set; }
    }
}
