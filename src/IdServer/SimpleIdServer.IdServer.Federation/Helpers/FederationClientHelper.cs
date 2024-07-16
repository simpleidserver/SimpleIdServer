// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;



// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation.Clients;
using SimpleIdServer.OpenidFederation.Stores;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Federation.Helpers;

public class FederationClientHelper : StandardClientHelper
{
    private readonly ILogger<FederationClientHelper> _logger;
    private readonly IFederationEntityStore _federationEntityStore;
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;

    public FederationClientHelper(
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IClientRepository clientRepository,
        ILogger<FederationClientHelper> logger,
        IFederationEntityStore federationEntityStore,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository) : base(httpClientFactory, clientRepository)
    {
        _logger = logger;
        _federationEntityStore = federationEntityStore;
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
    }

    /// <summary>
    /// Resolve the client.
    /// Two scenarios are supported : 
    /// 1). Returns the client from the database.
    /// 2). Resolve the trust chain and returns the client.
    /// </summary>
    /// <param name="realm"></param>
    /// <param name="clientId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OAuthException"></exception>
    public override async Task<Client> ResolveClient(string realm, string clientId, CancellationToken cancellationToken)
    {
        var result = await base.ResolveClient(realm, clientId, cancellationToken);
        // 1. IF THE CLIENT IS EXPIRED THEN TRY TO RENEW THE FEDERATION.
        if (result == null)
            result = await ResolveClientByOpenidFederation(realm, clientId, cancellationToken);

        return result;
    }

    private async Task<Client> ResolveClientByOpenidFederation(string realm, string clientId, CancellationToken cancellationToken)
    {
        const string metadataName = "openid_relying_party";
        var authorizedAuthMethods = new List<string>
        {
            "private_key_jwt",
            "tls_client_auth",
            "self_signed_tls_client_auth"
        };
        using (var resolver = TrustChainResolver.New(HttpClientFactory.GetHttpClient()))
        {
            List<OpenidTrustChain> trustChains = null;
            try
            {
                trustChains = await resolver.ResolveTrustChains(clientId, cancellationToken);
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
                _logger.LogError(message);
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, message);
            }

            var allAuthorities = await _federationEntityStore.GetAllAuthorities(realm, cancellationToken);
            var allTrustAnchors = trustChains.Select(c => c.TrustAnchor);
            var filteredTrustChain = trustChains.FirstOrDefault(tc => allAuthorities.Any(at => at.Sub == tc.TrustAnchor.FederationResult.Sub));
            if (filteredTrustChain == null)
                throw new OAuthException(ErrorCodes.MISSING_TRUST_ANCHOR, Resources.Global.NoTrustAnchorCanBeResolved);


            var federationResult = filteredTrustChain.EntityStatements.First().FederationResult;
            if (federationResult.Metadata == null ||
                federationResult.Metadata.OtherParameters == null ||
                !federationResult.Metadata.OtherParameters.ContainsKey(metadataName))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Resources.Global.MissingOpenidRpInEntityStatement);
            var client = JsonSerializer.Deserialize<Client>(federationResult.Metadata.OtherParameters[metadataName]);
            if(string.IsNullOrWhiteSpace(client.TokenEndPointAuthMethod))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, OAuthClientParameters.TokenEndpointAuthMethod));

            if (!authorizedAuthMethods.Contains(client.TokenEndPointAuthMethod))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Resources.Global.OnlyFollowingAuthMethodsAreSupported, string.Join(",", authorizedAuthMethods)));

            var existingRealm = await _realmRepository.Get(realm, cancellationToken);
            client.Id = Guid.NewGuid().ToString();
            client.Realms = new List<Realm>
            {
                existingRealm
            };
            client.Scopes = await _scopeRepository.GetByNames(realm, client.Scopes.Select(s => s.Name).ToList(), cancellationToken);
            client.ClientType = ClientTypes.FEDERATION;
            client.ClientSecret = Guid.NewGuid().ToString();
            ClientRepository.Add(client);
            return client;
        }
    }
}