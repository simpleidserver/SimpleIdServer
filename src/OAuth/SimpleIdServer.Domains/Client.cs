// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Domains
{
    public class Client : IEquatable<Client>
    {
        /// <summary>
        /// <summary>
        /// Client identifier.
        /// </summary>
        public string ClientId { get; set; } = null!;
        /// <summary>
        /// Client secret.
        /// </summary>
        public string ClientSecret { get; set; } = null!;
        /// <summary>
        /// Client secret expiration time.
        /// </summary>
        public DateTime? ClientSecretExpirationTime { get; set; }
        /// <summary>
        /// Cryptographic algorithm used to secure the JWS access token.
        /// </summary>
        public string? TokenSignedResponseAlg { get; set; }

        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS access token.
        /// </summary>
        public string? TokenEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS access token.
        /// </summary>
        public string? TokenEncryptedResponseEnc { get; set; } = null;

        /// <summary>
        /// Requested authentication method for the token endpoint.
        /// </summary>
        public string? TokenEndPointAuthMethod { get; set; } = null;

        /// <summary>
        /// Array of OAUTH2.0 grant type strings that the client can use at the token endpoint.
        /// </summary>
        public IEnumerable<string> GrantTypes { get; set; } = new List<string>();

        /// <summary>
        /// Array of the OAUTH2.0 response type strings that the client can use at the authorization endpoint.
        /// </summary>
        public IEnumerable<string> ResponseTypes { get; set; } = new List<string>();

        /// <summary>
        /// Array of redirection URIS for use in redirect-based flows.
        /// </summary>
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();

        /// <summary>
        /// Array of URLs supplied by the RP to which it MAY request that the End-User's User Agent be redirected using the post_logout_redirect_uri parameter after a logout has been performed.
        /// </summary>
        public IEnumerable<string> PostLogoutRedirectUris { get; set; } = new List<string>();

        /// <summary>
        /// URI string referencing the client’s JSON Web Key (JWK) Set document, which contains the client’s public keys.
        /// </summary>
        public string? JwksUri { get; set; } = null;

        /// <summary>
        /// Token expiration time in seconds.
        /// </summary>
        public double TokenExpirationTimeInSeconds { get; set; }

        /// <summary>
        /// Refresh token expiration time in seconds.
        /// </summary>
        public double RefreshTokenExpirationTimeInSeconds { get; set; }

        /// <summary>
        /// Preferred token profile.
        /// </summary>
        public string? PreferredTokenProfile { get; set; } = null;

        /// <summary>
        /// Array of strings representing ways to contact people responsible for this client, typically email addresses.
        /// </summary>
        public IEnumerable<string> Contacts { get; set; } = new List<string>();

        /// <summary>
        /// A unique identifier string assigned by the client developer or software publisher used by registration endpoints to identify the client software to be dynamically registered.
        /// </summary>
        public string? SoftwareId { get; set; } = null;

        /// <summary>
        /// A version identifier string for the client software identified by "software_id".
        /// </summary>
        public string? SoftwareVersion { get; set; } = null;

        /// <summary>
        /// Creation date time.
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// Update date time.
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// String containing the access token to be used at the client configuration endpoint to perform subsequent operations upon the client registration.
        /// </summary>
        public string? RegistrationAccessToken { get; set; } = null;

        /// <summary>
        /// Boolean value used to indicate the client's intention to use mutual-TLS client certificate-bound access tokens.
        /// https://tools.ietf.org/html/rfc8705#section-3.4
        /// </summary>
        public bool TlsClientCertificateBoundAccessToken { get; set; }

        /// <summary>
        /// A string representation of the expected  subject distinguished  of the certificate that the OAuth client will use in mututal-TLS authentication.
        /// </summary>
        public string? TlsClientAuthSubjectDN { get; set; } = null;

        /// <summary>
        /// A string containing the value of an expected dNSName SAN entry in the certificate.
        /// </summary>
        public string? TlsClientAuthSanDNS { get; set; } = null;

        /// <summary>
        /// A string containing the value expected uniformResourceIdentifier  SAN entry in the certificate.
        /// </summary>
        public string? TlsClientAuthSanURI { get; set; } = null;

        /// <summary>
        /// Astring representation of an IP address in either dotted decimal notation(for IPv4) or colon-delimited hexadecimal(for IPv6, as defined in [RFC5952]) 
        /// that is expected to be present as an iPAddress SAN entry in the certificate
        /// </summary>
        public string? TlsClientAuthSanIP { get; set; } = null;

        /// <summary>
        ///  A string containing the value of an expected rfc822Name SAN entry in the certificate.
        /// </summary>
        public string? TlsClientAuthSanEmail { get; set; } = null;

        /// <summary>
        /// Enable or disble the consent screen.
        /// </summary>
        public bool IsConsentDisabled { get; set; }

        /// <summary>
        /// Scopes used by the client to control its access.
        /// </summary>
        public ICollection<ClientScope> Scopes { get; set; } = new List<ClientScope>();

        /// <summary>
        /// Client’s JSON Web Key Set document value, which contains the client’s public keys.
        /// </summary>
        public ICollection<ClientSerializedJsonWebKey> JsonWebKeys { get; set; } = new List<ClientSerializedJsonWebKey>();
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();

        public override bool Equals(object obj)
        {
            var r = obj as Client;
            if (r == null)
            {
                return false;
            }

            return GetHashCode() == r.GetHashCode();
        }

        public bool Equals(Client other)
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
    }
}
