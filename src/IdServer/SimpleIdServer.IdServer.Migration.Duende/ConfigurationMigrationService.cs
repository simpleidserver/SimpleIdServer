// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrate;
using System.Text.Json;
using DPoPTokenExpirationValidationMode = Duende.IdentityServer.Models.DPoPTokenExpirationValidationMode;
using DuendeAccessTokenType = Duende.IdentityServer.Models.AccessTokenType;
using DuendeApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;
using DuendeApiScope = Duende.IdentityServer.EntityFramework.Entities.ApiScope;
using DuendeClient = Duende.IdentityServer.EntityFramework.Entities.Client;
using DuendeIdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;

namespace SimpleIdServer.IdServer.Migration.Duende;

public class ConfigurationMigrationService
{
    private const string _jwk = "JWK";
    private readonly ConfigurationDbContext _dbContext;

    public ConfigurationMigrationService(ConfigurationDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }

    public async Task<MigrationResult> Extract(CancellationToken cancellationToken)
    {
        var scopes = await _dbContext.ApiScopes
            .Include(c => c.UserClaims)
            .ToListAsync();
        var apiResources = await _dbContext.ApiResources
            .Include(c => c.UserClaims)
            .Include(c => c.Scopes)
            .ToListAsync();
        var clients = await _dbContext.Clients
            .Include(c => c.RedirectUris)
            .Include(c => c.AllowedGrantTypes)
            .Include(c => c.ClientSecrets)
            .Include(c => c.PostLogoutRedirectUris)
            .Include(c => c.AllowedScopes).ToListAsync();
        var identityResources = await _dbContext.IdentityResources
            .Include(c => c.UserClaims)
            .ToListAsync();
        var extractedApiScopes = scopes.Select(s => Map(s)).ToList();
        var extractedApiResources = apiResources.Select(r => Map(r, extractedApiScopes)).ToList();
        var extractedIdentityScopes = identityResources.Select(r => Map(r)).ToList();
        var allScopes = new List<Scope>();
        allScopes.AddRange(extractedApiScopes);
        allScopes.AddRange(extractedIdentityScopes);
        var extractedClients = clients.Select(c => Map(c, allScopes)).ToList();
        var result = new MigrationResult(extractedClients, allScopes, extractedApiResources);
        var json = JsonSerializer.Serialize(result);
        return result;
    }

