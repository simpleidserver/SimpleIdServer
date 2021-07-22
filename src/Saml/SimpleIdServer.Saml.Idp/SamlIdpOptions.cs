// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Idp
{
    public class SamlIdpOptions
    {
        public SamlIdpOptions()
        {
            DefaultAuthnContextClassRef = "urn:oasis:names:tc:SAML:2.0:ac:classes:Password";
            Issuer = "http://simpleidserver-saml-idp.com";
            CookieAuthExpirationTimeInSeconds = 5 * 60;
            SessionCookieName = CookieAuthenticationDefaults.CookiePrefix + "Session";
        }

        /// <summary>
        /// Default Authentication Context Class used to authenticate the user.
        /// </summary>
        public string DefaultAuthnContextClassRef { get; set; }
        /// <summary>
        /// Issuer.
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// Certificate used to sign response.
        /// </summary>
        public X509Certificate2 SigningCertificate { get; set; }
        /// <summary>
        /// Cookie auth expiration time in seconds.
        /// </summary>
        public int CookieAuthExpirationTimeInSeconds { get; set; }
        /// <summary>
        /// Name of the cookie used to store the session id.
        /// </summary>
        public string SessionCookieName { get; set; }
    }
}
