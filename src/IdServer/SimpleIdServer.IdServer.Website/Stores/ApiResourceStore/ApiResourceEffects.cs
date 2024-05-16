// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.ApiResources;
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.ApiResourceStore
{
    public class ApiResourceEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public ApiResourceEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchApiResourcesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetApiResourcesBaseUrl();
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
            var searchResult = SidJsonSerializer.Deserialize<SearchResult<ApiResource>>(json);
            var selectedResources = new List<string>();
            if(!string.IsNullOrWhiteSpace(action.ScopeName))
                selectedResources = searchResult.Content.Where(c => c.Scopes.Any(s => s.Name == action.ScopeName)).Select(r => r.Name).ToList();

            dispatcher.Dispatch(new SearchApiResourcesSuccessAction { ApiResources = searchResult.Content, SelectedApiResources = selectedResources, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(AddApiResourceAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetApiResourcesBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new AddApiResourceRequest
            {
                Audience = action.Audience,
                Description = action.Description,
                Name = action.Name
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newApiResource = SidJsonSerializer.Deserialize<Domains.ApiResource>(json);
                dispatcher.Dispatch(new AddApiResourceSuccessAction { Id = newApiResource.Id, Name = action.Name, Description = action.Description, Audience = action.Audience });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddApiResourceFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedApiResourcesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetApiResourcesBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var resourceId in action.ResourceIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{baseUrl}/{resourceId}"),
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedApiResourcesSuccessAction { ResourceIds = action.ResourceIds });
        }

        [EffectMethod]
        public async Task Handle(UpdateApiScopeResourcesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateScopeResourcesRequest
            {
                Resources = action.Resources
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/resources"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateApiScopeResourcesSuccessAction { Id = action.Id, Resources = action.Resources });
        }

        [EffectMethod]
        public async Task Handle(UnassignApiResourcesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateScopeResourcesRequest
            {
                Resources = action.Resources
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/resources"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UnassignApiResourcesSuccessAction { Id = action.Id, Resources = action.Resources });
        }

        private Task<string> GetApiResourcesBaseUrl() => GetBaseUrl("apiresources");

        private Task<string> GetScopesBaseUrl() => GetBaseUrl("scopes");

        private async Task<string> GetBaseUrl(string subUrl)
        {
            if (_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/{subUrl}";
            }

            return $"{_options.IdServerBaseUrl}/{subUrl}";
        }
    }

    public class SearchApiResourcesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchApiResourcesSuccessAction
    {
        public IEnumerable<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public IEnumerable<string> SelectedApiResources { get; set; } = new List<string>();
        public int Count { get; set; }
    }

    public class AddApiResourceAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public string? Audience { get; set; } = null;
    }

    public class AddApiResourceSuccessAction
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Audience { get; set; } = null;
        public string? Description { get; set; } = null;
    }

    public class AddApiResourceFailureAction
    {
        public string Name { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class ToggleAvailableApiResourceSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string ResourceName { get; set; } = null!;
    }

    public class ToggleActiveApiResourceSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string ResourceName { get; set; } = null!;
    }

    public class ToggleAllAvailableApiResourceSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class ToggleAllActiveApiResourceSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class UpdateApiScopeResourcesAction
    {
        public string Id { get; set; } = null!;
        public IEnumerable<string> Resources { get; set; } = new List<string>();
    }

    public class UpdateApiScopeResourcesSuccessAction
    {
        public string Id { get; set; } = null!;
        public IEnumerable<string> Resources { get; set; } = new List<string>();
    }

    public class UnassignApiResourcesAction
    {
        public string Id { get; set; } = null!;
        public IEnumerable<string> Resources { get; set; } = new List<string>();
    }

    public class UnassignApiResourcesSuccessAction
    {
        public string Id { get; set; } = null!;
        public IEnumerable<string> Resources { get; set; } = new List<string>();
    }

    public class RemoveSelectedApiResourcesAction
    {
        public IEnumerable<string> ResourceIds { get; set; }
    }

    public class RemoveSelectedApiResourcesSuccessAction
    {
        public IEnumerable<string> ResourceIds { get; set; }
    }

    public class StartAddApiResourceAction
    {

    }
}