    private static Scope Map(DuendeApiScope scope)
    {
        var lst = DefaultClaimMappers.All;
        var result = new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = scope.Name,
            Type = scope.UserClaims.Any() ? ScopeTypes.IDENTITY : ScopeTypes.APIRESOURCE,
            Protocol = ScopeProtocols.OPENID,
            Description = scope.Description,
            IsExposedInConfigurationEdp = scope.ShowInDiscoveryDocument,
            ClaimMappers = scope.UserClaims.Select(c =>
            {
                var existingMapper = lst.SingleOrDefault(m => m.Name == c.Type);
                if (existingMapper != null)
                {
                    return existingMapper;
                }

                return new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = c.Type,
                    TargetClaimPath = c.Type,
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                };
            }).ToList(),
            CreateDateTime = scope.Created,
            UpdateDateTime = scope.Updated ?? scope.Created,
        };
        return result;
    }

    private static ApiResource Map(DuendeApiResource resource, List<Scope> scopes)
    {
        var result = new ApiResource
        {
            Id = Guid.NewGuid().ToString(),
            Name = resource.Name,
            Audience = resource.Name,
            Scopes = scopes.Where(s => resource.Scopes.Any(rs => rs.Scope == s.Name)).ToList(),
            Description = resource.Description,
        };
        return result;
    }

    private static Scope Map(DuendeIdentityResource identityResource)
    {
        var lst = DefaultClaimMappers.All;
        return new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = identityResource.Name,
            Description = identityResource.Description,
            IsExposedInConfigurationEdp = identityResource.ShowInDiscoveryDocument,
            ClaimMappers = identityResource.UserClaims.Select(c =>
            {
                var ss = lst.Where(m => m.Name == c.Type);
                var existingMapper = lst.SingleOrDefault(m => m.Name == c.Type);
                if(existingMapper != null)
                {
                    return existingMapper;
                }

                return new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = c.Type,
                    TargetClaimPath = c.Type,
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                };
            }).ToList(),
            CreateDateTime = identityResource.Created,
            UpdateDateTime = identityResource.Updated ?? identityResource.Created,
        };
    }

    private static Client Map(DuendeClient client, List<Scope> scopes)
    {
        var accessTokenType = (DuendeAccessTokenType)client.AccessTokenType;
        var result = new Client
        {
            Id = Guid.NewGuid().ToString(),
            ClientId = client.ClientId,
            GrantTypes = client.AllowedGrantTypes.Select(g => g.GrantType).ToList(),
            FrontChannelLogoutUri = client.FrontChannelLogoutUri,
            FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
            IsPublic = client.RequirePkce,
            AccessTokenType = accessTokenType == DuendeAccessTokenType.Jwt ? AccessTokenTypes.Jwt : AccessTokenTypes.Reference,
            RequestObjectSigningAlg = ResolveRequestObjectSigningAlg(client),
            IsDPOPNonceRequired = ResolveIsDPOPNonceRequired(client),
            DPOPNonceLifetimeInSeconds = ResolveDPOPNonceLifetimeInSeconds(client),
            RedirectionUrls = client.RedirectUris.Select(r => r.RedirectUri).ToList(),
            PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(r => r.PostLogoutRedirectUri).ToList(),
            BackChannelLogoutUri = client.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
            TokenExpirationTimeInSeconds = client.AccessTokenLifetime,
            RefreshTokenExpirationTimeInSeconds = client.AbsoluteRefreshTokenLifetime,
            InitiateLoginUri = client.InitiateLoginUri,
            UserCookieExpirationTimeInSeconds = (double?)client.UserSsoLifetime,
            PairWiseIdentifierSalt = client.PairWiseSubjectSalt,
            IsConsentDisabled = !client.RequireConsent,
            Scopes = scopes.Where(s => client.AllowedScopes.Any(rs => rs.Scope == s.Name)).ToList(),
            SerializedJsonWebKeys = ResolveSerializedJsonWebKeys(client),
            ClientSecret = Guid.NewGuid().ToString(),
            CreateDateTime = client.Created,
            UpdateDateTime = client.Updated ?? client.Created,
        };
        result.UpdateClientName(client.ClientName, Constants.DefaultLanguage);
        result.UpdateClientUri(client.ClientUri, Constants.DefaultLanguage);
        result.UpdateLogoUri(client.LogoUri, Constants.DefaultLanguage);
        /*
        AuthorizationCodeLifetime
        AllowPlainTextPkce
        EnableLocalLogin
        RequireRequestObject
        AllowedIdentityTokenSigningAlgorithms
        AccessTokenLifetime
        AllowOfflineAccess
        AllowAccessTokensViaBrowser
         public bool Enabled { get; set; } = true;
        public string ProtocolType { get; set; } = "oidc";
        public List<ClientSecret> ClientSecrets { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public string Description { get; set; }
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; } public List<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        public bool IncludeJwtId { get; set; }
        public List<ClientClaim> Claims { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public string ClientClaimsPrefix { get; set; } = "client_";
        public List<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
        public List<ClientProperty> Properties { get; set; }
        public string UserCodeType { get; set; }
        public int DeviceCodeLifetime { get; set; } = 300;
        */

        return result;
    }

    private static string ResolveRequestObjectSigningAlg(DuendeClient client)
    {
        var clientSecret = client.ClientSecrets.FirstOrDefault(s => s.Type == "JWK");
        if (clientSecret == null)
        {
            return null;
        }

        var jwk = new JsonWebKey(clientSecret.Value);
        return jwk.Alg;
    }

    private static List<ClientJsonWebKey> ResolveSerializedJsonWebKeys(DuendeClient client)
    {
        var filteredClientSecrets = client.ClientSecrets.Where(s => s.Type == _jwk);
        if (!filteredClientSecrets.Any())
        {
            return new List<ClientJsonWebKey>();
        }

        return filteredClientSecrets.Select(s =>
        {
            var jwk = new JsonWebKey(s.Value);
            var keyType = SecurityKeyTypes.CERTIFICATE;
            if(jwk.Kty == "EC")
            {
                keyType = SecurityKeyTypes.ECDSA;
            }
            else if (jwk.Kty == "RSA")
            {
                keyType = SecurityKeyTypes.RSA;
            }

            return new ClientJsonWebKey
            {
                SerializedJsonWebKey = s.Value,
                Alg = jwk.Alg,
                Kid = jwk.Kid,
                KeyType = keyType,
                Usage = jwk.Use
            };
        }).ToList();
    }

    private static bool ResolveIsDPOPNonceRequired(DuendeClient client)
    {
        return client.DPoPValidationMode == DPoPTokenExpirationValidationMode.Nonce || client.DPoPValidationMode == DPoPTokenExpirationValidationMode.IatAndNonce;
    }

    private static int ResolveDPOPNonceLifetimeInSeconds(DuendeClient client)
    {
        return (int)client.DPoPClockSkew.TotalSeconds;
    }
}
