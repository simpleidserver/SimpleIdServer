// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Text.Json;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleIdServer.IdServer.Migrations.Openiddict;

public class OpeniddictMigrationService : BaseMicrosoftIdentityMigrationService
{
    private static Dictionary<string, string> _mappingPermissionToGrantTypes = new Dictionary<string, string>
    {
        {  Permissions.GrantTypes.AuthorizationCode, AuthorizationCodeHandler.GRANT_TYPE },
        {  Permissions.GrantTypes.Password, PasswordHandler.GRANT_TYPE },
        {  Permissions.GrantTypes.ClientCredentials, ClientCredentialsHandler.GRANT_TYPE },
        {  Permissions.GrantTypes.RefreshToken, RefreshTokenHandler.GRANT_TYPE },
        {  Permissions.GrantTypes.DeviceCode, DeviceCodeHandler.GRANT_TYPE }
    };
    private static Dictionary<string, List<string>> _mappingPermissionToResponseTypes = new Dictionary<string, List<string>>
    {
        {  Permissions.ResponseTypes.IdToken, new List<string> { IdTokenResponseTypeHandler.RESPONSE_TYPE } },
        {  Permissions.ResponseTypes.Token, new List<string> { TokenResponseTypeHandler.RESPONSE_TYPE } },
        {  Permissions.ResponseTypes.Code, new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE } },
        {  Permissions.ResponseTypes.IdTokenToken, new List<string> { IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE } },
        {  Permissions.ResponseTypes.CodeIdToken, new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, IdTokenResponseTypeHandler.RESPONSE_TYPE } },
        {  Permissions.ResponseTypes.CodeIdTokenToken, new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE } }
    };
    private readonly ConfigurationDbcontext _dbcontext;
    private readonly IScopeRepository _scopeRepository;
    public override string Name => Constants.Name;

    public OpeniddictMigrationService(
        ConfigurationDbcontext dbcontext,
        ApplicationDbContext applicationDbContext,
        IScopeRepository scopeRepository) : base(applicationDbContext)
    {
        _dbcontext = dbcontext;
        _scopeRepository = scopeRepository;
    }

    public override Task<int> NbApiScopes(CancellationToken cancellationToken)
    {
        return _dbcontext.Scopes.CountAsync(cancellationToken);
    }

    public override async Task<List<Scope>> ExtractApiScopes(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var scopes = await _dbcontext.Scopes
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        return scopes.Select(Map).ToList();
    }

    public override Task<int> NbIdentityScopes(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }

    public override Task<List<Scope>> ExtractIdentityScopes(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<Scope>());
    }

    public override async Task<int> NbApiResources(CancellationToken cancellationToken)
    {
        return await _dbcontext.Scopes.CountAsync(cancellationToken);
    }

    public override async Task<List<ApiResource>> ExtractApiResources(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var scopes = await _dbcontext.Scopes
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        var allScopeIds = scopes.Select(s => $"{Constants.ApiScope}_{s.Name}").Distinct().ToList();
        var allScopes = await _scopeRepository.GetByIds(allScopeIds, cancellationToken);
        var result = new List<ApiResource>();
        foreach (var scope in scopes.Where(s => s.Resources != null))
        {
            var resources = JsonSerializer.Deserialize<List<string>>(scope.Resources) ?? new List<string>();
            foreach(var resource in  resources)
            {
                var existingResource = result.SingleOrDefault(r => r.Name == resource);
                if (existingResource == null)
                {
                    existingResource = new ApiResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        Audience = resource,
                        Name = resource,
                        Description = resource,
                        Scopes = new List<Scope>(),
                        UpdateDateTime = DateTime.UtcNow,
                        CreateDateTime = DateTime.UtcNow,
                    };
                    result.Add(existingResource);
                }

                existingResource.Scopes.Add(allScopes.Single(s => s.Id == $"{Constants.ApiScope}_{scope.Name}"));
            }
        }

        return result;
    }

    public override Task<int> NbClients(CancellationToken cancellationToken)
    {
        return _dbcontext.Applications.CountAsync(cancellationToken);
    }

    public override async Task<List<Client>> ExtractClients(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var applications = await _dbcontext.Applications
            .Skip(parameter.StartIndex).Take(parameter.Count)
            .AsNoTracking()
            .ToListAsync();
        var clients = new List<Client>();
        var scopes = await _scopeRepository.GetByNames(
            applications.SelectMany(a => ResolveScopes(a)).Distinct().ToList(), 
            cancellationToken);
        foreach (var application in applications)
        {
            var record = Map(application, scopes);
            clients.Add(record);
        }

        return clients;
    }

    private static Client Map(OpenIddictEntityFrameworkCoreApplication client,  List<Scope> scopes)
    {
        var permissions = ParseArray(client.Permissions);
        var resolvedScopes = scopes.Where(s => ResolveScopes(client).Contains(s.Name)).ToList();
        var result = new Client
        {
            Id = Guid.NewGuid().ToString(),
            Source = Constants.Name,
            ClientId = client.ClientId,
            PostLogoutRedirectUris = ParseArray(client.PostLogoutRedirectUris),
            RedirectionUrls = ParseArray(client.RedirectUris),
            Secrets = new List<ClientSecret>(),
            GrantTypes = ResolveGrantTypes(permissions),
            ResponseTypes = ResolveResponseTypes(permissions),
            Scopes = resolvedScopes,
            ClientType = ResolveClientType(client),
            UpdateDateTime = DateTime.UtcNow,
            CreateDateTime = DateTime.UtcNow
        };
        if(!string.IsNullOrWhiteSpace(client.ClientSecret))
        {
            result.Add(ClientSecret.Resolve(client.ClientSecret));
        }

        if(result.ClientType == Domains.ClientTypes.WEBSITE || result.ClientType == Domains.ClientTypes.MACHINE)
        {
            result.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
        }

        if (client.ClientType == "public")
        {
            result.IsPublic = true;
        }

        result.UpdateClientName(client.DisplayName);
        return result;
    }

    private static Scope Map(OpenIddictEntityFrameworkCoreScope scope)
    {
        var result = new Scope
        {
            Id = $"{Constants.ApiScope}_{scope.Name}",
            Source = Constants.Name,
            Protocol = ScopeProtocols.OAUTH,
            Type = ScopeTypes.APIRESOURCE,
            Name = scope.Name,
            Description = scope.Description,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        return result;
    }

    private static List<string> ParseArray(string str)
    {
        if(string.IsNullOrWhiteSpace(str))
        {
            return new List<string>();
        }

        return JsonSerializer.Deserialize<List<string>>(str) ?? new List<string>();
    }

    private static List<string> ResolveGrantTypes(List<string> permissions)
    {
        return _mappingPermissionToGrantTypes.Where(kvp => permissions.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();
    }

    private static List<string> ResolveResponseTypes(List<string> permissions)
    {
        return _mappingPermissionToResponseTypes.Where(kvp => permissions.Contains(kvp.Key)).SelectMany(kvp => kvp.Value).Distinct().ToList();
    }

    private static List<string> ResolveScopes(OpenIddictEntityFrameworkCoreApplication client)
    {
        var permissions = ParseArray(client.Permissions).Where(s => s.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope)).Select(s => s.Replace(OpenIddictConstants.Permissions.Prefixes.Scope, "")).ToList();
        var clientType = ResolveClientType(client);
        if (clientType != Domains.ClientTypes.MACHINE)
        {
            permissions.Add(DefaultScopes.OpenIdScope.Name);
        }

        return permissions;
    }

    private static Domains.ClientTypes ResolveClientType(OpenIddictEntityFrameworkCoreApplication client)
    {
        if(client.ClientType == "public")
        {
            return Domains.ClientTypes.SPA;
        }

        var permissions = ParseArray(client.Permissions);
        var resolvedGrantTypes = ResolveGrantTypes(permissions);
        if (resolvedGrantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
        {
            return Domains.ClientTypes.WEBSITE;
        }

        return Domains.ClientTypes.MACHINE;
    }
}
