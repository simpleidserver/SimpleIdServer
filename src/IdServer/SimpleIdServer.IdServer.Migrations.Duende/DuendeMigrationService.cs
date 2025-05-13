// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.IdentityModel.Tokens.Jwt;
using DPoPTokenExpirationValidationMode = Duende.IdentityServer.Models.DPoPTokenExpirationValidationMode;
using DuendeAccessTokenType = Duende.IdentityServer.Models.AccessTokenType;
using DuendeApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;
using DuendeApiScope = Duende.IdentityServer.EntityFramework.Entities.ApiScope;
using DuendeClient = Duende.IdentityServer.EntityFramework.Entities.Client;
using DuendeIdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;
namespace SimpleIdServer.IdServer.Migrations.Duende;

public class DuendeMigrationService : IMigrationService
{
    public string Name => Constants.Name;
    private readonly ConfigurationDbContext _configurationDbcontext;
    private readonly ApplicationDbContext _applicationDbcontext;
    private readonly IScopeRepository _scopeRepository;

    public DuendeMigrationService(
        ConfigurationDbContext configurationDbcontext,
        ApplicationDbContext applicationDbcontext,
        IScopeRepository scopeRepository)
    {
        _configurationDbcontext = configurationDbcontext;
        _applicationDbcontext = applicationDbcontext;
        _scopeRepository = scopeRepository;
    }

    public Task<int> NbApiScopes(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.ApiScopes.CountAsync(cancellationToken);
    }

    public async Task<List<Scope>> ExtractApiScopes(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var scopes = await _configurationDbcontext.ApiScopes
            .Include(c => c.UserClaims)
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        return scopes.Select(Map).ToList();
    }

    public Task<int> NbIdentityScopes(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.IdentityResources.CountAsync(cancellationToken);
    }

    public async Task<List<Scope>> ExtractIdentityScopes(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var scopes = await _configurationDbcontext.IdentityResources
            .Include(c => c.UserClaims)
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        return scopes.Select(Map).ToList();
    }

    public Task<int> NbApiResources(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.ApiResources.CountAsync(cancellationToken);
    }

    public async Task<List<ApiResource>> ExtractApiResources(ExtractParameter parameter, CancellationToken cancellationToken)
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

    public Task<int> NbClients(CancellationToken cancellationToken)
    {
        return _configurationDbcontext.Clients.CountAsync(cancellationToken);
    }

