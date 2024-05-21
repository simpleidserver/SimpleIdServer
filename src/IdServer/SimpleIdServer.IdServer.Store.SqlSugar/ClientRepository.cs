// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class ClientRepository : IClientRepository
    {
        private readonly DbContext _dbContext;

        public ClientRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Client client)
        {
            _dbContext.Client.InsertNav(Transform(client))
                .Include(c => c.Scopes)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Realms)
                .Include(c => c.DeviceAuthCodes)
                .Include(c => c.Translations)
                .ExecuteCommand();
        }

        public void Delete(Client client)
        {
            _dbContext.Client.Deleteable(Transform(client)).ExecuteCommand();
        }

        public void Update(Client client)
        {
            _dbContext.Client.UpdateNav(Transform(client))
                .Include(c => c.Scopes)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Realms)
                .Include(c => c.DeviceAuthCodes)
                .Include(c => c.Translations)
                .ExecuteCommand();
        }

        public void DeleteRange(IEnumerable<Client> clients)
        {
            var cls = clients.Select(c => Transform(c)).ToList();
            _dbContext.Client.Deleteable(cls).ExecuteCommand();
        }

        public async Task<List<Client>> GetAll(string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Translations)
                .Includes(p => p.Realms)
                .Includes(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<Client>> GetAll(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                    .Includes(p => p.Realms)
                    .Where(p => clientIds.Contains(p.ClientId) && p.Realms.Any(r => r.RealmsName == realm))
                    .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<Client> GetByClientId(string realm, string clientId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Scopes, s => s.ClaimMappers)
                .Includes(c => c.SerializedJsonWebKeys)
                .Includes(c => c.Translations)
                .Includes(c => c.Realms)
                .FirstAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<Client>> GetByClientIds(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.SerializedJsonWebKeys)
                .Includes(c => c.Realms)
                .Includes(c => c.Scopes)
                .Includes(c => c.Translations)
                .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList(); 
        }

        public async Task<List<Client>> GetByClientIdsAndExistingBackchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.RealmsName == realm) && !string.IsNullOrWhiteSpace(c.BackChannelLogoutUri))
                .ToListAsync();
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<Client>> GetByClientIdsAndExistingFrontchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.RealmsName == realm) && !string.IsNullOrWhiteSpace(c.FrontChannelLogoutUri))
                .ToListAsync();
            return result.Select(r => r.ToDomain()).ToList();
        }

        public Task<int> NbClients(string realm, CancellationToken cancellationToken)
        {
            return _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Realms)
                .CountAsync(c => c.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        }

        public async Task<SearchResult<Client>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
        {
            var result = _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Translations)
                .Includes(p => p.Realms)
                .Includes(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm));
            if (!string.IsNullOrWhiteSpace(request.Filter))
                result = result.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                result = result.OrderBy(request.OrderBy);
            else
                result = result.OrderByDescending(r => r.UpdateDateTime);

            var nb = result.Count();
            var clients = await result.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new SearchResult<Client>
            {
                Count = nb,
                Content = clients.Select(c => c.ToDomain()).ToList()
            };
        }

        private static SugarClient Transform(Client client)
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
                Scopes = client.Scopes == null ? new List<SugarScope>() : client.Scopes.Select(r => new SugarScope
                {
                    ScopesId = r.Id,
                }).ToList()
            };
        }
    }
}
