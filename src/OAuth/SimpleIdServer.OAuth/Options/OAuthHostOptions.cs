// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Options
{
    public class OAuthHostOptions
    {
        /// <summary>
        /// OAUTH2.0 client's default scopes.
        /// </summary>
        public ICollection<string> DefaultScopes { get; set; } = new List<string>();
        /// <summary>
        /// OAUTH2.0 client's default token profile.
        /// </summary>
        public string DefaultTokenProfile { get; set; } = BearerTokenProfile.DEFAULT_NAME;
        /// <summary>
        /// Client secret expiration time in seconds.
        /// </summary>
        public int? ClientSecretExpirationInSeconds { get; set; } = null;
        /// <summary>
        /// Authorization cod expiration time in seconds.
        /// </summary>
        public int AuthorizationCodeExpirationInSeconds { get; set; }
        /// <summary>
        /// Trusted parties used to validate the software statement.
        /// </summary>
        public ICollection<SoftwareStatementTrustedParty> SoftwareStatementTrustedParties { get; set; } = new List<SoftwareStatementTrustedParty>();
        /// <summary>
        /// Mututal TLS is enabled.
        /// </summary>
        public bool MtlsEnabled { get; set; } = false;
        /// <summary>
        /// Client certificate authentication scheme.
        /// </summary>
        public string CertificateAuthenticationScheme { get; set; } = "Certificate";
        /// <summary>
        /// Default token signed response algorithm.
        /// </summary>
        public string DefaultTokenSignedResponseAlg { get; set; } = RSA256SignHandler.ALG_NAME;
        /// <summary>
        /// JWK expiration time in seconds.
        /// </summary>
        public int JWKExpirationTimeInSeconds { get; set; } = 60 * 5;
        /// <summary>
        /// Default Token Expiration Time in seconds.
        /// </summary>
        public int DefaultTokenExpirationTimeInSeconds { get; set; } = 60 * 30;
        /// <summary>
        /// Default Refresh Token Expiration Time in seconds.
        /// </summary>
        public int DefaultRefreshTokenExpirationTimeInSeconds { get; set; } = 60 * 30;
        /// <summary>
        /// HOTP Window.
        /// </summary>
        public int HOTPWindow { get; set; } = 5;
        /// <summary>
        /// Calculate time windows.
        /// </summary>
        public int TOTPStep { get; set; } = 30;
        /// <summary>
        /// Default OTP algorithm.
        /// </summary>
        public OTPAlgs OTPAlg { get; set; }
        /// <summary>
        /// Default OTP issuer.
        /// </summary>
        public string OTPIssuer { get; set; } = "SimpleIdServer";
        /// <summary>
        /// If true then "scope" claim is expressed as a list of space-delimited case sensistive strings"
        /// If false then "scope" claim is expressed as an array of string.
        /// </summary>
        public bool IsScopeClaimConcatenationEnabled { get; set; } = false;
    }
}