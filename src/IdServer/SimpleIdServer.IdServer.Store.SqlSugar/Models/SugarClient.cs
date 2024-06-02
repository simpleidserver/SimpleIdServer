// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models
{
    [SugarTable("Clients")]
    public class SugarClient
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string? RegistrationAccessToken { get; set; } = null;
        public string GrantTypes { get; set; }
        public string RedirectionUrls { get; set; }
        public string? TokenEndPointAuthMethod { get; set; } = null;
        public string ResponseTypes { get; set; }
        public string? JwksUri { get; set; } = null;
        public string Contacts { get; set; }
        public string? SoftwareId { get; set; } = null;
        public string? SoftwareVersion { get; set; } = null;
        public string? TlsClientAuthSubjectDN { get; set; } = null;
        public string? TlsClientAuthSanDNS { get; set; } = null;
        public string? TlsClientAuthSanURI { get; set; } = null;
        public string? TlsClientAuthSanIP { get; set; } = null;
        public string? TlsClientAuthSanEmail { get; set; } = null;
        public string? AuthenticationContextClassReferenceId { get; set; } = null;
        public DateTime? ClientSecretExpirationTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool IsTokenExchangeEnabled { get; set; }
        public TokenExchangeTypes? TokenExchangeType { get; set; }
        public double? TokenExpirationTimeInSeconds { get; set; }
        public double? CNonceExpirationTimeInSeconds { get; set; }
        public double? RefreshTokenExpirationTimeInSeconds { get; set; }
        public string? TokenSignedResponseAlg { get; set; } = null;
        public string? TokenEncryptedResponseAlg { get; set; } = null;
        public string? TokenEncryptedResponseEnc { get; set; } = null;
        public string PostLogoutRedirectUris { get; set; }
        public bool RedirectToRevokeSessionUI { get; set; }
        public string? PreferredTokenProfile { get; set; } = null;
        public string? RequestObjectSigningAlg { get; set; } = null;
        public string? RequestObjectEncryptionAlg { get; set; } = null;
        public string? RequestObjectEncryptionEnc { get; set; } = null;
        public string? SubjectType { get; set; } = null;
        public string? PairWiseIdentifierSalt { get; set; } = null;
        public string? SectorIdentifierUri { get; set; } = null;
        public string? IdTokenSignedResponseAlg { get; set; } = null;
        public string? IdTokenEncryptedResponseAlg { get; set; } = null;
        public string? IdTokenEncryptedResponseEnc { get; set; } = null;
        public string? BCTokenDeliveryMode { get; set; } = null;
        public string? BCClientNotificationEndpoint { get; set; } = null;
        public string? BCAuthenticationRequestSigningAlg { get; set; }
        public string? UserInfoSignedResponseAlg { get; set; } = null;
        public string? UserInfoEncryptedResponseAlg { get; set; } = null;
        public string? UserInfoEncryptedResponseEnc { get; set; } = null;
        public bool BCUserCodeParameter { get; set; }
        public string? FrontChannelLogoutUri { get; set; } = null;
        public string? CredentialOfferEndpoint { get; set; } = null;
        public bool IsTransactionCodeRequired { get; set; }
        public double PreAuthCodeExpirationTimeInSeconds { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; }
        public double? DefaultMaxAge { get; set; } = null;
        public bool TlsClientCertificateBoundAccessToken { get; set; }
        public string? BackChannelLogoutUri { get; set; } = null;
        public string? ApplicationType { get; set; } = null;
        public string? InitiateLoginUri { get; set; }
        public bool RequireAuthTime { get; set; }
        public string? AuthorizationSignedResponseAlg { get; set; } = null;
        public string? AuthorizationEncryptedResponseAlg { get; set; } = null;
        public string AuthorizationDataTypes { get; set; }
        public string? AuthorizationEncryptedResponseEnc { get; set; } = null;
        public bool DPOPBoundAccessTokens { get; set; }
        public bool IsConsentDisabled { get; set; }
        public bool IsResourceParameterRequired { get; set; }
        public int AuthReqIdExpirationTimeInSeconds { get; set; }
        public string? ClientType { get; set; } = null;
        public bool IsDPOPNonceRequired { get; set; }
        public double DPOPNonceLifetimeInSeconds { get; set; }
        public bool IsRedirectUrlCaseSensitive { get; set; }
        public string? SerializedParameters { get; set; } = null;
        public string DefaultAcrValues { get; set; }
        public int BCIntervalSeconds { get; set; }
        public AccessTokenTypes AccessTokenType { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(SugarClientScope.ClientsId))]
        public List<SugarClientScope> ClientScopes { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(SugarClientJsonWebKey.ClientId))]
        public List<SugarClientJsonWebKey> SerializedJsonWebKeys { get; set; }
        [Navigate(typeof(SugarClientRealm), nameof(SugarClientRealm.ClientsId), nameof(SugarClientRealm.RealmsName))]
        public List<SugarRealm> Realms { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(SugarDeviceAuthCode.ClientId))]
        public List<SugarDeviceAuthCode> DeviceAuthCodes { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(SugarTranslation.ClientId))]
        public List<SugarTranslation> Translations { get; set; }

        public static SugarClient Transform(Client client)
        {
            return new SugarClient
            {
                AccessTokenType = client.AccessTokenType,
                ApplicationType = client.ApplicationType,
                AuthorizationEncryptedResponseAlg = client.AuthorizationEncryptedResponseAlg,
                AuthorizationEncryptedResponseEnc = client.AuthorizationEncryptedResponseEnc,
                AuthorizationSignedResponseAlg = client.AuthorizationSignedResponseAlg,
                AuthReqIdExpirationTimeInSeconds = client.AuthReqIdExpirationTimeInSeconds,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                BCAuthenticationRequestSigningAlg = client.BCAuthenticationRequestSigningAlg,
                BCClientNotificationEndpoint = client.BCClientNotificationEndpoint,
                BCIntervalSeconds = client.BCIntervalSeconds,
                BCTokenDeliveryMode = client.BCTokenDeliveryMode,
                BCUserCodeParameter = client.BCUserCodeParameter,
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                ClientSecretExpirationTime = client.ClientSecretExpirationTime,
                ClientType = client.ClientType,
                CNonceExpirationTimeInSeconds = client.CNonceExpirationTimeInSeconds,
                CreateDateTime = client.CreateDateTime,
                CredentialOfferEndpoint = client.CredentialOfferEndpoint,
                DefaultMaxAge = client.DefaultMaxAge,
                DPOPBoundAccessTokens = client.DPOPBoundAccessTokens,
                DPOPNonceLifetimeInSeconds = client.DPOPNonceLifetimeInSeconds,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                IdTokenEncryptedResponseAlg = client.IdTokenEncryptedResponseAlg,
                IdTokenEncryptedResponseEnc = client.IdTokenEncryptedResponseEnc,
                IdTokenSignedResponseAlg = client.IdTokenSignedResponseAlg,
                InitiateLoginUri = client.InitiateLoginUri,
                IsConsentDisabled = client.IsConsentDisabled,
                IsDPOPNonceRequired = client.IsDPOPNonceRequired,
                IsRedirectUrlCaseSensitive = client.IsRedirectUrlCaseSensitive,
                Id = client.Id,
                IsResourceParameterRequired = client.IsResourceParameterRequired,
                IsTokenExchangeEnabled = client.IsTokenExchangeEnabled,
                IsTransactionCodeRequired = client.IsTransactionCodeRequired,
                JwksUri = client.JwksUri,
                PairWiseIdentifierSalt = client.PairWiseIdentifierSalt,
                PreAuthCodeExpirationTimeInSeconds = client.PreAuthCodeExpirationTimeInSeconds,
                PreferredTokenProfile = client.PreferredTokenProfile,
                RedirectToRevokeSessionUI = client.RedirectToRevokeSessionUI,
                RefreshTokenExpirationTimeInSeconds = client.RefreshTokenExpirationTimeInSeconds,
                RegistrationAccessToken = client.RegistrationAccessToken,
                RequestObjectEncryptionAlg = client.RequestObjectEncryptionAlg,
                RequestObjectEncryptionEnc = client.RequestObjectEncryptionEnc,
                RequestObjectSigningAlg = client.RequestObjectSigningAlg,
                RequireAuthTime = client.RequireAuthTime,
                SectorIdentifierUri = client.SectorIdentifierUri,
                SoftwareId = client.SoftwareId,
                SoftwareVersion = client.SoftwareVersion,
                SubjectType = client.SubjectType,
                TlsClientAuthSanDNS = client.TlsClientAuthSanDNS,
                TlsClientAuthSanEmail = client.TlsClientAuthSanEmail,
                TlsClientAuthSanIP = client.TlsClientAuthSanIP,
                TlsClientAuthSanURI = client.TlsClientAuthSanURI,
                TlsClientAuthSubjectDN = client.TlsClientAuthSubjectDN,
                TlsClientCertificateBoundAccessToken = client.TlsClientCertificateBoundAccessToken,
                TokenEncryptedResponseAlg = client.TokenEncryptedResponseAlg,
                TokenEncryptedResponseEnc = client.TokenEncryptedResponseEnc,
                TokenEndPointAuthMethod = client.TokenEndPointAuthMethod,
                UpdateDateTime = client.UpdateDateTime,
                UserInfoEncryptedResponseAlg = client.UserInfoEncryptedResponseAlg,
                UserInfoSignedResponseAlg = client.UserInfoSignedResponseAlg,
                UserInfoEncryptedResponseEnc = client.UserInfoEncryptedResponseEnc,
                SerializedParameters = client.SerializedParameters,
                TokenExchangeType = client.TokenExchangeType,
                TokenExpirationTimeInSeconds = client.TokenExpirationTimeInSeconds,
                TokenSignedResponseAlg = client.TokenSignedResponseAlg,
                ResponseTypes = client.ResponseTypes == null ? string.Empty : string.Join(",", client.ResponseTypes),
                RedirectionUrls = client.RedirectionUrls == null ? string.Empty : string.Join(",", client.RedirectionUrls),
                PostLogoutRedirectUris = client.PostLogoutRedirectUris == null ? string.Empty : string.Join(",", client.PostLogoutRedirectUris),
                GrantTypes = client.GrantTypes == null ? string.Empty : string.Join(",", client.GrantTypes),
                DefaultAcrValues = client.DefaultAcrValues == null ? string.Empty : string.Join(",", client.DefaultAcrValues),
                Contacts = client.Contacts == null ? string.Empty : string.Join(",", client.Contacts),
                AuthorizationDataTypes = client.AuthorizationDataTypes == null ? string.Empty : string.Join(",", client.AuthorizationDataTypes),
                DeviceAuthCodes = client.DeviceAuthCodes == null ? new List<SugarDeviceAuthCode>() : client.DeviceAuthCodes.Select(a => new SugarDeviceAuthCode
                {
                    CreateDateTime = a.CreateDateTime,
                    DeviceCode = a.DeviceCode,
                    ExpirationDateTime = a.ExpirationDateTime,
                    LastAccessTime = a.LastAccessTime,
                    NextAccessDateTime = a.NextAccessDateTime,
                    Scopes = a.Scopes == null ? string.Empty : string.Join(",", a.Scopes),
                    Status = a.Status,
                    UpdateDateTime = a.UpdateDateTime,
                    UserLogin = a.UserLogin,
                    UserCode = a.UserCode
                }).ToList(),
                SerializedJsonWebKeys = client.SerializedJsonWebKeys == null ? new List<SugarClientJsonWebKey>() : client.SerializedJsonWebKeys.Select(s => new SugarClientJsonWebKey
                {
                    Alg = s.Alg,
                    KeyType = s.KeyType,
                    Kid = s.Kid,
                    SerializedJsonWebKey = s.SerializedJsonWebKey,
                    Usage = s.Usage,
                }).ToList(),
                Translations = client.Translations == null ? new List<SugarTranslation>() : client.Translations.Select(t => new SugarTranslation
                {
                    Key = t.Key,
                    Language = t.Language,
                    Value = t.Value
                }).ToList(),
                Realms = client.Realms == null ? new List<SugarRealm>() : client.Realms.Select(r => new SugarRealm
                {
                    RealmsName = r.Name
                }).ToList(),
                ClientScopes = client.Scopes == null ? new List<SugarClientScope>() : client.Scopes.Select(r => new SugarClientScope
                {
                    ScopesId = r.Id
                }).ToList()
            };
        }

        public Client ToDomain()
        {
            return new Client
            {
                AccessTokenType = AccessTokenType,
                ApplicationType = ApplicationType,
                AuthorizationEncryptedResponseAlg = AuthorizationEncryptedResponseAlg,
                AuthorizationEncryptedResponseEnc = AuthorizationEncryptedResponseEnc,
                AuthorizationSignedResponseAlg = AuthorizationSignedResponseAlg,
                AuthReqIdExpirationTimeInSeconds = AuthReqIdExpirationTimeInSeconds,
                BackChannelLogoutSessionRequired = BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = BackChannelLogoutUri,
                BCAuthenticationRequestSigningAlg = BCAuthenticationRequestSigningAlg,
                BCClientNotificationEndpoint = BCClientNotificationEndpoint,
                BCIntervalSeconds = BCIntervalSeconds,
                BCTokenDeliveryMode = BCTokenDeliveryMode,
                BCUserCodeParameter = BCUserCodeParameter,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                ClientSecretExpirationTime = ClientSecretExpirationTime,
                ClientType = ClientType,
                CNonceExpirationTimeInSeconds = CNonceExpirationTimeInSeconds,
                CredentialOfferEndpoint = CredentialOfferEndpoint,
                DefaultMaxAge = DefaultMaxAge,
                DPOPBoundAccessTokens = DPOPBoundAccessTokens,
                DPOPNonceLifetimeInSeconds = DPOPNonceLifetimeInSeconds,
                FrontChannelLogoutUri = FrontChannelLogoutUri,
                IdTokenEncryptedResponseAlg = IdTokenEncryptedResponseAlg,
                IdTokenEncryptedResponseEnc = IdTokenEncryptedResponseEnc,
                IdTokenSignedResponseAlg = IdTokenSignedResponseAlg,
                UserInfoSignedResponseAlg = UserInfoSignedResponseAlg,
                FrontChannelLogoutSessionRequired = FrontChannelLogoutSessionRequired,
                InitiateLoginUri = InitiateLoginUri,
                IsConsentDisabled = IsConsentDisabled,
                IsDPOPNonceRequired = IsDPOPNonceRequired,
                IsRedirectUrlCaseSensitive = IsRedirectUrlCaseSensitive,
                IsResourceParameterRequired = IsResourceParameterRequired,
                IsTokenExchangeEnabled = IsTokenExchangeEnabled,
                IsTransactionCodeRequired = IsTransactionCodeRequired,
                JwksUri = JwksUri,
                PairWiseIdentifierSalt = PairWiseIdentifierSalt,
                PreferredTokenProfile = PreferredTokenProfile,
                PreAuthCodeExpirationTimeInSeconds = PreAuthCodeExpirationTimeInSeconds,
                RedirectToRevokeSessionUI = RedirectToRevokeSessionUI,
                RegistrationAccessToken = RegistrationAccessToken,
                RefreshTokenExpirationTimeInSeconds = RefreshTokenExpirationTimeInSeconds,
                RequestObjectEncryptionAlg = RequestObjectEncryptionAlg,
                RequestObjectEncryptionEnc = RequestObjectEncryptionEnc,
                RequestObjectSigningAlg = RequestObjectSigningAlg,
                RequireAuthTime = RequireAuthTime,
                SectorIdentifierUri = SectorIdentifierUri,
                SerializedParameters = SerializedParameters,
                SoftwareId = SoftwareId,
                SoftwareVersion = SoftwareVersion,
                SubjectType = SubjectType,
                TlsClientAuthSanDNS = TlsClientAuthSanDNS,
                TlsClientAuthSanEmail = TlsClientAuthSanEmail,
                TlsClientAuthSanIP = TlsClientAuthSanIP,
                TlsClientAuthSanURI = TlsClientAuthSanURI,
                TlsClientAuthSubjectDN = TlsClientAuthSubjectDN,
                TlsClientCertificateBoundAccessToken = TlsClientCertificateBoundAccessToken,
                TokenEncryptedResponseAlg = TokenEncryptedResponseAlg,
                TokenEncryptedResponseEnc = TokenEncryptedResponseEnc,
                TokenEndPointAuthMethod = TokenEndPointAuthMethod,
                TokenExchangeType = TokenExchangeType,
                TokenExpirationTimeInSeconds = TokenExpirationTimeInSeconds,
                TokenSignedResponseAlg = TokenSignedResponseAlg,
                UserInfoEncryptedResponseEnc = UserInfoEncryptedResponseEnc,
                UserInfoEncryptedResponseAlg = UserInfoEncryptedResponseAlg,
                UpdateDateTime = UpdateDateTime,
                CreateDateTime = CreateDateTime,
                ResponseTypes = ResponseTypes == null ? new List<string>() : ResponseTypes.Split(','),
                RedirectionUrls = RedirectionUrls == null ? new List<string>() : RedirectionUrls.Split(','),
                PostLogoutRedirectUris = PostLogoutRedirectUris == null ? new List<string>() : PostLogoutRedirectUris.Split(','),
                GrantTypes = GrantTypes == null ? new List<string>() : GrantTypes.Split(','),
                DefaultAcrValues = DefaultAcrValues == null ? new List<string>() : DefaultAcrValues.Split(','),
                Contacts = Contacts == null ? new List<string>() : Contacts.Split(','),
                AuthorizationDataTypes = AuthorizationDataTypes == null ? new List<string>() : AuthorizationDataTypes.Split(','),
                Realms = Realms == null ? new List<Realm>() : Realms.Select(r => r.ToDomain()).ToList(),
                Translations = Translations == null ? new List<Translation>() : Translations.Select(r => r.ToDomain()).ToList(),
                DeviceAuthCodes = DeviceAuthCodes == null ? new List<DeviceAuthCode>() : DeviceAuthCodes.Select(r => r.ToDomain()).ToList(),
                SerializedJsonWebKeys = SerializedJsonWebKeys == null ? new List<ClientJsonWebKey>() : SerializedJsonWebKeys.Select(j => j.ToDomain()).ToList(),
                Scopes = ClientScopes == null ? new List<Scope>() : ClientScopes.Select(s => s.Scope.ToDomain()).ToList(),
                Id = Id
            };
        }
    }
}
