// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Clients;
using SimpleIdServer.OpenidFederation.Stores;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Federation;

public interface IClientRegistrationService
{
    Task<Client> AutomaticRegisterClient(string realm, string clientId, CancellationToken cancellationToken);
    Task<(Client, OpenidTrustChain)> ExplicitRegisterClient(string realm, string entityStatement, CancellationToken cancellationToken);
    Task<(Client, OpenidTrustChain)> ExplicitResolveClientTrustChain(string realm, string clientId, CancellationToken cancellationToken);
    Task<(Client, OpenidTrustChain)> AutomaticResolveClientTrustChain(string realm, string clientId, CancellationToken cancellationToken);
}

public class ClientRegistrationService : IClientRegistrationService
{
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;
    private readonly IFederationEntityStore _federationEntityStore;
    private readonly IClientRepository _clientRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly ILogger<ClientRegistrationService> _logger;

    public ClientRegistrationService(
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IFederationEntityStore federationEntityStore,
        IClientRepository clientRepository,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        ILogger<ClientRegistrationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _federationEntityStore = federationEntityStore;
        _clientRepository = clientRepository;
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _logger = logger;
    }

    public async Task<Client> AutomaticRegisterClient(
        string realm, 
        string clientId,
        CancellationToken cancellationToken)
    {
        var record = await AutomaticResolveClientTrustChain(realm, clientId, cancellationToken);
        return await AddClient(realm, record, cancellationToken);
    }

    public async Task<(Client, OpenidTrustChain)> ExplicitRegisterClient(
        string realm,
        string entityStatement,
        CancellationToken cancellationToken)
    {
        var record = await ExplicitResolveClientTrustChain(realm, entityStatement, cancellationToken);
        var newClient = record.Item1;
        var trustChain = record.Item2;
        var existingClient = await _clientRepository.GetByClientId(realm, newClient.ClientId, cancellationToken);
        if (existingClient != null)
        {
            existingClient.ExpirationDateTime = trustChain.ExpirationDateTime;
            existingClient.UpdateDateTime = DateTime.UtcNow;
            _clientRepository.Update(existingClient);
            return (existingClient, trustChain);
        }

        return (await AddClient(realm, record, cancellationToken), trustChain);
    }

    public Task<(Client, OpenidTrustChain)> ExplicitResolveClientTrustChain(
        string realm,
        string entityStatement,
        CancellationToken cancellationToken)
    {
        return ResolveClientTrustChain(realm,
            (r, c) => r.ResolveTrustChains(entityStatement, cancellationToken),
            ClientRegistrationMethods.Explicit,
            cancellationToken);
    }

    public Task<(Client, OpenidTrustChain)> AutomaticResolveClientTrustChain(
        string realm,
        string clientId,
        CancellationToken cancellationToken)
    {
        return ResolveClientTrustChain(realm,
            (r, c) => r.ResolveTrustChainsFromClientId(clientId, cancellationToken),
            ClientRegistrationMethods.Automatic,
            cancellationToken);
    }

    private async Task<(Client, OpenidTrustChain)> ResolveClientTrustChain(
        string realm, 
        Func<TrustChainResolver, CancellationToken, Task<List<OpenidTrustChain>>> fn,
        string type,
        CancellationToken cancellationToken)
    {
        var authorizedAuthMethods = new List<string>
        {
            "private_key_jwt",
            "tls_client_auth",
            "self_signed_tls_client_auth"
        };
        using (var resolver = TrustChainResolver.New(_httpClientFactory.GetHttpClient()))
        {
            List<OpenidTrustChain> trustChains = null;
            try
            {
                trustChains = await fn(resolver, cancellationToken);
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
                _logger.LogError(message);
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ex.Message);
            }

            var allAuthorities = await _federationEntityStore.GetAllAuthorities(realm, cancellationToken);
            var allTrustAnchors = trustChains.Select(c => c.TrustAnchor);
            var filteredTrustChain = trustChains.FirstOrDefault(tc => allAuthorities.Any(at => at.Sub == tc.TrustAnchor.FederationResult.Sub));
            if (filteredTrustChain == null)
                throw new OAuthException(ErrorCodes.MISSING_TRUST_ANCHOR, Resources.Global.NoTrustAnchorCanBeResolved);

            var federationResult = filteredTrustChain.EntityStatements.First().FederationResult;
            if (federationResult.Metadata == null ||
                federationResult.Metadata.OtherParameters == null ||
                !federationResult.Metadata.OtherParameters.ContainsKey(EntityStatementTypes.OpenidRelyingParty))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Resources.Global.MissingOpenidRpInEntityStatement);
            var client = JsonSerializer.Deserialize<Client>(federationResult.Metadata.OtherParameters[EntityStatementTypes.OpenidRelyingParty]);
            if (string.IsNullOrWhiteSpace(client.TokenEndPointAuthMethod))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, OAuthClientParameters.TokenEndpointAuthMethod));

            if (!authorizedAuthMethods.Contains(client.TokenEndPointAuthMethod))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Resources.Global.OnlyFollowingAuthMethodsAreSupported, string.Join(",", authorizedAuthMethods)));

            var metadata = federationResult.Metadata.OtherParameters[EntityStatementTypes.OpenidRelyingParty];
            if (!metadata.ContainsKey(OAuthClientParameters.ClientRegistrationTypesSupported))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Resources.Global.ClientRegistrationTypesMustBeSpecified);

            var clientRegistrationTypes = (metadata[OAuthClientParameters.ClientRegistrationTypesSupported] as JsonArray).Select(c => c.ToString());
            if(!clientRegistrationTypes.Contains(type))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Resources.Global.ClientRegistrationTypeNotSupported, type));

            return (client, filteredTrustChain);
        }
    }

    private async Task<Client> AddClient(string realm, (Client, OpenidTrustChain) record, CancellationToken cancellationToken)
    {
        var newClient = record.Item1;
        var trustChain = record.Item2;
        var existingRealm = await _realmRepository.Get(realm, cancellationToken);
        newClient.Id = Guid.NewGuid().ToString();
        newClient.Realms = new List<Realm>
        {
            existingRealm
        };
        newClient.Scopes = await _scopeRepository.GetByNames(realm, newClient.Scopes.Select(s => s.Name).ToList(), cancellationToken);
        newClient.ClientType = ClientTypes.FEDERATION;
        newClient.ClientSecret = Guid.NewGuid().ToString();
        newClient.ExpirationDateTime = trustChain.ExpirationDateTime;
        newClient.UpdateDateTime = DateTime.UtcNow;
        newClient.CreateDateTime = DateTime.UtcNow;
        _clientRepository.Add(newClient);
        return newClient;
    }
}