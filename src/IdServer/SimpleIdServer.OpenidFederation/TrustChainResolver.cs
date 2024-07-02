// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.OpenidFederation;

public class TrustChainResolver : IDisposable
{
    private readonly HttpClient _httpClient;

    private TrustChainResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public static TrustChainResolver New(HttpClient httpClient)
        => new TrustChainResolver(httpClient);

    public async Task<List<OpenidFederationResult>> ResolveTrustChains(string entityId, CancellationToken cancellationToken)
    {
        var result = new ConcurrentBag<OpenidFederationResult>();
        await Extract(result, entityId, cancellationToken);
        return result.ToList();
    }

    public Task<OpenidFederationResult> ResolveOpenidFederation(string entityId, CancellationToken cancellationToken)
        => InternalResolveOpenidFederation(entityId, cancellationToken);

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private async Task<bool> Extract(ConcurrentBag<OpenidFederationResult> federationLst, string entityId, CancellationToken cancellationToken)
    {
        var openidFederation = await InternalResolveOpenidFederation(entityId, cancellationToken);
        if (openidFederation == null) return false;
        federationLst.Add(openidFederation);
        if(openidFederation.AuthorityHints != null && openidFederation.AuthorityHints.Any())
        {
            var taskLst = openidFederation.AuthorityHints.Select(h => Extract(federationLst, h, cancellationToken)).ToList();
            var authorityHintsResult = await Task.WhenAll(taskLst);
            if (authorityHintsResult.Any(c => !c)) throw new InvalidOperationException("Impossible to resolve the trust chain");
        }

        return true;
    }

    private async Task<OpenidFederationResult> InternalResolveOpenidFederation(string entityId, CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{entityId}/{OpenidFederationConstants.EndPoints.OpenidFederation}")
        };
        var httpResult = await _httpClient.SendAsync(requestMessage, cancellationToken);
        if (!httpResult.IsSuccessStatusCode) return null;
        var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(content);
        var jwks = jwt.Claims.FirstOrDefault(c => c.Type == "jwks");
        if (jwks == null) return null;
        var keys = JsonSerializer.Deserialize<OpenidFederationJwksResult>(jwks.Value);
        if (keys == null) return null;
        var selectedKey = keys.JsonWebKeys?.FirstOrDefault(j => j["kid"].ToString() == jwt.Kid);
        if (selectedKey == null) return null;
        var jsonWebKey = new JsonWebKey(selectedKey.ToJsonString());
        var validationResult = handler.ValidateToken(content, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateLifetime = false,
            IssuerSigningKey = jsonWebKey
        });
        return validationResult.IsValid ? JsonSerializer.Deserialize<OpenidFederationResult>(Encoding.UTF8.GetString(Convert.FromBase64String(jwt.EncodedPayload))) : null;
    }
}