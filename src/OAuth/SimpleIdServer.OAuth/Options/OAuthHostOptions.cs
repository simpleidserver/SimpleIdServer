// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Options
{
    public class OAuthHostOptions
    {
        public OAuthHostOptions()
        {
            DefaultScopes = new List<string>();
            DefaultTokenProfile = BearerTokenProfile.DEFAULT_NAME;
            ClientSecretExpirationInSeconds = null;
            SoftwareStatementTrustedParties = new List<SoftwareStatementTrustedParty>();
            DefaultCulture = "en";
            AuthorizationCodeExpirationInSeconds = 10 * 60;
            SupportedUICultures = new List<UICultureOption>
            {
                new UICultureOption("fr", "French"),
                new UICultureOption("en", "English")
            };
            MtlsEnabled = false;
            CertificateAuthenticationScheme = "Certificate";
        }

        /// <summary>
        /// OAUTH2.0 client's default scopes.
        /// </summary>
        public ICollection<string> DefaultScopes { get; set; }
        /// <summary>
        /// OAUTH2.0 client's default token profile.
        /// </summary>
        public string DefaultTokenProfile { get; set; }
        /// <summary>
        /// Client secret expiration time in seconds.
        /// </summary>
        public int? ClientSecretExpirationInSeconds { get; set; }
        /// <summary>
        /// Authorization cod expiration time in seconds.
        /// </summary>
        public int AuthorizationCodeExpirationInSeconds { get; set; }
        /// <summary>
        /// Trusted parties used to validate the software statement.
        /// </summary>
        public ICollection<SoftwareStatementTrustedParty> SoftwareStatementTrustedParties { get; set; }
        /// <summary>
        /// Set the default UI culture.
        /// </summary>
        public string DefaultCulture { get; set; }
        /// <summary>
        /// Supported cultures.
        /// </summary>
        public IEnumerable<UICultureOption> SupportedUICultures { get; set; }
        /// <summary>
        /// Mututal TLS is enabled.
        /// </summary>
        public bool MtlsEnabled { get; set; }
        /// <summary>
        /// Client certificate authentication scheme.
        /// </summary>
        public string CertificateAuthenticationScheme { get; set; }
    }
}