// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface IClientHelper
{
    Task<IEnumerable<string>> GetRedirectionUrls(Client client, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetSectorIdentifierUrls(Client client, CancellationToken cancellationToken);
    Task<JsonWebKey> ResolveJsonWebKey(Client client, string kid, CancellationToken cancellationToken);
    Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(Client client, CancellationToken cancellationToken);
    Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(string jwksUri, CancellationToken cancellationToken);
    bool IsNonPreRegisteredRelyingParty(string clientId);
    Task<Client> ResolveClient(string realm, string clientId, CancellationToken cancellationToken);
    Task<Client> ResolveSelfDeclaredClient(JsonObject request, CancellationToken cancellationToken);
}

public class StandardClientHelper : IClientHelper
{
    private readonly Helpers.IHttpClientFactory _httpClientFactory;
    private readonly IClientRepository _clientRepository;

    public StandardClientHelper(
        Helpers.IHttpClientFactory httpClientFactory,
        IClientRepository clientRepository)
    {
        _httpClientFactory = httpClientFactory;
        _clientRepository = clientRepository;
    }

    public virtual async Task<IEnumerable<string>> GetRedirectionUrls(Client client, CancellationToken cancellationToken)
    {
        List<string> result = client.RedirectionUrls == null ? new List<string>() : client.RedirectionUrls.ToList();
        result.AddRange(await GetSectorIdentifierUrls(client, cancellationToken));
        return result;
    }

    public async Task<IEnumerable<string>> GetSectorIdentifierUrls(Client client, CancellationToken cancellationToken)
    {
        var result = new List<string>();
        if (!string.IsNullOrWhiteSpace(client.SectorIdentifierUri))
        {
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var httpResult = await httpClient.GetAsync(client.SectorIdentifierUri, cancellationToken);
                if (httpResult.IsSuccessStatusCode)
                {
                    var json = await httpResult.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var jArr = JsonSerializer.Deserialize<IEnumerable<string>>(json);
                        if (jArr != null)
                        {
                            foreach (var record in jArr)
                                result.Add(record.ToString());
                        }
                    }
                }
            }
        }

        return result;
    }

    public async Task<JsonWebKey> ResolveJsonWebKey(Client client, string kid, CancellationToken cancellationToken)
    {
        var jwks = await ResolveJsonWebKeys(client, cancellationToken);
        return jwks.FirstOrDefault(j => j.KeyId == kid);
    }

    public async Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(Client client, CancellationToken cancellationToken)
    {
        if (client.JsonWebKeys != null && client.JsonWebKeys.Any()) return client.JsonWebKeys;
        return await ResolveJsonWebKeys(client.JwksUri, cancellationToken);
    }

    public async Task<IEnumerable<JsonWebKey>> ResolveJsonWebKeys(string jwksUri, CancellationToken cancellationToken)
    {
        Uri uri = null;
        if (string.IsNullOrWhiteSpace(jwksUri) || !Uri.TryCreate(jwksUri, UriKind.Absolute, out uri)) return new JsonWebKey[0];
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            httpClient.BaseAddress = uri;
            var request = await httpClient.GetAsync(uri.AbsoluteUri, cancellationToken).ConfigureAwait(false);
            request.EnsureSuccessStatusCode();
            var json = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            var keysJson = JsonObject.Parse(json)["keys"].AsArray();
            var jsonWebKeys = keysJson.Select(k => new JsonWebKey(k.ToString()));
            return jsonWebKeys;
        }
    }

    public bool IsNonPreRegisteredRelyingParty(string clientId)
    {
        if (clientId.StartsWith($"{SimpleIdServer.Did.Constants.Scheme}:")) return true;
        return false;
    }

    public virtual Task<Client> ResolveClient(string realm, string clientId, CancellationToken cancellationToken)
        => _clientRepository.GetByClientId(realm, clientId, cancellationToken);

    public async Task<Client> ResolveSelfDeclaredClient(JsonObject request, CancellationToken cancellationToken)
    {
        var clientMetadataUri = request.GetClientMetadataUri();
        var clientMetadata = request.GetClientMetadata();
        var clientId = request.GetClientId();
        if (clientMetadata == null)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var requestMessage = new System.Net.Http.HttpRequestMessage
                {
                    Method = System.Net.Http.HttpMethod.Get,
                    RequestUri = new Uri(clientMetadataUri)
                };
                var httpResponse = await httpClient.SendAsync(requestMessage, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA_URI, Global.InvalidClientMetadataUri);
                var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                clientMetadata = JsonSerializer.Deserialize<Client>(json);
            }
        }

        clientMetadata.ClientId = clientId;
        clientMetadata.IsSelfIssueEnabled = true;
        return clientMetadata;
    }
}
