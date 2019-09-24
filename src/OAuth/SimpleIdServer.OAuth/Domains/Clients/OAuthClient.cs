// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains.Clients
{
    public enum ClientSecretTypes
    {
        SharedSecret = 0,
        X509Thumbprint = 1,
        X509Name = 2
    }

    public class ClientSecret : ICloneable
    {
        public ClientSecret(ClientSecretTypes type, string value)
        {
            Type = type;
            Value = value;
        }

        public ClientSecretTypes Type { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            return new ClientSecret(Type, Value);
        }
    }

    public class OAuthClient : ICloneable, IEquatable<OAuthClient>
    {
        public OAuthClient()
        {
            GrantTypes = new List<string>();
            RedirectionUrls = new List<string>();
            Contacts = new List<string>();
            RequestUris = new List<string>();
            PostLogoutRedirectUris = new List<string>();
            ResponseTypes = new List<string>();
            DefaultAcrValues = new List<string>();
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
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secrets.
        /// </summary>
        public ICollection<ClientSecret> Secrets { get; set; }

        /// <summary>
        /// Gets or sets the client names.
        /// </summary>
        public ICollection<OAuthTranslation> ClientNames { get; set; }

        /// <summary>
        /// Gets or sets the logo uris.
        /// </summary>
        public ICollection<OAuthTranslation> LogoUris { get; set; }

        /// <summary>
        /// Gets or sets the home page of the client.
        /// </summary>
        public ICollection<OAuthTranslation> ClientUris { get; set; }

        /// <summary>
        /// Gets or sets the URL that the RP provides to the End-User to read about the how the profile data will be used.
        /// </summary>
        public ICollection<OAuthTranslation> PolicyUris { get; set; }

        /// <summary>
        /// Gets or sets the URL that the RP provides to the End-User to read about the RP's terms of service.
        /// </summary>
        public ICollection<OAuthTranslation> TosUris { get; set; }

        #region Encryption mechanism for ACCESS TOKEN

        /// <summary>
        /// Gets or sets the JWS alg algorithm for signing the ID token issued to this client.
        /// The default is RS256. The public key for validating the signature is provided by retrieving the JWK Set referenced by the JWKS_URI
        /// </summary>
        public string TokenSignedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the JWE alg algorithm. REQUIRED for encrypting the ID token issued to this client.
        /// The default is that no encryption is performed
        /// </summary>
        public string TokenEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the JWE enc algorithm. REQUIRED for encrypting the ID token issued to this client.
        /// If IdTokenEncryptedResponseAlg is specified then the value is A128CBC-HS256
        /// </summary>
        public string TokenEncryptedResponseEnc { get; set; }

        #endregion

        #region Encryption mechanism for ID TOKEN

        /// <summary>
        /// Gets or sets the JWS alg algorithm for signing the ID token issued to this client.
        /// The default is RS256. The public key for validating the signature is provided by retrieving the JWK Set referenced by the JWKS_URI
        /// </summary>
        public string IdTokenSignedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the JWE alg algorithm. REQUIRED for encrypting the ID token issued to this client.
        /// The default is that no encryption is performed
        /// </summary>
        public string IdTokenEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the JWE enc algorithm. REQUIRED for encrypting the ID token issued to this client.
        /// If IdTokenEncryptedResponseAlg is specified then the value is A128CBC-HS256
        /// </summary>
        public string IdTokenEncryptedResponseEnc { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the client authentication method for the Token Endpoint. 
        /// </summary>
        public string TokenEndPointAuthMethod { get; set; }

        /// <summary>
        /// Gets or sets an array containing a list of OAUTH2.0 grant types
        /// </summary>
        public ICollection<string> GrantTypes { get; set; }

        /// <summary>
        /// Gets or sets the response types.
        /// </summary>
        public IEnumerable<string> ResponseTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of OAUTH2.0 grant_types.
        /// </summary>
        public ICollection<OAuthScope> AllowedScopes { get; set; }

        /// <summary>
        /// Gets or sets an array of Redirection URI values used by the client.
        /// </summary>
        public ICollection<string> RedirectionUrls { get; set; }

        /// <summary>
        /// Gets or sets the type of application
        /// </summary>
        public string ApplicationType { get; set; }

        /// <summary>
        /// Url for the Client's JSON Web Key Set document
        /// </summary>
        public string JwksUri { get; set; }

        /// <summary>
        /// Gets or sets the list of json web keys
        /// </summary>
        public ICollection<JsonWebKey> JsonWebKeys { get; set; }

        /// <summary>
        /// Gets or sets the list of contacts
        /// </summary>
        public IEnumerable<string> Contacts { get; set; }

        /// <summary>
        /// Get or set the sector identifier uri
        /// </summary>
        public string SectorIdentifierUri { get; set; }

        /// <summary>
        /// Gets or sets the subject type
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// Gets or sets the user info signed response algorithm
        /// </summary>
        public string UserInfoSignedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the user info encrypted response algorithm
        /// </summary>
        public string UserInfoEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the user info encrypted response enc
        /// </summary>
        public string UserInfoEncryptedResponseEnc { get; set; }

        /// <summary>
        /// Gets or sets the request objects signing algorithm
        /// </summary>
        public string RequestObjectSigningAlg { get; set; }

        /// <summary>
        /// Gets or sets the request object encryption algorithm
        /// </summary>
        public string RequestObjectEncryptionAlg { get; set; }

        /// <summary>
        /// Gets or sets the request object encryption enc
        /// </summary>
        public string RequestObjectEncryptionEnc { get; set; }

        /// <summary>
        /// Gets or sets the token endpoint authentication signing algorithm
        /// </summary>
        public string TokenEndPointAuthSigningAlg { get; set; }

        /// <summary>
        /// Gets or sets the default max age
        /// </summary>
        public double? DefaultMaxAge { get; set; }

        /// <summary>
        /// Gets or sets the require authentication time
        /// </summary>
        public bool RequireAuthTime { get; set; }

        /// <summary>
        /// Gets or sets the default acr values
        /// </summary>
        public ICollection<string> DefaultAcrValues { get; set; }

        /// <summary>
        /// Gets or sets the initiate login uri
        /// </summary>
        public string InitiateLoginUri { get; set; }

        /// <summary>
        /// Gets or sets the list of request uris
        /// </summary>
        public ICollection<string> RequestUris { get; set; }

        /// <summary>
        /// Gets or sets use SCIM protocol to access user information.
        /// </summary>
        public bool ScimProfile { get; set; }

        /// <summary>
        /// Client require PKCE.
        /// </summary>
        public bool RequirePkce { get; set; }

        /// <summary>
        /// Access / Identity token expiration time in seconds.
        /// </summary>
        public double TokenExpirationTimeInSeconds { get; set; }

        /// <summary>
        /// Refresh token expiration time in seconds.
        /// </summary>
        public double RefreshTokenExpirationTimeInSeconds { get; set; }

        /// <summary>
        /// Get or set the pair wise salt.
        /// </summary>
        public string PairWiseIdentifierSalt { get; set; }

        /// <summary>
        /// Get or sets the post logout redirect uris.
        /// </summary>
        public ICollection<string> PostLogoutRedirectUris { get; set; }
        
        /// <summary>
        /// Gets or sets the create date time.
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// Gets or sets the update date time.
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// Gets or sets the preferred token profile.
        /// </summary>
        public string PreferredTokenProfile { get; set; }

        /// <summary>
        /// Gets or sets the token profile algorithm.
        /// </summary>
        public string TokenProfileAlg { get; set; }

        /// <summary>
        /// Gets or sets "offline access enabled".
        /// </summary>
        public bool IsOfflineAccessEnabled { get; set; }

        /// <summary>
        /// Gets or sets the software identifier.
        /// </summary>
        public string SoftwareId { get; set; }

        /// <summary>
        /// Gets or sets the sofware version.
        /// </summary>
        public string SoftwareVersion { get; set; }

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

        public void AddSharedSecret(string secret)
        {
            Secrets.Add(new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash(secret)));
        }

        /// <summary>
        /// Get all the redirection urls.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetRedirectionUrls(IHttpClientFactory httpClientFactory)
        {
            var result = RedirectionUrls == null ? new List<string>() : RedirectionUrls.ToList();
            result.AddRange(await GetSectorIdentifierUrls(httpClientFactory));
            return result;
        }

        public async Task<IEnumerable<string>> GetSectorIdentifierUrls(IHttpClientFactory httpClientFactory)
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(SectorIdentifierUri))
            {
                using (var httpClient = httpClientFactory.GetHttpClient())
                {
                    var httpResult = await httpClient.GetAsync(SectorIdentifierUri);
                    if (httpResult.IsSuccessStatusCode)
                    {
                        var json = await httpResult.Content.ReadAsStringAsync();
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var jArr = JsonConvert.DeserializeObject<JArray>(json);
                            if (jArr != null)
                            {
                                foreach (var record in jArr)
                                {
                                    result.Add(record.ToString());
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public object Clone()
        {
            return new OAuthClient
            {
                ApplicationType = ApplicationType,
                ClientId = ClientId,
                ClientNames = ClientNames == null ? new List<OAuthTranslation>() : ClientNames.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                ClientUris = ClientUris == null ? new List<OAuthTranslation>() : ClientUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                LogoUris = LogoUris == null ? new List<OAuthTranslation>() : LogoUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                PolicyUris = PolicyUris == null ? new List<OAuthTranslation>() : PolicyUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                TosUris = TosUris == null ? new List<OAuthTranslation>() : TosUris.Select(c => (OAuthTranslation)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                DefaultAcrValues = DefaultAcrValues,
                DefaultMaxAge = DefaultMaxAge,
                IdTokenEncryptedResponseAlg = IdTokenEncryptedResponseAlg,
                IdTokenEncryptedResponseEnc = IdTokenEncryptedResponseEnc,
                IdTokenSignedResponseAlg = IdTokenSignedResponseAlg,
                InitiateLoginUri = InitiateLoginUri,
                JwksUri = JwksUri,
                RefreshTokenExpirationTimeInSeconds = RefreshTokenExpirationTimeInSeconds,
                RequestObjectEncryptionAlg = RequestObjectEncryptionAlg,
                RequestObjectEncryptionEnc = RequestObjectEncryptionEnc,
                RequestObjectSigningAlg = RequestObjectSigningAlg,
                RequireAuthTime = RequireAuthTime,
                RequirePkce = RequirePkce,
                ScimProfile = ScimProfile,
                SectorIdentifierUri = SectorIdentifierUri,
                UpdateDateTime = UpdateDateTime,
                TokenEndPointAuthMethod = TokenEndPointAuthMethod,
                SubjectType = SubjectType,
                TokenEndPointAuthSigningAlg = TokenEndPointAuthSigningAlg,
                TokenExpirationTimeInSeconds = TokenExpirationTimeInSeconds,
                UserInfoEncryptedResponseAlg = UserInfoEncryptedResponseAlg,
                UserInfoEncryptedResponseEnc = UserInfoEncryptedResponseEnc,
                UserInfoSignedResponseAlg = UserInfoSignedResponseAlg,
                Secrets = Secrets == null ? new List<ClientSecret>() : Secrets.Select(s => (ClientSecret)s.Clone()).ToList(),
                AllowedScopes = AllowedScopes == null ? new List<OAuthScope>() : AllowedScopes.Select(s => (OAuthScope)s.Clone()).ToList(),
                JsonWebKeys = JsonWebKeys == null ? new List<JsonWebKey>() : JsonWebKeys.Select(j => (JsonWebKey)j.Clone()).ToList(),
                GrantTypes = GrantTypes.ToList(),
                RequestUris = RequestUris.ToList(),
                RedirectionUrls = RedirectionUrls.ToList(),
                Contacts = Contacts.ToList(),
                PostLogoutRedirectUris = PostLogoutRedirectUris.ToList(),
                PreferredTokenProfile = PreferredTokenProfile,
                PairWiseIdentifierSalt = PairWiseIdentifierSalt,
                TokenProfileAlg = TokenProfileAlg,
                IsOfflineAccessEnabled = IsOfflineAccessEnabled,
                SoftwareId = SoftwareId,
                SoftwareVersion = SoftwareVersion,
                TokenEncryptedResponseAlg = TokenEncryptedResponseAlg,
                TokenEncryptedResponseEnc = TokenEncryptedResponseEnc,
                TokenSignedResponseAlg = TokenSignedResponseAlg,
                ResponseTypes = ResponseTypes.ToList()
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
