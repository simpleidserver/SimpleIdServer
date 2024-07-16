// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
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

    public FederationClientHelper(
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IClientRepository clientRepository,
        ILogger<FederationClientHelper> logger,
        IFederationEntityStore federationEntityStore) : base(httpClientFactory, clientRepository)
    {
        _logger = logger;
        _federationEntityStore = federationEntityStore;
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
        if (result == null)
            result = await ResolveClientByOpenidFederation(realm, clientId, cancellationToken);

        return result;
    }

    private async Task<Client> ResolveClientByOpenidFederation(string realm, string clientId, CancellationToken cancellationToken)
    {
        const string metadataName = "openid_relying_party";
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
            return client;
        }
    }
}