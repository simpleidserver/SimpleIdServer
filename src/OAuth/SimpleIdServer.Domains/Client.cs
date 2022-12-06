// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Domains.DTOs;
using SimpleIdServer.Helpers.Extensions;
using SimpleIdServer.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Domains
{
    public class Client : IEquatable<Client>
    {
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
        public string? RegistrationAccessToken { get; set; } = null;
        /// <summary>
        /// Array of OAUTH2.0 grant type strings that the client can use at the token endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.GrantTypes)]
        public IEnumerable<string> GrantTypes { get; set; } = new List<string>();
        /// <summary>
        /// Array of redirection URIS for use in redirect-based flows.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RedirectUris)]
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        /// <summary>
        /// Requested authentication method for the token endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenEndpointAuthMethod)]
        public string? TokenEndPointAuthMethod { get; set; } = null;
        /// <summary>
        /// Array of the OAUTH2.0 response type strings that the client can use at the authorization endpoint.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ResponseTypes)]
        public IEnumerable<string> ResponseTypes { get; set; } = new List<string>();
        /// <summary>
        /// Readable client name.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ClientName)]
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
        [JsonPropertyName(OAuthClientParameters.LogoUri)]
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
        [JsonPropertyName(OAuthClientParameters.ClientUri)]
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
        [JsonPropertyName(OAuthClientParameters.TosUri)]
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
        [JsonPropertyName(OAuthClientParameters.TosUri)]
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
        public string? JwksUri { get; set; } = null;
        /// <summary>
        /// Array of strings representing ways to contact people responsible for this client, typically email addresses.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.Contacts)]
        public IEnumerable<string> Contacts { get; set; } = new List<string>();
        /// <summary>
        /// A unique identifier string assigned by the client developer or software publisher used by registration endpoints to identify the client software to be dynamically registered.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SoftwareId)]
        public string? SoftwareId { get; set; } = null;
        /// <summary>
        /// A version identifier string for the client software identified by "software_id".
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.SoftwareVersion)]
        public string? SoftwareVersion { get; set; } = null;
        /// <summary>
        /// A string representation of the expected  subject distinguished  of the certificate that the OAuth client will use in mututal-TLS authentication.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSubjectDN)]
        public string? TlsClientAuthSubjectDN { get; set; } = null;
        /// <summary>
        /// A string containing the value of an expected dNSName SAN entry in the certificate.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanDNS)]
        public string? TlsClientAuthSanDNS { get; set; } = null;
        /// <summary>
        /// A string containing the value expected uniformResourceIdentifier  SAN entry in the certificate.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanUri)]
        public string? TlsClientAuthSanURI { get; set; } = null;
        /// <summary>
        /// Astring representation of an IP address in either dotted decimal notation(for IPv4) or colon-delimited hexadecimal(for IPv6, as defined in [RFC5952]) 
        /// that is expected to be present as an iPAddress SAN entry in the certificate
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanIp)]
        public string? TlsClientAuthSanIP { get; set; } = null;
        /// <summary>
        ///  A string containing the value of an expected rfc822Name SAN entry in the certificate.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientAuthSanEmail)]
        public string? TlsClientAuthSanEmail { get; set; } = null;
        /// <summary>
        /// Client secret expiration time.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.ClientSecretExpiresAt)]
        public DateTime? ClientSecretExpirationTime { get; set; }
        /// <summary>
        /// Update date time.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
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
        public double TokenExpirationTimeInSeconds { get; set; }
        /// <summary>
        /// Refresh token expiration time in seconds.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.RefreshTokenExpirationTimeInSeconds)]
        public double RefreshTokenExpirationTimeInSeconds { get; set; }
        [JsonPropertyName(OAuthClientParameters.RefreshTokenExpirationTimeInSeconds)]
        public string Scope
        {
            get
            {
                return string.Join(" ", Scopes.Select(s => s.Scope));
            }
        }
        [JsonPropertyName(OAuthClientParameters.Jwks)]
        public JsonObject Jwks
        {
            get
            {
                return new JsonObject
                {
                    { "keys", JsonSerializer.Serialize(JsonWebKeys.Select(j => j.Serialize())) }
                };
            }
        }
        /// <summary>
        /// Cryptographic algorithm used to secure the JWS access token.
        /// </summary>
        public string? TokenSignedResponseAlg { get; set; }
        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS access token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseAlg)]
        public string? TokenEncryptedResponseAlg { get; set; }
        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS access token.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseEnc)]
        public string? TokenEncryptedResponseEnc { get; set; } = null;
        /// <summary>
        /// Array of URLs supplied by the RP to which it MAY request that the End-User's User Agent be redirected using the post_logout_redirect_uri parameter after a logout has been performed.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.PostLogoutRedirectUris)]
        public IEnumerable<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        /// <summary>
        /// Preferred token profile.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.PreferredTokenProfile)]
        public string? PreferredTokenProfile { get; set; } = null;
        /// <summary>
        /// Boolean value used to indicate the client's intention to use mutual-TLS client certificate-bound access tokens.
        /// https://tools.ietf.org/html/rfc8705#section-3.4
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.TlsClientCertificateBoundAccessToken)]
        public bool TlsClientCertificateBoundAccessToken { get; set; }
        /// <summary>
        /// Enable or disble the consent screen.
        /// </summary>
        [JsonPropertyName(OAuthClientParameters.IsConsentDisabled)]
        public bool IsConsentDisabled { get; set; }
        /// <summary>
        /// Scopes used by the client to control its access.
        /// </summary>
        [JsonIgnore]
        public ICollection<ClientScope> Scopes { get; set; } = new List<ClientScope>();
        /// <summary>
        /// Client’s JSON Web Key Set document value, which contains the client’s public keys.
        /// </summary>
        [JsonIgnore]
        public ICollection<JsonWebKey> JsonWebKeys { get; set; } = new List<JsonWebKey>();
        [JsonIgnore]
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();

        private string? Translate(string key)
        {
            var translation = Translations.FirstOrDefault(t => t.Key == key && t.Language == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
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
}
