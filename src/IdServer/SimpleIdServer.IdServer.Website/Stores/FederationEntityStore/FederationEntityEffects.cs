// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Federation.Apis.FederationEntity;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Infrastructures;
using SimpleIdServer.OpenidFederation.Domains;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.FederationEntityStore;

public class FederationEntityEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public FederationEntityEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> options,
        IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
    }

    [EffectMethod]
    public async Task Handle(SearchFederationEntitiesAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetFederationEntitiesUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/.search"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new SearchRequest
            {
                Filter = SanitizeExpression(action.Filter),
                OrderBy = SanitizeExpression(action.OrderBy),
                Skip = action.Skip,
                Take = action.Take
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var searchResult = SidJsonSerializer.Deserialize<SearchResult<FederationEntity>>(json);
        dispatcher.Dispatch(new SearchFederationEntitiesSuccessAction { FederationEntities = searchResult.Content, Count = searchResult.Count });

        string SanitizeExpression(string expression) => expression.Replace("FederationEntity.", "").Replace("Value", "");
    }

    [EffectMethod]
    public async Task Handle(AddTrustedAnchorAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetFederationEntitiesUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/trustanchors"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new AddTrustedAnchorRequest
            {
                Url = action.Url
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var federationEntity = SidJsonSerializer.Deserialize<FederationEntity>(json);
            dispatcher.Dispatch(new AddTrustedAnchorSuccessAction { FederationEntity = federationEntity });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddTrustedAnchorFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(RemoveFederationEntitiesAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetFederationEntitiesUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach(var id in action.Ids)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{id}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveFederationEntitiesSuccessAction { Ids = action.Ids });
    }

    private async Task<string> GetFederationEntitiesUrl()
    {
        if (_options.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/federationentities";
        }

        return $"{_options.IdServerBaseUrl}/federationentities";
    }
}

public class SearchFederationEntitiesAction
{
    public string? Filter { get; set; } = null;
    public string? OrderBy { get; set; } = null;
    public int? Skip { get; set; } = null;
    public int? Take { get; set; } = null;
    public bool OnlyRoot { get; set; } = true;
}

public class SearchFederationEntitiesSuccessAction
{
    public IEnumerable<FederationEntity> FederationEntities { get; set; } = new List<FederationEntity>();
    public int Count { get; set; }
}

public class AddTrustedAnchorAction
{
    public string Url { get; set; }
}

public class AddTrustedAnchorSuccessAction
{
    public FederationEntity FederationEntity { get; set; }
}

public class AddTrustedAnchorFailureAction
{
    public string ErrorMessage { get; set; }
}

public class StartAddTrustAnchorAction
{

}

public class ToggleFederationEntityAction
{
    public bool IsSelected { get; set; } = false;
    public string Id { get; set; } = null!;
}

public class ToggleAllFederationEntitiesAction
{
    public bool IsSelected { get; set; } = false;
}

public class RemoveFederationEntitiesAction
{
    public List<string> Ids { get; set; }
}

public class RemoveFederationEntitiesSuccessAction
{
    public List<string> Ids { get; set; }
}