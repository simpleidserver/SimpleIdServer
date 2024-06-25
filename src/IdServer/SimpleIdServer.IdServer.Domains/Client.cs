// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    [JsonConverter(typeof(TranslatableConverter<Client>))]
    public class Client : ITranslatable, IEquatable<Client>
    {
        [JsonPropertyName(OAuthClientParameters.Id)]
        public string Id { get; set; }
        /// <summary>
        /// Client identifier.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ClientId)]
        public string ClientId { get; set; } = null!;
        /// <summary>
        /// Client secret.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ClientSecret)]
        public string ClientSecret { get; set; } = null!;
        /// <summary>
        /// String containing the access token to be used at the client configuration endpoint to perform subsequent operations upon the client registration.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RegistrationAccessToken)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RegistrationAccessToken { get; set; } = null;
        /// <summary>
        /// Array of OAUTH2.0 grant type strings that the client can use at the token endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.GrantTypes)]
        public ICollection<string> GrantTypes { get; set; } = new List<string>();
        /// <summary>
        /// Array of redirection URIS for use in redirect-based flows.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RedirectUris)]
        public ICollection<string> RedirectionUrls { get; set; } = new List<string>();
        /// <summary>
        /// Requested authentication method for the token endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenEndpointAuthMethod)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TokenEndPointAuthMethod { get; set; } = null;
        /// <summary>
        /// Array of the OAUTH2.0 response type strings that the client can use at the authorization endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ResponseTypes)]
        public ICollection<string> ResponseTypes { get; set; } = new List<string>();
        /// <summary>
        /// Readable client name.
        /// </summary>
        public string? ClientName
        {
            get
            {
                return Translate("client_name");
            }
        }
        /// <summary>
        ///Readable client logo.
        /// </summary>
        public string? LogoUri
        {
            get
            {
                return Translate("logo_uri");
            }
        }
        /// <summary>
        ///Readable client uri.
        /// </summary>
        public string? ClientUri
        {
            get
            {
                return Translate("client_uri");
            }
        }
        /// <summary>
        ///Readable TOS uri.
        /// </summary>
        public string? TosUri
        {
            get
            {
                return Translate("tos_uri");
            }
        }
        /// <summary>
        ///Readable policy uri.
        /// </summary>
        public string? PolicyUri
        {
            get
            {
                return Translate("policy_uri");
            }
        }
        /// <summary>
        /// URI string referencing the client’s JSON Web Key (JWK) Set document, which contains the client’s public keys.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.JwksUri)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? JwksUri { get; set; } = null;
        /// <summary>
        /// Array of strings representing ways to contact people responsible for this client, typically email addresses.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.Contacts)]
        public ICollection<string> Contacts { get; set; } = new List<string>();
        /// <summary>
        /// A unique identifier string assigned by the client developer or software publisher used by registration endpoints to identify the client software to be dynamically registered.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SoftwareId)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SoftwareId { get; set; } = null;
        /// <summary>
        /// A version identifier string for the client software identified by "software_id".
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SoftwareVersion)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SoftwareVersion { get; set; } = null;
        /// <summary>
        /// A string representation of the expected  subject distinguished  of the certificate that the OAuth client will use in mututal-TLS authentication.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSubjectDN)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TlsClientAuthSubjectDN { get; set; } = null;
        /// <summary>
        /// A string containing the value of an expected dNSName SAN entry in the certificate.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanDNS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TlsClientAuthSanDNS { get; set; } = null;
        /// <summary>
        /// A string containing the value expected uniformResourceIdentifier  SAN entry in the certificate.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanUri)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TlsClientAuthSanURI { get; set; } = null;
        /// <summary>
        /// Astring representation of an IP address in either dotted decimal notation(for IPv4) or colon-delimited hexadecimal(for IPv6, as defined in [RFC5952]) 
        /// that is expected to be present as an iPAddress SAN entry in the certificate
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanIp)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TlsClientAuthSanIP { get; set; } = null;
        /// <summary>
        ///  A string containing the value of an expected rfc822Name SAN entry in the certificate.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanEmail)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TlsClientAuthSanEmail { get; set; } = null;
        /// <summary>
        /// Client secret expiration time.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ClientSecretExpiresAt)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? ClientSecretExpirationTime { get; set; }
        /// <summary>
        /// Update date time.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Enable or disable token exchange.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IsTokenExchangeEnabled)]
        public bool IsTokenExchangeEnabled { get; set; }
        /// <summary>
        /// Type of token exchange (delegation or impersonation).
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenExchangeType)]
        public TokenExchangeTypes? TokenExchangeType { get; set; }
        /// <summary>
        /// Creation date time.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(OAuthClientParameters.ClientIdIssuedAt)]
        public double ClientIdIssuedAt
        {
            get
            {
                return CreateDateTime.ConvertToUnixTimestamp();
            }
        }
        /// <summary>
        /// Token expiration time in seconds.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenExpirationTimeInSeconds)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TokenExpirationTimeInSeconds { get; set; }
        /// <summary>
        /// Lifetime in seconds of the c_nonce.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CNonceExpirationTimeInSeconds { get; set; }
        /// <summary>
        /// Refresh token expiration time in seconds.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RefreshTokenExpirationTimeInSeconds)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? RefreshTokenExpirationTimeInSeconds { get; set; }
        [JsonPropertyName(OAuthClientParameters.Scope)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Scope
        {
            get
            {
                return string.Join(" ", Scopes.Select(s => s.Name));
            }
            set
            {
                Scopes = value == null ? new List<Scope>() : value.Split(" ").Select(v => new Scope
                {
                    Name = v
                }).ToList();
            }
        }
        [JsonPropertyName(OAuthClientParameters.Jwks)]
        public JsonObject Jwks
        {
            get
            {
                var keys = new JsonArray();
                foreach (var jwk in SerializedJsonWebKeys) keys.Add(jwk.Serialize());
                return new JsonObject
                {
                    { "keys", keys }
                };
            }
        }
        [JsonIgnore]
        public IEnumerable<JsonWebKey> JsonWebKeys
        {
            get
            {
                return SerializedJsonWebKeys.Select(j=> new JsonWebKey(j.SerializedJsonWebKey));
            }
        }

        /// <summary>
        /// Array of strings representing URI scheme identifiers and optionally method names of supported Subject Syntax Types.
        /// For example : did:key, did:example, urn:ietf:params:oauth:jwk-thumbprint
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SubjectSyntaxTypesSupported)]
        public List<string> SubjectSyntaxTypesSupported { get; set; } = new List<string>();
        /// <summary>
        /// Cryptographic algorithm used to secure the JWS access token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenSignedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TokenSignedResponseAlg { get; set; } = null;
        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS access token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TokenEncryptedResponseAlg { get; set; } = null;
        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS access token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseEnc)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TokenEncryptedResponseEnc { get; set; } = null;
        /// <summary>
        /// Array of URLs supplied by the RP to which it MAY request that the End-User's User Agent be redirected using the post_logout_redirect_uri parameter after a logout has been performed.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.PostLogoutRedirectUris)]
        public ICollection<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        /// <summary>
        /// Redirect the user-agent to the revoke session UI.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RedirectToRevokeSessionUI)]
        public bool RedirectToRevokeSessionUI { get; set; } = true;
        /// <summary>
        /// Preferred token profile.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.PreferredTokenProfile)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PreferredTokenProfile { get; set; } = null;
        /// <summary>
        /// Alg algorithm that MUST be used for signing Request Objects sent to the OP. 
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RequestObjectSigningAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestObjectSigningAlg { get; set; } = null;
        /// <summary>
        /// The RP is declaring that it may use for encrypting Request Objects sent to the OP. 
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RequestObjectEncryptionAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestObjectEncryptionAlg { get; set; } = null;
        /// <summary>
        /// The RP is declaring that it may use for encrypting Request Objects sent to the OP
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RequestObjectEncryptionEnc)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestObjectEncryptionEnc { get; set; } = null;
        /// <summary>
        /// subject_type requested for responses to this client. Possible values are “pairwise” or “public”.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SubjectType)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SubjectType { get; set; } = null;
        [JsonIgnore]
        /// <summary>
        /// SALT used to calculate the pairwise.
        /// </summary>
        public string? PairWiseIdentifierSalt { get; set; } = null;
        /// <summary>
        /// The value of the sector_identifier_uri MUST be a URL using the https scheme that points to a JSON file containing an array of redirect_uri values.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SectorIdentifierUri)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SectorIdentifierUri { get; set; } = null;
        /// <summary>
        /// Cryptographic algorithm used to secure the JWS identity token. 
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IdTokenSignedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IdTokenSignedResponseAlg { get; set; } = null;
        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS identity token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IdTokenEncryptedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IdTokenEncryptedResponseAlg { get; set; } = null;
        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS identity token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IdTokenEncryptedResponseEnc)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IdTokenEncryptedResponseEnc { get; set; } = null;
        /// <summary>
        /// One of the following values: poll, ping or push.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.BCTokenDeliveryMode)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BCTokenDeliveryMode { get; set; } = null;
        /// <summary>
        /// This is the endpoint to which the OP will post a notification after a successful or failed end-user authentication.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.BCClientNotificationEndpoint)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BCClientNotificationEndpoint { get; set; } = null;
        /// <summary>
        /// The JWS algorithm alg value that the Client will use for signing authentication request.
        /// When omitted, the Client will not send signed authentication requests.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.BCAuthenticationRequestSigningAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BCAuthenticationRequestSigningAlg { get; set; }
        /// <summary>
        /// Required for signing UserInfo responses.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.UserInfoSignedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UserInfoSignedResponseAlg { get; set; } = null;
        [JsonPropertyName(OAuthClientParameters.UserInfoEncryptedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        /// <summary>
        /// Required for encrypting the identity token issued to this client.
        /// </summary>
        public string? UserInfoEncryptedResponseAlg { get; set; } = null;
        [JsonPropertyName(OAuthClientParameters.UserInfoEncryptedResponseEnc)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        /// <summary>
        /// Required for encrypting the identity token issued to this client.
        /// </summary>
        public string? UserInfoEncryptedResponseEnc { get; set; } = null;
        /// <summary>
        /// Boolean value specifying whether the Client supports the user_code parameter. 
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.BCUserCodeParameter)]
        public bool BCUserCodeParameter { get; set; } = false;
        [JsonPropertyName(OAuthClientParameters.FrontChannelLogoutUri)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        /// <summary>
        /// RP URL that will cause the RP to log itself out when rendered in an iframe by the OP.
        /// </summary>
        public string? FrontChannelLogoutUri { get; set; } = null;
        /// <summary>
        /// URL of the issuance initation endpoint of a Wallet.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.CredentialOfferEndpoint)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CredentialOfferEndpoint { get; set; } = null;
        /// <summary>
        /// If the wallet decided to use Pre-Authorized Code Flow, a PIN value must be sent in the user_pin parameter with the respective token request.
        /// </summary>
        [JsonIgnore]
        public bool IsTransactionCodeRequired { get; set; } = false;
        /// <summary>
        /// Expiration time in seconds of the pre-authorized code.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.PreAuthCodeExpirationTimeInSeconds)]
        public double PreAuthCodeExpirationTimeInSeconds { get; set; } = 5 * 60;
        [JsonPropertyName(OAuthClientParameters.FrontChannelLogoutSessionRequired)]
        /// <summary>
        /// Boolean value specifying whether the RP requires that iss (issuer) and sid (session id) query parameters be included to identify the RP session
        /// with the OP when the front_channel_logout_uri is used.
        /// </summary>
        public bool FrontChannelLogoutSessionRequired { get; set; }
        [JsonPropertyName(OAuthClientParameters.BackChannelLogoutSessionRequired)]
        /// <summary>
        /// Boolean value specifying whether the OP can pass a SID claim in the Logout token to identify the RP session with the OP.
        /// If supported, the sid claim is also included in ID tokens issued by the OP.
        /// </summary>
        public bool BackChannelLogoutSessionRequired { get; set; }
        [JsonPropertyName(OAuthClientParameters.DefaultMaxAge)]
        public double? DefaultMaxAge { get; set; } = null;
        /// <summary>
        /// Boolean value used to indicate the client's intention to use mutual-TLS client certificate-bound access tokens.
        /// https://tools.ietf.org/html/rfc8705#section-3.4
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientCertificateBoundAccessToken)]
        public bool TlsClientCertificateBoundAccessToken { get; set; }
        /// <summary>
        /// RP URL that will cause the RP to log itself out when sent a Logout Token by the OP.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.BackChannelLogoutUri)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BackChannelLogoutUri { get; set; } = null;
        /// <summary>
        /// Kind of the application. The default, if omitted is web.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ApplicationType)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ApplicationType { get; set; } = null;
        /// <summary>
        /// URI using the https scheme that a third party can use to initiate a login by the RP.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.InitiateLoginUri)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InitiateLoginUri { get; set; }
        /// <summary>
        /// Boolean value specifying whether the auth_time Claim in the ID Token is REQUIRED.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RequireAuthTime)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool RequireAuthTime { get; set; } = false;
        /// <summary>
        /// The JWS [RFC7515] alg algorithm REQUIRED for signing authorization responses.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.AuthorizationSignedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationSignedResponseAlg { get; set; } = null;
        /// <summary>
        /// The JWE [RFC7516] alg algorithm REQUIRED for encrypting authorization responses
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.AuthorizationEncryptedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationEncryptedResponseAlg { get; set; } = null;
        /// <summary>
        /// List of supported authorization details types.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.AuthorizationDataTypes)]
        public ICollection<string> AuthorizationDataTypes { get; set; } = new List<string>();
        /// <summary>
        /// he JWE [RFC7516] enc algorithm REQUIRED for encrypting authorization responses.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.AuthorizationEncryptedResponseEnc)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationEncryptedResponseEnc { get; set; } = null;
        /// <summary>
        /// Boolean value specifying whether the client always uses DPoP for token requests.
        /// If omitted, the default value is false.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.DPOPBoundAccessTokens)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool DPOPBoundAccessTokens { get; set; } = false;
        /// <summary>
        /// Enable or disble the consent screen.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IsConsentDisabled)]
        public bool IsConsentDisabled { get; set; }
        /// <summary>
        /// 'resource' parameter can be required or not.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IsResourceParameterRequired)]
        public bool IsResourceParameterRequired { get; set; } = false;
        /// <summary>
        /// Expiration time in seconds of auth_req_id
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.AuthReqIdExpirationTimeInSeconds)]
        public int AuthReqIdExpirationTimeInSeconds { get; set; } = 120;
        /// <summary>
        /// Scopes used by the client to control its access.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.Scopes)]
        public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
        /// <summary>
        /// Client’s JSON Web Key Set document value, which contains the client’s public keys.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SerializedJsonWebKeys)]
        public ICollection<ClientJsonWebKey> SerializedJsonWebKeys { get; set; } = new List<ClientJsonWebKey>();
        [JsonPropertyName(OAuthClientParameters.ClientType)]
        public string? ClientType { get; set; } = null;
        /// <summary>
        /// Nonce is required in the DPoP proof.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IsDPOPNonceRequired)]
        public bool IsDPOPNonceRequired { get; set; } = false;
        /// <summary>
        /// Lifetime of a DPOP nonce.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.DPOPNonceLifetimeInSeconds)]
        public double DPOPNonceLifetimeInSeconds { get; set; } = 5 * 60;
        [JsonPropertyName(OAuthClientParameters.IsRedirectUrlCaseSensitive)]
        public bool IsRedirectUrlCaseSensitive { get; set; } = false;
        /// <summary>
        /// Enable or disable self-issued.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IsSelfIssueEnabled)]
        public bool IsSelfIssueEnabled { get; set; }
        [JsonPropertyName(OAuthClientParameters.Parameters)]
        public JsonObject Parameters
        {
            get
            {
                return SerializedParameters == null ? new JsonObject() : JsonObject.Parse(SerializedParameters).AsObject();
            }
            set
            {
                SerializedParameters = JsonSerializer.Serialize(value);
            }
        }
        [JsonIgnore]
        public string? SerializedParameters { get; set; } = null;
        /// <summary>
        /// Default acr values.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.DefaultAcrValues)]
        public ICollection<string> DefaultAcrValues { get; set; } = new List<string>();
        /// <summary>
        /// A JSON number with a positive integer value indicating the minimum amount of time in seconds that the client MUST wait between polling requests to the token endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.BCIntervalSeconds)]
        public int BCIntervalSeconds { get; set; } = 5;
        /// <summary>
        /// Set the type of access token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.AccessTokenType)]
        public AccessTokenTypes AccessTokenType { get; set; } = AccessTokenTypes.Jwt;
        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
        [JsonIgnore]
        public ICollection<DeviceAuthCode> DeviceAuthCodes { get; set; } = new List<DeviceAuthCode>();

        public double? GetDoubleParameter(string name)
        {
            if (!Parameters.ContainsKey(name)) return null;
            if (double.TryParse(Parameters[name].ToString(), out double res)) return res;
            return null;
        }

        public void Add(string keyId, JsonWebKey jsonWebKey, string usage, SecurityKeyTypes keyType)
        {
            SerializedJsonWebKeys.Add(new ClientJsonWebKey
            {
                Kid = keyId,
                SerializedJsonWebKey = JsonSerializer.Serialize(jsonWebKey),
                Alg = jsonWebKey.Alg,
                KeyType = keyType,
                Usage = usage
            });
        }

        public string GetStringParameter(string name) => Parameters[name].ToString();

        public void UpdateClientName(string clientName)
        {
            var language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var translation = Translations.FirstOrDefault(tr => tr.Key == "client_name" && tr.Language == language);
            if (translation == null)
                Translations.Add(new Translation { Language = language, Key = "client_name", Value = clientName });
            else
                translation.Value = clientName;
        }

        public IEnumerable<string> GetStringArrayParameter(string name) => Parameters[name].ToString().Split(',');

        private string? Translate(string key)
        {
            var translation = Translations.FirstOrDefault(t => t.Key == key && t.Language == CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            return translation?.Value;
        }

        public override bool Equals(object? obj)
        {
            var r = obj as Client;
            if (r == null)
            {
                return false;
            }

            return GetHashCode() == r.GetHashCode();
        }

        public bool Equals(Client? other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return ClientId.GetHashCode();
        }

        public JsonObject Serialize(string baseUrl)
        {
            var result = JsonSerializer.SerializeToNode(this).AsObject();
            result.Add(OAuthClientParameters.RegistrationClientUri, $"{baseUrl}/{ClientId}");
            return result;
        }
    }

    public class ClientTypes
    {
        public const string SPA = "SPA";
        public const string MACHINE = "MACHINE";
        public const string WEBSITE = "WEBSITE";
        public const string MOBILE = "MOBILE";
        public const string EXTERNAL = "EXTERNALDEVICE";
        public const string WALLET = "WALLET";
        public const string DEVICE = "DEVICE";
        public const string HIGHLYSECUREDWEBSITE = "HIGHLYSECUREDWEBSITE";
        public const string GRANTMANAGEMENT = "GRANTMANAGEMENT";
        public const string SAML = "SAML";
        public const string CREDENTIAL_ISSUER = "CREDENTIALISSUER";
    }
}
