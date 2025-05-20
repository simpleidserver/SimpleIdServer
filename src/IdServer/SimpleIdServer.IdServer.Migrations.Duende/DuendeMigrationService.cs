// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using DPoPTokenExpirationValidationMode = Duende.IdentityServer.Models.DPoPTokenExpirationValidationMode;
using DuendeAccessTokenType = Duende.IdentityServer.Models.AccessTokenType;
using DuendeApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;
using DuendeApiScope = Duende.IdentityServer.EntityFramework.Entities.ApiScope;
using DuendeClient = Duende.IdentityServer.EntityFramework.Entities.Client;
using DuendeIdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;
namespace SimpleIdServer.IdServer.Migrations.Duende;

public class DuendeMigrationService : BaseMicrosoftIdentityMigrationService
{    
    private readonly ConfigurationDbContext _configurationDbcontext;
    private readonly IScopeRepository _scopeRepository;

    public DuendeMigrationService(
        ConfigurationDbContext configurationDbcontext,
        ApplicationDbContext applicationDbcontext,
        IScopeRepository scopeRepository) : base(applicationDbcontext)
    {
        _configurationDbcontext = configurationDbcontext;
        _scopeRepository = scopeRepository;
    }

    public override string Name => Constants.Name;

    public override Task<int> NbApiScopes(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.ApiScopes.CountAsync(cancellationToken);
    }

    public override async Task<List<Scope>> ExtractApiScopes(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var scopes = await _configurationDbcontext.ApiScopes
            .Include(c => c.UserClaims)
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        return scopes.Select(Map).ToList();
    }

    public override Task<int> NbIdentityScopes(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.IdentityResources.CountAsync(cancellationToken);
    }

    public override async Task<List<Scope>> ExtractIdentityScopes(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var scopes = await _configurationDbcontext.IdentityResources
            .Include(c => c.UserClaims)
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        return scopes.Select(Map).ToList();
    }

    public override Task<int> NbApiResources(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.ApiResources.CountAsync(cancellationToken);
    }

    public override async Task<List<ApiResource>> ExtractApiResources(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var apiResources = await _configurationDbcontext.ApiResources
            .Include(c => c.UserClaims)
            .Include(c => c.Scopes)
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        var allScopeIds = apiResources.SelectMany(r => r.Scopes.Select(s => $"{Constants.ApiScope}_{s.Scope}")).Distinct().ToList();
        var allScopes = await _scopeRepository.GetByIds(allScopeIds, cancellationToken);
        return apiResources.Select(r =>
        {
            var filteredScopes = allScopes.Where(s => r.Scopes.All(rs => rs.Scope == s.Name)).ToList();
            return Map(r, filteredScopes);
        }).ToList();
    }

    public override Task<int> NbClients(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.Clients.CountAsync(cancellationToken);
    }

    public override async Task<List<Client>> ExtractClients(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var clients = await _configurationDbcontext.Clients
            .Include(c => c.RedirectUris)
            .Include(c => c.AllowedGrantTypes)
            .Include(c => c.ClientSecrets)
            .Include(c => c.PostLogoutRedirectUris)
            .Include(c => c.AllowedScopes)
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        var allScopeNames = clients.SelectMany(c => c.AllowedScopes.Select(s => s.Scope)).Distinct().ToList();
        allScopeNames.Add(DefaultScopes.OpenIdScope.Name);
        var allScopes = await _scopeRepository.GetByNames(allScopeNames, cancellationToken);
        return clients.Select(c =>
        {
            var clientType = ResolveClientType(c);
            var filteredScopes = allScopes.Where(s => c.AllowedScopes.All(cs => cs.Scope == s.Name) || (s.Name == DefaultScopes.OpenIdScope.Name && clientType != ClientTypes.MACHINE)).ToList();
            return Map(c, filteredScopes);
        }).ToList();
    }

