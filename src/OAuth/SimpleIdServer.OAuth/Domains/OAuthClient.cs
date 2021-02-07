// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains
{
    public enum ClientSecretTypes
    {
        SharedSecret = 0
    }

    public class ClientSecret : ICloneable
    {
        public ClientSecret(ClientSecretTypes type, string value, DateTime? expirationDateTime)
        {
            Type = type;
            Value = value;
            ExpirationDateTime = expirationDateTime;
        }

        public ClientSecretTypes Type { get; set; }
        public string Value { get; set; }
        public DateTime? ExpirationDateTime { get; set; }

        public object Clone()
        {
            return new ClientSecret(Type, Value, ExpirationDateTime);
        }
    }

    public class OAuthClient : ICloneable, IEquatable<OAuthClient>
    {
        public OAuthClient()
        {
            GrantTypes = new List<string>();
            RedirectionUrls = new List<string>();
            ResponseTypes = new List<string>();
            Contacts = new List<string>();
            PostLogoutRedirectUris = new List<string>();
            Secrets = new List<ClientSecret>();
            AllowedScopes = new List<OAuthScope>();
            JsonWebKeys = new List<JsonWebKey>();
            ClientNames = new List<OAuthTranslation>();
            ClientUris = new List<OAuthTranslation>();
            LogoUris = new List<OAuthTranslation>();
            PolicyUris = new List<OAuthTranslation>();
            TosUris = new List<OAuthTranslation>();
        }

        /// <summary>
        /// Client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client secrets.
        /// </summary>
        public ICollection<ClientSecret> Secrets { get; set; }

        /// <summary>
        /// One or more human readable client name
        /// </summary>
        public ICollection<OAuthTranslation> ClientNames { get; set; }

        /// <summary>
        /// One or more URL that references a logo for the client
        /// </summary>
        public ICollection<OAuthTranslation> LogoUris { get; set; }

        /// <summary>
        /// One or more URL of a web page providing information about the client.
        /// </summary>
        public ICollection<OAuthTranslation> ClientUris { get; set; }

        /// <summary>
        /// One or more URL that points to a human-readable policy document for the client
        /// </summary>
        public ICollection<OAuthTranslation> PolicyUris { get; set; }

        /// <summary>
        /// One or more URL that points to a human-readable terms of service document for the client
        /// </summary>
        public ICollection<OAuthTranslation> TosUris { get; set; }

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
        public ICollection<string> GrantTypes { get; set; }

        /// <summary>
        /// Array of the OAUTH2.0 response type strings that the client can use at the authorization endpoint.
        /// </summary>
        public IEnumerable<string> ResponseTypes { get; set; }

        /// <summary>
        /// Scope values that the client can use when requesting access tokens.
        /// </summary>
        public IEnumerable<OAuthScope> AllowedScopes { get; set; }

        /// <summary>
        /// Array of redirection URIS for use in redirect-based flows.
        /// </summary>
        public ICollection<string> RedirectionUrls { get; set; }

        /// <summary>
        /// Array of URLs supplied by the RP to which it MAY request that the End-User's User Agent be redirected using the post_logout_redirect_uri parameter after a logout has been performed.
        /// </summary>
        public ICollection<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// URI string referencing the client’s JSON Web Key (JWK) Set document, which contains the client’s public keys.
        /// </summary>
        public string JwksUri { get; set; }

        /// <summary>
        /// Client’s JSON Web Key Set document value, which contains the client’s public keys.
        /// </summary>
        public ICollection<JsonWebKey> JsonWebKeys { get; set; }

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
        public ICollection<string> Contacts { get; set; }

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

        public void AddClientName(string language, string value)
        {
            ClientNames.Add(new OAuthTranslation($"{ClientId}_client_name", value, language));
        }

        public void AddClientUri(string language, string value)
        {
            ClientUris.Add(new OAuthTranslation($"{ClientId}_client_uri", value, language));
        }

        public void AddLogoUri(string language, string value)
        {
            LogoUris.Add(new OAuthTranslation($"{ClientId}_logo_uri", value, language));
        }

        public void AddTosUri(string language, string value)
        {
            TosUris.Add(new OAuthTranslation($"{ClientId}_tos_uri", value, language));
        }

        public void AddPolicyUri(string language, string value)
        {
            PolicyUris.Add(new OAuthTranslation($"{ClientId}_policy_uri", value, language));
        }

        public void AddSharedSecret(string secret, DateTime? expirationDateTime)
        {
            Secrets.Add(new ClientSecret(ClientSecretTypes.SharedSecret, secret, expirationDateTime));
        }

        /// <summary>
        /// Get all the redirection urls.
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<string>> GetRedirectionUrls(IHttpClientFactory httpClientFactory)
        {
            IEnumerable<string> result = RedirectionUrls == null ? new List<string>() : RedirectionUrls.ToList();
            return Task.FromResult(result);
        }

        public virtual object Clone()
        {
            return new OAuthClient
            {
                ClientId = ClientId,
                ClientNames = ClientNames == null ? new List<OAuthTranslation>() : ClientNames.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                ClientUris = ClientUris == null ? new List<OAuthTranslation>() : ClientUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                LogoUris = LogoUris == null ? new List<OAuthTranslation>() : LogoUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                PolicyUris = PolicyUris == null ? new List<OAuthTranslation>() : PolicyUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                TosUris = TosUris == null ? new List<OAuthTranslation>() : TosUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                JwksUri = JwksUri,
                RefreshTokenExpirationTimeInSeconds = RefreshTokenExpirationTimeInSeconds,
                UpdateDateTime = UpdateDateTime,
                TokenEndPointAuthMethod = TokenEndPointAuthMethod,
                TokenExpirationTimeInSeconds = TokenExpirationTimeInSeconds,
                Secrets = Secrets == null ? new List<ClientSecret>() : Secrets.Select(s => (ClientSecret)s.Clone()).ToList(),
                AllowedScopes = AllowedScopes == null ? new List<OAuthScope>() : AllowedScopes.Select(s => (OAuthScope)s.Clone()).ToList(),
                JsonWebKeys = JsonWebKeys == null ? new List<JsonWebKey>() : JsonWebKeys.Select(j => (JsonWebKey)j.Clone()).ToList(),
                GrantTypes = GrantTypes.ToList(),
                RedirectionUrls = RedirectionUrls.ToList(),
                PreferredTokenProfile = PreferredTokenProfile,
                TokenEncryptedResponseAlg = TokenEncryptedResponseAlg,
                TokenEncryptedResponseEnc = TokenEncryptedResponseEnc,
                TokenSignedResponseAlg = TokenSignedResponseAlg,
                ResponseTypes = ResponseTypes.ToList(),
                Contacts = Contacts.ToList(),
                SoftwareId = SoftwareId,
                SoftwareVersion = SoftwareVersion,
                RegistrationAccessToken = RegistrationAccessToken,
                PostLogoutRedirectUris = PostLogoutRedirectUris.ToList()
            };
        }

        public bool Equals(OAuthClient other)
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
