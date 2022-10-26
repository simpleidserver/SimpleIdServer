// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthClient : BaseClient, ICloneable
    {
        #region Properties

        public ICollection<OAuthScope> OAuthAllowedScopes { get; set; }
        public override ICollection<OAuthScope> AllowedScopes
        {
            get
            {
                return OAuthAllowedScopes;
            }
            set
            {
                OAuthAllowedScopes = value;
            }
        }

        #endregion

        #region Actions

        public override void SetAllowedScopes(ICollection<OAuthScope> scopes)
        {
            OAuthAllowedScopes.Clear();
            foreach (var scope in scopes)
            {
                OAuthAllowedScopes.Add(scope);
            }
        }

        #endregion

        public object Clone()
        {
            return new OAuthClient
            {
                ClientId = ClientId,
                Translations = Translations == null ? new List<OAuthClientTranslation>() : Translations.Select(t => (OAuthClientTranslation)t.Clone()).ToList(),
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
                RegistrationAccessToken = RegistrationAccessToken,
                PostLogoutRedirectUris = PostLogoutRedirectUris.ToList(),
                TlsClientAuthSanDNS = TlsClientAuthSanDNS,
                TlsClientAuthSanEmail = TlsClientAuthSanEmail,
                TlsClientAuthSanIP = TlsClientAuthSanIP,
                TlsClientAuthSanURI = TlsClientAuthSanURI,
                TlsClientAuthSubjectDN = TlsClientAuthSubjectDN,
                TlsClientCertificateBoundAccessToken = TlsClientCertificateBoundAccessToken,
                IsConsentDisabled = IsConsentDisabled
            };
        }
    }
}