    public async Task<List<Client>> ExtractClients(ExtractParameter parameter, CancellationToken cancellationToken)
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
        var allScopes = await _scopeRepository.GetByNames(allScopeNames, cancellationToken);
        return clients.Select(c =>
        {
            var filteredScopes = allScopes.Where(s => c.AllowedScopes.All(cs => cs.Scope == s.Name)).ToList();
            return Map(c, filteredScopes);
        }).ToList();
    }

    public Task<int> NbGroups(CancellationToken cancellationToken)
    {
        return _applicationDbcontext.Roles.CountAsync(cancellationToken);
    }

    public async Task<List<Group>> ExtractGroups(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var allGroups = await _applicationDbcontext.Roles.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(cancellationToken);
        return allGroups.Select(Map).ToList();
    }

    public Task<int> NbUsers(CancellationToken cancellationToken)
    {
        return _applicationDbcontext.Users.CountAsync(cancellationToken);
    }

    public async Task<List<User>> ExtractUsers(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var users = await _applicationDbcontext.Users.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(cancellationToken);
        var allUserIds = users.Select(u => u.Id).ToList();
        var allUserClaims = await _applicationDbcontext.UserClaims.Where(c => allUserIds.Contains(c.UserId)).ToListAsync(cancellationToken);
        var allUserRoles = await _applicationDbcontext.UserRoles.Where(c => allUserIds.Contains(c.UserId)).ToListAsync(cancellationToken);
        var allUserRoleIds = allUserRoles.Select(ur => ur.RoleId).Distinct().ToList();
        var extractedUsers = new List<User>();
        foreach (var user in users)
        {
            var userClaims = allUserClaims.Where(c => c.UserId == user.Id).Select(Map).ToList();
            var userRoles = allUserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
            var extractedUser = Map(user, userClaims, userRoles);
            extractedUsers.Add(extractedUser);
        }

        return extractedUsers;
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
            ClientSecret = Guid.NewGuid().ToString(),
            CreateDateTime = client.Created,
            UpdateDateTime = client.Updated ?? client.Created,
        };
        result.UpdateClientName(client.ClientName, IdServer.Constants.DefaultLanguage);
        result.UpdateClientUri(client.ClientUri, IdServer.Constants.DefaultLanguage);
        result.UpdateLogoUri(client.LogoUri, IdServer.Constants.DefaultLanguage);
        /*
        AuthorizationCodeLifetime
        AllowPlainTextPkce
        EnableLocalLogin
        RequireRequestObject
        AllowedIdentityTokenSigningAlgorithms
        AccessTokenLifetime
        AllowOfflineAccess
        AllowAccessTokensViaBrowser
        Enabled
        ProtocolType
        ClientSecrets 
        RequireClientSecret
        Description
        AllowRememberConsent
        AlwaysIncludeUserClaimsInIdToken
        IdentityProviderRestrictions
        IncludeJwtId
        Claims
        AlwaysSendClientClaims
        ClientClaimsPrefix
        ClientCorsOrigin
        ClientProperty
        UserCodeType
        DeviceCodeLifetime
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

    private static UserClaim Map(IdentityUserClaim<string> userClaim)
    {
        return new UserClaim
        {
            Id = Guid.NewGuid().ToString(),
            Name = userClaim.ClaimType,
            Value = userClaim.ClaimValue,
            UserId = userClaim.UserId
        };
    }

    private static Group Map(IdentityRole identityRole)
    {
        return new Group
        {
            Id = identityRole.Id,
            Name = identityRole.Name,
            Source = Constants.Name,
            FullPath = identityRole.Name,
            Description = identityRole.NormalizedName,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
    }

    private static User Map(ApplicationUser applicationUser, List<UserClaim> userClaims, List<string> groupIds)
    {
        var result = new User
        {
            Id = applicationUser.Id,
            Source = Constants.Name,
            Name = applicationUser.UserName,
            Email = applicationUser.Email,
            UnblockDateTime = applicationUser.LockoutEnd == null ? null : applicationUser.LockoutEnd.Value.UtcDateTime,
            NbLoginAttempt = applicationUser.AccessFailedCount,
            EmailVerified = applicationUser.EmailConfirmed,
            Credentials = new List<UserCredential>
            {
                new UserCredential
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = applicationUser.PasswordHash,
                    CredentialType = UserCredential.PWD,
                    IsActive = true
                }
            },
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        result.Status = result.IsBlocked() ? UserStatus.BLOCKED : UserStatus.ACTIVATED;
        var claims = new List<UserClaim>();
        var filteredClaims = userClaims.Where(c => c.UserId == applicationUser.Id);
        claims.AddRange(filteredClaims);
        if (!string.IsNullOrWhiteSpace(applicationUser.PhoneNumber) && !claims.Any(c => c.Type == JwtRegisteredClaimNames.PhoneNumber))
        {
            claims.Add(new UserClaim
            {
                Id = Guid.NewGuid().ToString(),
                Name = JwtRegisteredClaimNames.PhoneNumber,
                Value = applicationUser.PhoneNumber
            });
        }

        if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.PhoneNumberVerified))
        {
            claims.Add(new UserClaim
            {
                Id = Guid.NewGuid().ToString(),
                Name = JwtRegisteredClaimNames.PhoneNumberVerified,
                Value = applicationUser.PhoneNumberConfirmed.ToString().ToLowerInvariant()
            });
        }

        result.OAuthUserClaims = claims;
        result.Groups = groupIds.Select(groupId => new GroupUser
        {
            GroupsId = groupId
        }).ToList();
        return result;
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

        return ClientTypes.MACHINE;
    }
}
