// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation;

namespace SimpleIdServer.IdServer.Federation.Helpers;

public class FederationClientHelper : StandardClientHelper
{
    private readonly IClientRegistrationService _clientRegistrationService;

    public FederationClientHelper(
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IClientRepository clientRepository,
        IClientRegistrationService clientRegistrationService) : base(httpClientFactory, clientRepository)
    {
        _clientRegistrationService = clientRegistrationService;
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
            result = await _clientRegistrationService.AutomaticRegisterClient(realm, clientId, cancellationToken);
        else
            await RenewClientTrustChain(realm, result, cancellationToken);

        return result;
    }

    private async Task RenewClientTrustChain(string realm, Client client, CancellationToken cancellationToken)
    {
        if (client.ClientType != ClientTypes.FEDERATION ||
            client.ExpirationDateTime > DateTime.UtcNow ||
            !client.ClientRegistrationTypesSupported.Contains(ClientRegistrationMethods.Automatic)) return;
        var record = await _clientRegistrationService.AutomaticResolveClientTrustChain(realm, client.ClientId, cancellationToken);
        client.UpdateDateTime = DateTime.UtcNow;
        client.ExpirationDateTime = record.Item2.ExpirationDateTime;
        ClientRepository.Update(client);
    }
}