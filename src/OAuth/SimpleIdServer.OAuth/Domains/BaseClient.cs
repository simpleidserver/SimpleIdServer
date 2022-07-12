// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains
{
    public abstract class BaseClient : IEquatable<BaseClient>
    {
        public BaseClient()
        {
            GrantTypes = new List<string>();
            RedirectionUrls = new List<string>();
            ResponseTypes = new List<string>();
            Contacts = new List<string>();
            PostLogoutRedirectUris = new List<string>();
            JsonWebKeys = new List<JsonWebKey>();
            Translations = new List<OAuthClientTranslation>();
        }

        #region Properties

        /// <summary>
        /// Client identifier.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Client secret.
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Client secret expiration time.
        /// </summary>
        public DateTime? ClientSecretExpirationTime { get; set; }

        /// <summary>
        /// One or more human readable client name
        /// </summary>
        public ICollection<OAuthTranslation> ClientNames 
        {
            get 
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "client_name").ToList();
            }
        }

        /// <summary>
        /// One or more URL that references a logo for the client
        /// </summary>
        public ICollection<OAuthTranslation> LogoUris
        {
            get
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "logo_uri").ToList();
            }
        }

        /// <summary>
        /// One or more URL of a web page providing information about the client.
        /// </summary>
        public ICollection<OAuthTranslation> ClientUris
        {
            get
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "client_uri").ToList();
            }
        }

        /// <summary>
        /// One or more URL that points to a human-readable policy document for the client
        /// </summary>
        public ICollection<OAuthTranslation> PolicyUris
        {
            get
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "policy_uri").ToList();
            }
        }

        /// <summary>
        /// One or more URL that points to a human-readable terms of service document for the client
        /// </summary>
        public ICollection<OAuthTranslation> TosUris
        {
            get
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "tos_uri").ToList();
            }
        }

        public ICollection<OAuthClientTranslation> Translations { get; set; }

        /// <summary>
        /// Cryptographic algorithm used to secure the JWS access token.
        /// </summary>
        public string TokenSignedResponseAlg { get; set; }

        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS access token.
        /// </summary>
        public string TokenEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS access token.
        /// </summary>
        public string TokenEncryptedResponseEnc { get; set; }

        /// <summary>
        /// Requested authentication method for the token endpoint.
        /// </summary>
        public string TokenEndPointAuthMethod { get; set; }

        /// <summary>
        /// Array of OAUTH2.0 grant type strings that the client can use at the token endpoint.
        /// </summary>
        public IEnumerable<string> GrantTypes { get; set; }

        /// <summary>
        /// Array of the OAUTH2.0 response type strings that the client can use at the authorization endpoint.
        /// </summary>
        public IEnumerable<string> ResponseTypes { get; set; }

        /// <summary>
        /// Array of redirection URIS for use in redirect-based flows.
        /// </summary>
        public IEnumerable<string> RedirectionUrls { get; set; }

        /// <summary>
        /// Array of URLs supplied by the RP to which it MAY request that the End-User's User Agent be redirected using the post_logout_redirect_uri parameter after a logout has been performed.
        /// </summary>
        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// URI string referencing the client’s JSON Web Key (JWK) Set document, which contains the client’s public keys.
        /// </summary>
        public string JwksUri { get; set; }

        /// <summary>
        /// Client’s JSON Web Key Set document value, which contains the client’s public keys.
        /// </summary>
        public IEnumerable<JsonWebKey> JsonWebKeys { get; set; }

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
        public string PreferredTokenProfile { get; set; }

        /// <summary>
        /// Array of strings representing ways to contact people responsible for this client, typically email addresses.
        /// </summary>
        public IEnumerable<string> Contacts { get; set; }

        /// <summary>
        /// A unique identifier string assigned by the client developer or software publisher used by registration endpoints to identify the client software to be dynamically registered.
        /// </summary>
        public string SoftwareId { get; set; }

        /// <summary>
        /// A version identifier string for the client software identified by "software_id".
        /// </summary>
        public string SoftwareVersion { get; set; }

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
        public string RegistrationAccessToken { get; set; }

        /// <summary>
        /// Boolean value used to indicate the client's intention to use mutual-TLS client certificate-bound access tokens.
        /// https://tools.ietf.org/html/rfc8705#section-3.4
        /// </summary>
        public bool TlsClientCertificateBoundAccessToken { get; set; }

        /// <summary>
        /// A string representation of the expected  subject distinguished  of the certificate that the OAuth client will use in mututal-TLS authentication.
        /// </summary>
        public string TlsClientAuthSubjectDN { get; set; }

        /// <summary>
        /// A string containing the value of an expected dNSName SAN entry in the certificate.
        /// </summary>
        public string TlsClientAuthSanDNS { get; set; }

        /// <summary>
        /// A string containing the value expected uniformResourceIdentifier  SAN entry in the certificate.
        /// </summary>
        public string TlsClientAuthSanURI { get; set; }

        /// <summary>
        /// Astring representation of an IP address in either dotted decimal notation(for IPv4) or colon-delimited hexadecimal(for IPv6, as defined in [RFC5952]) 
        /// that is expected to be present as an iPAddress SAN entry in the certificate
        /// </summary>
        public string TlsClientAuthSanIP { get; set; }

        /// <summary>
        ///  A string containing the value of an expected rfc822Name SAN entry in the certificate.
        /// </summary>
        public string TlsClientAuthSanEmail { get; set; }

        /// <summary>
        /// Scopes used by the client to control its access.
        /// </summary>
        public ICollection<ClientScope> Scopes { get; set; }

        /// <summary>
        /// Allowed scopes
        /// </summary>
        public abstract ICollection<OAuthScope> AllowedScopes { get; set; }

        #endregion

        public async Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(IHttpClientFactory httpClientFactory)
        {
            if (JsonWebKeys != null && JsonWebKeys.Any())
            {
                return JsonWebKeys;
            }

            Uri uri = null;
            if (string.IsNullOrWhiteSpace(JwksUri) || !Uri.TryCreate(JwksUri, UriKind.Absolute, out uri))
            {
                return new JsonWebKey[0];
            }

            using (var httpClient = httpClientFactory.GetHttpClient())
            {
                httpClient.BaseAddress = uri;
                var request = await httpClient.GetAsync(uri.AbsoluteUri).ConfigureAwait(false);
                request.EnsureSuccessStatusCode();
                var json = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                var keysJson = JObject.Parse(json)["keys"].ToString();
                var jsonWebKeys = JsonConvert.DeserializeObject<JArray>(keysJson).Select(k => JsonWebKey.Deserialize(k.ToString()));
                return jsonWebKeys;
            }
        }

        public void SetClientNames(ICollection<OAuthTranslation> translations)
        {
            ClearTranslations("client_name");
            foreach(var translation in translations)
            {
                AddClientName(translation.Language, translation.Value);
            }
        }

        public void AddClientName(string language, string value)
        {
            Translations.Add(new OAuthClientTranslation
            {
                Translation = new OAuthTranslation($"{ClientId}_client_name", value, language)
                {
                    Type = "client_name"
                }
            });
        }

        public void AddClientUri(string language, string value)
        {
            Translations.Add(new OAuthClientTranslation
            {
                Translation = new OAuthTranslation($"{ClientId}_client_uri", value, language)
                {
                    Type = "client_uri"
                }
            });
        }

        public void SetLogoUris(ICollection<OAuthTranslation> translations)
        {
            ClearTranslations("logo_uri");
            foreach(var translation in translations)
            {
                AddLogoUri(translation.Language, translation.Value);
            }
        }

        public void AddLogoUri(string language, string value)
        {
            Translations.Add(new OAuthClientTranslation
            {
                Translation = new OAuthTranslation($"{ClientId}_logo_uri", value, language)
                {
                    Type = "logo_uri"
                }
            });
        }

        public void AddTosUri(string language, string value)
        {
            Translations.Add(new OAuthClientTranslation
            {
                Translation = new OAuthTranslation($"{ClientId}_tos_uri", value, language)
                {
                    Type = "tos_uri"
                }
            });
        }

        public void AddPolicyUri(string language, string value)
        {
            Translations.Add(new OAuthClientTranslation
            {
                Translation = new OAuthTranslation($"{ClientId}_policy_uri", value, language)
                {
                    Type = "policy_uri"
                }
            });
        }

        public void SetClientSecret(string secret, DateTime? expirationTime)
        {
            ClientSecret = secret;
            ClientSecretExpirationTime = expirationTime;
        }

        public abstract void SetAllowedScopes(ICollection<OAuthScope> scopes);

        /// <summary>
        /// Get all the redirection urls.
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<string>> GetRedirectionUrls(IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
        {
            IEnumerable<string> result = RedirectionUrls == null ? new List<string>() : RedirectionUrls.ToList();
            return Task.FromResult(result);
        }

        public override bool Equals(object obj)
        {
            var r = obj as BaseClient;
            if (r == null)
            {
                return false;
            }

            return GetHashCode() == r.GetHashCode();
        }

        public bool Equals(BaseClient other)
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

        protected void ClearTranslations(string type)
        {
            for (int i = Translations.Count() - 1; i >= 0; i--)
            {
                var translation = Translations.ElementAt(i);
                if (translation.Translation.Type == type)
                {
                    Translations.Remove(translation);
                }
            }
        }
    }
}
