// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Text.Json;

namespace SimpleIdServer.OAuth.Options
{
    public class OAuthHostOptions
    {
        /// <summary>
        /// OAUTH2.0 client's default scopes.
        /// </summary>
        public ICollection<string> DefaultScopes { get; set; } = new List<string>();
        /// <summary>
        /// Client secret expiration time in seconds.
        /// </summary>
        public int? ClientSecretExpirationInSeconds { get; set; } = null;
        /// <summary>
        /// Authorization code expiration time in seconds.
        /// </summary>
        public int AuthorizationCodeExpirationInSeconds { get; set; } = 600;
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
        public string CertificateAuthenticationScheme { get; set; } = Constants.CertificateAuthenticationScheme;
        /// <summary>
        /// JWK expiration time in seconds.
        /// </summary>
        public int JWKExpirationTimeInSeconds { get; set; } = 60 * 5;
        /// <summary>
        /// Default Token Expiration Time in seconds.
        /// </summary>
        public double DefaultTokenExpirationTimeInSeconds { get; set; } = 60 * 30;
        /// <summary>
        /// Default Refresh Token Expiration Time in seconds.
        /// </summary>
        public double DefaultRefreshTokenExpirationTimeInSeconds { get; set; } = 60 * 30;
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
        /// <summary>
        /// Default authentication method used by the client.
        /// </summary>
        public string DefaultTokenEndPointAuthMethod { get; set; } = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
        /// <summary>
        /// Default token signed response algorithm.
        /// </summary>
        public string DefaultTokenSignedResponseAlg { get; set; } = SecurityAlgorithms.RsaSha256;
        /// <summary>
        /// OAUTH2.0 client's default token profile.
        /// </summary>
        public string DefaultTokenProfile { get; set; } = BearerTokenProfile.DEFAULT_NAME;
        /// <summary>
        /// Default encryption alg (JWE).
        /// </summary>
        public string DefaultTokenEncrypteAlg { get; set; } = SecurityAlgorithms.Aes128KW;
        /// <summary>
        /// Default encryption enc (JWE).
        /// </summary>
        public string DefaultTokenEncryptedEnc { get; set; } = SecurityAlgorithms.Aes128CbcHmacSha256;
        /// <summary>
        /// Default code challenge method.
        /// </summary>
        public string DefaultCodeChallengeMethod { get; set; } = PlainCodeChallengeMethodHandler.DEFAULT_NAME;
        /// <summary>
        /// Customizable parameters.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// UI cultures.
        /// </summary>
        public IEnumerable<UICulture> SupportedUICultures { get; set; } = new List<UICulture> { new UICulture { DisplayName = "English", Name = "en" } };
        /// <summary>
        /// Default culture.
        /// </summary>
        public string DefaultCulture { get; set; } = "en";

        public int GetIntParameter(string name) => int.Parse(Parameters[name]);

        public string GetStringParameter(string name) => Parameters[name];

        public IEnumerable<string> GetStringArrayParameter(string name) => Parameters[name].Split(',');

        public IEnumerable<T> GetObjectArrayParameter<T>(string name) => JsonSerializer.Deserialize<IEnumerable<T>>(Parameters[name]);
    }

    public class UICulture
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}