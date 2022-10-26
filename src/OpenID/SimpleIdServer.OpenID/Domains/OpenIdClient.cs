// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Domains
{
    public class OpenIdClient : BaseClient, ICloneable
    {
        public OpenIdClient() : base()
        {
            DefaultAcrValues = new List<string>();
        }

        #region Properties

        /// <summary>
        /// Kind of the application. The default, if omitted, is web. The defined values are “native” or “web”. 
        /// </summary>
        public string ApplicationType { get; set; }

        /// <summary>
        /// Kind of application, spa etc...
        /// </summary>
        public ApplicationKinds? ApplicationKind { get; set; }

        public string ApplicationTypeCategory { get; set; }

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
        public IEnumerable<string> DefaultAcrValues { get; set; }

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
        /// URI using the https scheme that a third party can use to initiate a login by the RP.
        /// </summary>
        public string InitiateLoginUri { get; set; }
        /// <summary>
        /// One of the following values: poll, ping or push.
        /// </summary>
        public string BCTokenDeliveryMode { get; set; }
        /// <summary>
        /// This is the endpoint to which the OP will post a notification after a successful or failed end-user authentication.
        /// </summary>
        public string BCClientNotificationEndpoint { get; set; }
        /// <summary>
        /// The JWS algorithm alg value that the Client will use for signing authentication request.
        /// When omitted, the Client will not send signed authentication requests.
        /// </summary>
        public string BCAuthenticationRequestSigningAlg { get; set; }
        /// <summary>
        /// Boolean value specifying whether the Client supports the user_code parameter. 
        /// </summary>
        public bool BCUserCodeParameter { get; set; }
        /// <summary>
        /// RP URL that will cause the RP to log itself out when rendered in an iframe by the OP.
        /// </summary>
        public string FrontChannelLogoutUri { get; set; }
        /// <summary>
        /// Boolean value specifying whether the RP requires that iss (issuer) and sid (session id) query parameters be included to identify the RP session
        /// with the OP when the front_channel_logout_uri is used.
        /// </summary>
        public bool FrontChannelLogoutSessionRequired { get; set; }
        /// <summary>
        /// Boolean value specifying whether the OP can pass a SID claim in the Logout token to identify the RP session with the OP.
        /// If supported, the sid claim is also included in ID tokens issued by the OP.
        /// </summary>
        public bool BackChannelLogoutSessionRequired { get; set; }
        public string BackChannelLogoutUri { get; set; }
        public ICollection<OpenIdClientScope> OpenIdAllowedScopes { get; set; }
        public override ICollection<OAuthScope> AllowedScopes
        {
            get
            {
                return OpenIdAllowedScopes.Select(o => o.Scope).ToList();
            }
            set
            {
                OpenIdAllowedScopes = value.Select(v => new OpenIdClientScope
                {
                    Scope = v
                }).ToList();
            }
        }

        #endregion

        public static OpenIdClient Create(
            ApplicationKinds applicationKind, 
            string clientName, 
            string language, 
            DateTime? clientSecretExpirationTime,
            int tokenExpirationTimeInSeconds,
            int refreshTokenExpirationTimeInSeconds)
        {
            var result = new OpenIdClient
            {
                ClientId = Guid.NewGuid().ToString(),
                ApplicationKind = applicationKind,
                TokenExpirationTimeInSeconds = tokenExpirationTimeInSeconds,
                RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds
            };
            result.SetClientSecret(Guid.NewGuid().ToString(), clientSecretExpirationTime);
            result.AddClientName(language, clientName);
            switch(applicationKind)
            {
                case ApplicationKinds.Native:
                case ApplicationKinds.SPA:
                    result.TokenEndPointAuthMethod = OAuthPKCEAuthenticationHandler.AUTH_METHOD;
                    result.GrantTypes = new string[] { AuthorizationCodeHandler.GRANT_TYPE };
                    break;
                case ApplicationKinds.Service:
                case ApplicationKinds.Web:
                case ApplicationKinds.Trusted:
                    result.TokenEndPointAuthMethod = OAuthClientSecretBasicAuthenticationHandler.AUTH_METHOD;
                    if (applicationKind == ApplicationKinds.Trusted)
                    {
                        result.GrantTypes = new string[] { PasswordHandler.GRANT_TYPE };
                    }
                    else if (applicationKind == ApplicationKinds.Web)
                    {
                        result.GrantTypes = new string[] { AuthorizationCodeHandler.GRANT_TYPE };
                    }
                    else if (applicationKind == ApplicationKinds.Service)
                    {
                        result.GrantTypes = new string[] { ClientCredentialsHandler.GRANT_TYPE };
                    }
                    break;
            }
            result.UpdateDateTime = DateTime.UtcNow;
            result.CreateDateTime = DateTime.UtcNow;
            return result;
        }

        public override void SetAllowedScopes(ICollection<OAuthScope> scopes)
        {
            OpenIdAllowedScopes.Clear();
            foreach (var scope in scopes)
            {
                OpenIdAllowedScopes.Add(new OpenIdClientScope
                {
                    Scope = scope
                });
            }
        }

        /// <summary>
        /// Resolve redirection urls.
        /// </summary>
        /// <returns></returns>
        public async override Task<IEnumerable<string>> GetRedirectionUrls(IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
        {
            var result = (await base.GetRedirectionUrls(httpClientFactory, cancellationToken)).ToList();
            result.AddRange(await GetSectorIdentifierUrls(httpClientFactory, cancellationToken));
            return result;
        }

        public async Task<IEnumerable<string>> GetSectorIdentifierUrls(IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(SectorIdentifierUri))
            {
                using (var httpClient = httpClientFactory.GetHttpClient())
                {
                    var httpResult = await httpClient.GetAsync(SectorIdentifierUri, cancellationToken);
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
            return new OpenIdClient
            {
                ClientId = ClientId,
                Translations = Translations == null ? new List<OAuthClientTranslation>() : Translations.Select(c => (OAuthClientTranslation)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                JwksUri = JwksUri,
                RefreshTokenExpirationTimeInSeconds = RefreshTokenExpirationTimeInSeconds,
                UpdateDateTime = UpdateDateTime,
                TokenEndPointAuthMethod = TokenEndPointAuthMethod,
                TokenExpirationTimeInSeconds = TokenExpirationTimeInSeconds,
                ClientSecret = ClientSecret,
                ClientSecretExpirationTime = ClientSecretExpirationTime,
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
                UserInfoSignedResponseAlg = UserInfoSignedResponseAlg,
                RegistrationAccessToken = RegistrationAccessToken,
                PostLogoutRedirectUris = PostLogoutRedirectUris,
                InitiateLoginUri = InitiateLoginUri,
                BCTokenDeliveryMode = BCTokenDeliveryMode,
                BCClientNotificationEndpoint = BCClientNotificationEndpoint,
                BCAuthenticationRequestSigningAlg = BCAuthenticationRequestSigningAlg,
                BCUserCodeParameter = BCUserCodeParameter,
                FrontChannelLogoutUri = FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = FrontChannelLogoutSessionRequired,
                BackChannelLogoutSessionRequired = BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = BackChannelLogoutUri,
                TlsClientAuthSanDNS = TlsClientAuthSanDNS,
                TlsClientAuthSanEmail = TlsClientAuthSanEmail,
                TlsClientAuthSanIP = TlsClientAuthSanIP,
                TlsClientAuthSanURI = TlsClientAuthSanURI,
                TlsClientAuthSubjectDN = TlsClientAuthSubjectDN,
                TlsClientCertificateBoundAccessToken = TlsClientCertificateBoundAccessToken,
                ApplicationKind = ApplicationKind,
                IsConsentDisabled = IsConsentDisabled,
            };
        }
    }
}
