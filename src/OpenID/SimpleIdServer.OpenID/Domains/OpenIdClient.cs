using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Infrastructures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Domains
{
    public class OpenIdClient : OAuthClient
    {
        public OpenIdClient()
        {
            AllowedScopes = new List<OpenIdScope>();
            DefaultAcrValues = new List<string>();
        }

        /// <summary>
        /// Kind of the application. The default, if omitted, is web. The defined values are “native” or “web”. 
        /// </summary>
        public string ApplicationType { get; set; }

        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS identity token.
        /// </summary>
        public string IdTokenEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS identity token.
        /// </summary>
        public string IdTokenEncryptedResponseEnc { get; set; }

        /// <summary>
        /// Cryptographic algorithm used to secure the JWS identity token. 
        /// </summary>
        public string IdTokenSignedResponseAlg { get; set; }

        /// <summary>
        /// Required for signing UserInfo responses.
        /// </summary>
        public string UserInfoSignedResponseAlg { get; set; }

        /// <summary>
        /// Required for encrypting the identity token issued to this client.
        /// </summary>
        public string UserInfoEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Required for encrypting the identity token issued to this client.
        /// </summary>
        public string UserInfoEncryptedResponseEnc { get; set; }

        /// <summary>
        /// Must be used for signing Request Objects sent to the OpenID provider.
        /// </summary>
        public string RequestObjectSigningAlg { get; set; }

        /// <summary>
        /// JWE alg algorithm the relying party is declaring that it may use for encrypting Request Objects sent to the OpenID provider
        /// </summary>
        public string RequestObjectEncryptionAlg { get; set; }

        /// <summary>
        /// JWE enc algorithm the relying party is declaring that it may use for encrypting request objects sent to the OpenID provider.
        /// </summary>
        public string RequestObjectEncryptionEnc { get; set; }

        /// <summary>
        /// subject_type requested for responses to this client. Possible values are “pairwise” or “public”.
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// Default Maximum Authentication Age.
        /// </summary>
        public double? DefaultMaxAge { get; set; }

        /// <summary>
        /// Default requested Authentication Context Class Reference values.
        /// </summary>
        public ICollection<string> DefaultAcrValues { get; set; }

        /// <summary>
        /// Default requested Authentication Context Class Reference values.
        /// </summary>
        public bool RequireAuthTime { get; set; }

        /// <summary>
        /// URI using the HTTPS scheme to be used in calculating Pseudonymous Identifiers by the OpenID provider.
        /// </summary>
        public string SectorIdentifierUri { get; set; }

        /// <summary>
        /// SALT used to calculate the pairwise.
        /// </summary>
        public string PairWiseIdentifierSalt { get; set; }

        /// <summary>
        /// OPENID scopes.
        /// </summary>
        public IEnumerable<OpenIdScope> AllowedOpenIdScopes => (IEnumerable<OpenIdScope>)AllowedScopes;

        /// <summary>
        /// Resolve redirection urls.
        /// </summary>
        /// <returns></returns>
        public async override Task<IEnumerable<string>> GetRedirectionUrls(IHttpClientFactory httpClientFactory)
        {
            var result = (await base.GetRedirectionUrls(httpClientFactory)).ToList();
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

        public override object Clone()
        {
            return new OpenIdClient
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
                AllowedScopes = AllowedScopes == null ? new List<OpenIdScope>() : AllowedScopes.Select(s => (OpenIdScope)s.Clone()).ToList(),
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
                ApplicationType = ApplicationType,
                DefaultAcrValues = DefaultAcrValues.ToList(),
                DefaultMaxAge = DefaultMaxAge,
                IdTokenEncryptedResponseAlg = IdTokenEncryptedResponseAlg,
                IdTokenEncryptedResponseEnc = IdTokenEncryptedResponseEnc,
                IdTokenSignedResponseAlg = IdTokenSignedResponseAlg,
                PairWiseIdentifierSalt = PairWiseIdentifierSalt,
                RequestObjectEncryptionAlg = RequestObjectEncryptionAlg,
                RequestObjectEncryptionEnc = RequestObjectEncryptionEnc,
                RequestObjectSigningAlg = RequestObjectSigningAlg,
                RequireAuthTime = RequireAuthTime,
                SectorIdentifierUri = SectorIdentifierUri,
                SubjectType = SubjectType,
                UserInfoEncryptedResponseAlg = UserInfoEncryptedResponseAlg,
                UserInfoEncryptedResponseEnc = UserInfoEncryptedResponseEnc,
                UserInfoSignedResponseAlg = UserInfoSignedResponseAlg
            };
        }
    }
}
