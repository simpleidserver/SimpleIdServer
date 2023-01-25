// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Options
{
    public class IdServerHostOptions
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
        public bool MtlsEnabled { get; internal set; } = false;
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
        /// Defines a period that a TOTP code will be valid for in seconds.
        /// The default value is 30.
        /// </summary>
        public int TOTPStep { get; set; } = 30;
        /// <summary>
        /// Default OTP algorithm.
        /// Default value is TOPT.
        /// </summary>
        public OTPAlgs OTPAlg { get; set; } = OTPAlgs.TOTP;
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
        /// Default subject type.
        /// </summary>
        public string DefaultSubjectType { get; set; } = PublicSubjectTypeBuilder.SUBJECT_TYPE;
        /// <summary>
        /// Default code challenge method.
        /// </summary>
        public string DefaultCodeChallengeMethod { get; set; } = PlainCodeChallengeMethodHandler.DEFAULT_NAME;
        /// <summary>
        /// Default max_age.
        /// </summary>
        public double? DefaultMaxAge { get; set; } = null;
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
        /// <summary>
        /// Default acr value.
        /// </summary>
        public string DefaultAcrValue { get; set; } = Constants.StandardAcrs.FirstLevelAssurance.Name;
        /// <summary>
        /// Enable or disable realm.
        /// </summary>
        public bool UseRealm { get; set; } = false;
        /// <summary>
        /// Set the maximum lifetime of an authorization request.
        /// </summary>
        public int MaxRequestLifetime { get; set; } = 60 * 5;
        /// <summary>
        /// Enable or disable Back Channel Authentication.
        /// </summary>
        public bool IsBCEnabled { get; set; }
        /// <summary>
        /// Maximum number of characters for the "binding_message".
        /// </summary>
        public int MaxBindingMessageSize { get; set; } = 150;
        /// <summary>
        /// Cookie auth expiration time in seconds.
        /// </summary>
        public int CookieAuthExpirationTimeInSeconds { get; set; } = 5 * 60;
        /// <summary>
        /// Name of the cookie used to store the session id.
        /// </summary>
        public string SessionCookieName { get; set; } = CookieAuthenticationDefaults.CookiePrefix + "Session";
        /// <summary>
        /// Number of seconds the external authentication providers will be stored.
        /// </summary>
        public int? CacheExternalAuthProvidersInSeconds { get; set; }
        /// <summary>
        ///  If true, all authorization requests must specify a grant_management_action.
        ///  Default value is false.
        /// </summary>
        public bool GrantManagementActionRequired { get; set; } = false;
        /// <summary>
        /// Default OTP algorithm used.
        /// </summary>
        public OTPAlgs DefaultOTPAlg { get; set; } = OTPAlgs.TOTP;

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