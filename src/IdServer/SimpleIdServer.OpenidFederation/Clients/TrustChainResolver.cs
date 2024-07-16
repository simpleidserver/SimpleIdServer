// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Resources;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.OpenidFederation.Clients;

public class TrustChainResolver : IDisposable
{
    private const string separator = ";";
    private readonly HttpClient _httpClient;

    private TrustChainResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public static TrustChainResolver New()
        => new TrustChainResolver(new HttpClient());

    public static TrustChainResolver New(HttpClient httpClient)
        => new TrustChainResolver(httpClient);

    public async Task<List<OpenidTrustChain>> ResolveTrustChains(string entityId, CancellationToken cancellationToken)
    {
        var dic = new ConcurrentDictionary<string, EntityStatement>();
        await ExtractTrustChainFromRp(dic, entityId, cancellationToken);
        return Transform(dic);
    }

    public static List<OpenidTrustChain> Transform(ConcurrentDictionary<string, EntityStatement> dic)
    {
        var result = new List<OpenidTrustChain>();
        var levels = dic.ToDictionary(v => v.Key, v => v.Key.Split(separator).Count());
        var maxLevel = levels.Values.Max();
        for (var level = 1; level <= maxLevel; level++)
        {
            var filteredKeys = levels.Where(kvp => kvp.Value == level).Select(kvp => kvp.Key);
            foreach (var key in filteredKeys)
            {
                var entityStatement = dic[key];
                if (level == 1)
                    result.Add(new OpenidTrustChain(new List<EntityStatement> { entityStatement }, key));
                else
                {
                    var parentPath = string.Join(separator, key.Split(separator).Take(level - 1));
                    var possibleParents = result.Where(r => r.Path.StartsWith(parentPath));
                    var orphanParent = possibleParents.FirstOrDefault(p => p.Path == parentPath);
                    if (orphanParent != null)
                    {
                        orphanParent.EntityStatements.Add(entityStatement);
                        orphanParent.Path = key;
                    }
                    else
                    {
                        var entityStatements = possibleParents.First().EntityStatements.Take(level - 1).ToList();
                        entityStatements.Add(entityStatement);
                        result.Add(new OpenidTrustChain(entityStatements, key));
                    }
                }
            }
        }

        return result;
    }

    public Task<EntityStatement> ResolveOpenidFederation(string entityId, CancellationToken cancellationToken)
        => InternalResolveOpenidFederation(entityId, cancellationToken);

    public void Dispose() =>
        _httpClient.Dispose();

    private async Task<bool> ExtractTrustChainFromRp(ConcurrentDictionary<string, EntityStatement> federationLst, string entityId, CancellationToken cancellationToken)
    {
        var openidFederation = await InternalResolveOpenidFederation(entityId, cancellationToken);
        if (openidFederation == null) return false;
        federationLst.TryAdd(entityId, openidFederation);
        if (openidFederation.FederationResult.AuthorityHints != null && openidFederation.FederationResult.AuthorityHints.Any())
        {
            var taskLst = openidFederation.FederationResult.AuthorityHints.Select(h => ExtractTrustChainFromTaOrIntermediate(federationLst, entityId, h, entityId, cancellationToken)).ToList();
            var authorityHintsResult = await Task.WhenAll(taskLst);
            if (authorityHintsResult.Any(c => !c)) throw new InvalidOperationException(Global.ImpossibleToResolveTrustChain);
        }

        return true;
    }

    private async Task<bool> ExtractTrustChainFromTaOrIntermediate(
        ConcurrentDictionary<string, EntityStatement> federationLst,
        string entityParentFullPath,
        string authorityHint, 
        string entityId, 
        CancellationToken cancellationToken)
    {
        var openidFederation = await InternalResolveOpenidFederation(authorityHint, cancellationToken);
        if (openidFederation.FederationResult.Metadata == null || 
            openidFederation.FederationResult.Metadata.FederationEntity == null || 
            string.IsNullOrWhiteSpace(openidFederation.FederationResult.Metadata.FederationEntity.FederationFetchEndpoint)) return true;
        var fetchResult = await Fetch(openidFederation.FederationResult.Metadata.FederationEntity.FederationFetchEndpoint,
            authorityHint,
            entityId,
            cancellationToken);
        if (fetchResult == null) return false;
        var fullPath = $"{entityParentFullPath}{separator}{authorityHint}";
        federationLst.TryAdd(fullPath, fetchResult);
        if(openidFederation.FederationResult.AuthorityHints != null && openidFederation.FederationResult.AuthorityHints.Any())
        {
            var taskLst = openidFederation.FederationResult.AuthorityHints.Select(h => ExtractTrustChainFromTaOrIntermediate(federationLst, fullPath, h, entityId, cancellationToken)).ToList();
            var authorityHintsResult = await Task.WhenAll(taskLst);
            if (authorityHintsResult.Any()) return false;
        }
        else
            federationLst.TryAdd($"{fullPath}{separator}{authorityHint}", openidFederation);

        return true;
    }

    private async Task<EntityStatement?> InternalResolveOpenidFederation(string entityId, CancellationToken cancellationToken)
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
        var json = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(jwt.EncodedPayload));
        return new EntityStatement(content, JsonSerializer.Deserialize<OpenidFederationResult>(json));
    }

    private async Task<EntityStatement?> Fetch(string fetchUrl, string issuer, string sub, CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{fetchUrl}?iss={issuer}&sub={sub}")
        };
        var httpResult = await _httpClient.SendAsync(requestMessage, cancellationToken);
        if (!httpResult.IsSuccessStatusCode) return null;
        var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(content);
        var json = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(jwt.EncodedPayload));
        return new EntityStatement(content, JsonSerializer.Deserialize<OpenidFederationResult>(json));
    }
}