    private static Scope Map(DuendeApiScope scope)
    {
        var lst = DefaultClaimMappers.All;
        var result = new Scope
        {
            Id = $"{Constants.ApiScope}_{scope.Name}",
            Source = Constants.Name,
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

    private static Scope Map(DuendeIdentityResource identityResource)
    {
        var lst = DefaultClaimMappers.All;
        return new Scope
        {
            Id = $"{Constants.IdentityResource}_{identityResource.Name}",
            Source = Constants.Name,
            Name = identityResource.Name,
            Description = identityResource.Description,
            IsExposedInConfigurationEdp = identityResource.ShowInDiscoveryDocument,
            ClaimMappers = identityResource.UserClaims.Select(c =>
            {
                var ss = lst.Where(m => m.Name == c.Type);
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
            CreateDateTime = identityResource.Created,
            UpdateDateTime = identityResource.Updated ?? identityResource.Created,
        };
    }

    private static ApiResource Map(DuendeApiResource resource, List<Scope> scopes)
    {
        var result = new ApiResource
        {
            Id = Guid.NewGuid().ToString(),
            Source = Constants.Name,
            Name = resource.Name,
            Audience = resource.Name,
            Scopes = scopes,
            Description = resource.Description,
            CreateDateTime = resource.Created,
            UpdateDateTime = resource.Updated ?? resource.Created
        };
        return result;
    }

    private static Client Map(DuendeClient client, List<Scope> scopes)
    {
        var accessTokenType = (DuendeAccessTokenType)client.AccessTokenType;
        var result = new Client
        {
            Id = Guid.NewGuid().ToString(),
            ClientId = client.ClientId,
            Source = Constants.Name,
            ClientType = ResolveClientType(client),
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
            Scopes = scopes,
            SerializedJsonWebKeys = ResolveSerializedJsonWebKeys(client),
            Secrets = ResolveClientSecrets(client),
            CreateDateTime = client.Created,
            UpdateDateTime = client.Updated ?? client.Created,
            AuthorizationCodeExpirationInSeconds = client.AuthorizationCodeLifetime,
            DeviceCodeExpirationInSeconds = client.DeviceCodeLifetime,
            DeviceCodePollingInterval = client.PollingInterval ?? 5,
            PARExpirationTimeInSeconds = client.PushedAuthorizationLifetime ?? 600
        };
        result.ResponseTypes = ResolveResponseTypes(result.ClientType);
        result.TokenEndPointAuthMethod = result.ClientType == ClientTypes.MACHINE || result.ClientType == ClientTypes.WEBSITE ? OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD : null;
        result.UpdateClientName(client.ClientName, IdServer.Constants.DefaultLanguage);
        result.UpdateClientUri(client.ClientUri, IdServer.Constants.DefaultLanguage);
        result.UpdateLogoUri(client.LogoUri, IdServer.Constants.DefaultLanguage);
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
        var filteredClientSecrets = client.ClientSecrets.Where(s => s.Type == Constants.Jwk);
        if (!filteredClientSecrets.Any())
        {
            return new List<ClientJsonWebKey>();
        }

        return filteredClientSecrets.Select(s =>
        {
            var jwk = new JsonWebKey(s.Value);
            var keyType = SecurityKeyTypes.CERTIFICATE;
            if (jwk.Kty == "EC")
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

    private static List<ClientSecret> ResolveClientSecrets(DuendeClient client)
    {
        const string clientSecretType = "SharedSecret";
        var clientSecrets = client.ClientSecrets.Where(s => s.Type == clientSecretType);
        return clientSecrets.Select(s =>
        {
            var result = ClientSecret.Resolve(s.Value);
            result.ExpirationDateTime = s.Expiration;
            result.CreateDateTime = s.Created;
            return result;
        }).ToList();
    }

    private static ClientTypes ResolveClientType(DuendeClient client)
    {
        var grantTypes = client.AllowedGrantTypes.Select(g => g.GrantType).ToList();
        if (grantTypes.Contains(ClientCredentialsHandler.GRANT_TYPE))
        {
            return ClientTypes.MACHINE;
        }

        if (client.RequirePkce && grantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
        {
            return ClientTypes.SPA;
        }

        if (!client.RequirePkce && grantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
        {
            return ClientTypes.WEBSITE;
        }

        return ClientTypes.MACHINE;
    }

    private static List<string> ResolveResponseTypes(ClientTypes? type)
    {
        if (type == null || type == ClientTypes.MACHINE)
        {
            return new List<string>();
        }

        return new List<string>
        {
            AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE,
        };
    }
}
