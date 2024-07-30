// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Website.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore;

public class RealmEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;

    public RealmEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
    }


    [EffectMethod]
    public async Task Handle(GetAllRealmAction action, IDispatcher dispatcher)
    {
        var url = GetRealmsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var realms = SidJsonSerializer.Deserialize<IEnumerable<Realm>>(json);
        dispatcher.Dispatch(new GetAllRealmSuccessAction { Realms = realms });
    }

    [EffectMethod]
    public async Task Handle(AddRealmAction action, IDispatcher dispatcher)
    {
        if(!_options.IsReamEnabled)
        {
            dispatcher.Dispatch(new AddRealmFailureAction { ErrorMessage = Global.CannotAddRealmBecauseOptionIsDisabled });
            return;
        }

        var url = GetRealmsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var req = new AddRealmRequest
        {
            Name = action.Name,
            Description = action.Description
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new AddRealmSuccessAction
            {
                Description = action.Description,
                Name = action.Name
            });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddRealmFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(SearchRealmRolesAction action, IDispatcher dispatcher)
    {
        var baseUrl = GetRealmsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{action.Realm}/roles/.search"),
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
        var searchResult = SidJsonSerializer.DeserializeSearchRealmRoles(json);
        dispatcher.Dispatch(new SearchRealmRolesSuccessAction { RealmRoles = searchResult.Content, Count = searchResult.Count });
        string SanitizeExpression(string expression) => expression.Replace("Value.", "");
    }

    [EffectMethod]
    public async Task Handle(RemoveSelectedRealmRolesAction action, IDispatcher dispatcher)
    {
        var baseUrl = GetRealmsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach(var id in action.RealmRoleIds)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.RealmName}/roles/{id}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveSelectedRealmRolesSuccessAction { RealmRoleIds = action.RealmRoleIds });
    }

    [EffectMethod]
    public async Task Handle(GetRealmRoleAction action, IDispatcher dispatcher)
    {
        var baseUrl = GetRealmsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{action.Realm}/roles/{action.RoleId}"),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var content = await httpResult.Content.ReadAsStringAsync();
        var realmRole = JsonSerializer.Deserialize<RealmRole>(content);
        var scopes = new List<RealmRoleScope>();
        var scopesArray = JsonObject.Parse(content)["scopes"].AsArray();
        foreach(var scopeObj in scopesArray)
        {
            scopes.Add(new RealmRoleScope
            {
                Scope = JsonSerializer.Deserialize<Scope>(scopeObj["scope"].ToJsonString())
            });
        }

        realmRole.Scopes = scopes;
        dispatcher.Dispatch(new GetRealmRoleSuccessAction { RealmRole = realmRole });
    }

    private string GetRealmsUrl() => $"{_options.IdServerBaseUrl}/realms";
}

public class GetAllRealmAction
{
    public IEnumerable<Domains.Realm> Realms { get; set; }
}

public class GetAllRealmSuccessAction
{
    public IEnumerable<Domains.Realm> Realms { get; set; }
}

public class AddRealmAction
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class AddRealmFailureAction
{
    public string ErrorMessage { get; set; }
}

public class AddRealmSuccessAction
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class SearchRealmRolesAction
{
    public string Realm { get; set; }
    public string? Filter { get; set; } = null;
    public string? OrderBy { get; set; } = null;
    public int? Skip { get; set; } = null;
    public int? Take { get; set; } = null;
}

public class SearchRealmRolesSuccessAction
{
    public IEnumerable<Domains.RealmRole> RealmRoles { get; set; } = new List<Domains.RealmRole>();
    public int Count { get; set; }
}

public class RemoveSelectedRealmRolesAction
{
    public string RealmName { get; set; }
    public List<string> RealmRoleIds { get; set; }
}

public class RemoveSelectedRealmRolesSuccessAction
{
    public List<string> RealmRoleIds { get; set; }
}

public class ToggleRealmRoleAction
{
    public string RealmRoleId { get; set; }
    public bool IsSelected { get; set; }
}

public class ToggleAllRealmRolesAction
{
    public bool IsSelected { get; set; }
}

public class GetRealmRoleAction
{
    public string Realm { get; set; }
    public string RoleId { get; set; }
}

public class GetRealmRoleSuccessAction
{
    public RealmRole RealmRole { get; set; }
}