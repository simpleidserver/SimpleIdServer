﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    public class IdProviderEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public IdProviderEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IDbContextFactory<StoreDbContext> factory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _factory = factory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchIdProvidersAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var searchRequest = new DTOs.SearchRequest
            {
                Filter = SanitizeExpression(action.Filter),
                OrderBy = SanitizeExpression(action.OrderBy),
                Skip = action.Skip,
                Take = action.Take
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/.search"),
                Content = new StringContent(JsonSerializer.Serialize(searchRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DTOs.SearchResult<AuthenticationSchemeProviderResult>>(json);
            dispatcher.Dispatch(new SearchIdProvidersSuccessAction { IdProviders = result.Content, Count = result.Count });

            string SanitizeExpression(string expression) => expression?.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedIdProvidersAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var id in action.Ids)
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{id}")
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedIdProvidersSuccessAction { Ids = action.Ids });
        }

        [EffectMethod]
        public async Task Handle(GetIdProviderAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{action.Id}")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthenticationSchemeProviderResult>(json);
            dispatcher.Dispatch(new GetIdProviderSuccessAction { IdProvider = result });
        }

        [EffectMethod]
        public async Task Handle(AddIdProviderAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new AddAuthenticationSchemeProviderRequest
            {
                DefinitionName = action.IdProviderTypeName,
                Description = action.Description,
                Name = action.Name,
                DisplayName = action.DisplayName,
                Values = action.Properties
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new AddIdProviderSuccessAction { Name = action.Name, Description = action.Description, DisplayName = action.DisplayName });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddIdProviderFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateIdProviderDetailsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateAuthenticationSchemeProviderDetailsRequest
            {
                Description = action.Description,
                DisplayName = action.DisplayName
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{action.Name}/details"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateIdProviderDetailsSuccessAction { Description = action.Description, DisplayName = action.DisplayName, Name = action.Name });
        }

        [EffectMethod]
        public async Task Handle(UpdateAuthenticationSchemeProviderPropertiesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateAuthenticationSchemeProviderValuesRequest
            {
                Values = action.Properties
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{action.Name}/values"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateAuthenticationSchemeProviderPropertiesSuccessAction { Name = action.Name, Properties = action.Properties });
        }

        [EffectMethod]
        public async Task Handle(AddAuthenticationSchemeProviderMapperAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new AddAuthenticationSchemeProviderMapperRequest
            {
                MapperType = action.MapperType,
                Name = action.Name,
                SourceClaimName = action.SourceClaimName,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{action.IdProviderName}/mappers"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var newMapper = JsonSerializer.Deserialize<AuthenticationSchemeProviderMapperResult>(json);
            dispatcher.Dispatch(new AddAuthenticationSchemeProviderMapperSuccessAction
            {
                Id = newMapper.Id,
                IdProviderName = action.IdProviderName,
                MapperType = action.MapperType,
                Name = action.Name,
                SourceClaimName = action.SourceClaimName,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedAuthenticationSchemeProviderMappersAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var id in action.MapperIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{action.Name}/mappers/{id}")
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedAuthenticationSchemeProviderMappersSuccessAction
            {
                Name = action.Name,
                MapperIds = action.MapperIds
            });
        }

        [EffectMethod]
        public async Task Handle(UpdateAuthenticationSchemeProviderMapperAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateAuthenticationSchemeProviderMapperRequest
            {
                Name = action.Name,
                SourceClaimName = action.SourceClaimName,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/idproviders/{action.IdProviderName}/mappers/{action.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateAuthenticationSchemeProviderMapperSuccessAction
            {
                Id = action.Id,
                Name = action.Name,
                IdProviderName = action.IdProviderName,
                SourceClaimName = action.SourceClaimName,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            });
        }

        [EffectMethod]
        public async Task Handle(GetIdProviderDefsAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var idProviderDefs = await dbContext.AuthenticationSchemeProviderDefinitions.AsNoTracking().ToListAsync();
                dispatcher.Dispatch(new GetIdProviderDefsSuccessAction { AuthProviderDefinitions = idProviderDefs });
            }
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchIdProvidersAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchIdProvidersSuccessAction
    {
        public ICollection<AuthenticationSchemeProviderResult> IdProviders { get; set; }
        public int Count { get; set; }
    }

    public class RemoveSelectedIdProvidersAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class RemoveSelectedIdProvidersSuccessAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class GetIdProviderAction
    {
        public string Id { get; set; }
    }

    public class GetIdProviderFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class GetIdProviderSuccessAction
    {
        public AuthenticationSchemeProviderResult IdProvider { get; set; }
    }

    public class ToggleIdProviderSelectionAction
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllIdProvidersSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class GetIdProviderDefsAction
    {

    }

    public class GetIdProviderDefsSuccessAction
    {
        public IEnumerable<AuthenticationSchemeProviderDefinition> AuthProviderDefinitions { get; set; }
    }

    public class AddIdProviderAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string IdProviderTypeName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public Dictionary<string, string> Properties { get; set; }
    }

    public class AddIdProviderFailureAction
    {
        public string ErrorMessage { get; set; } = null!;
    }

    public class AddIdProviderSuccessAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; } = null;
        // public IEnumerable<AuthenticationSchemeProviderProperty> Properties { get; set; }
    }

    public class UpdateIdProviderDetailsAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; }
    }

    public class UpdateIdProviderDetailsSuccessAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; }
    }

    public class UpdateAuthenticationSchemeProviderPropertiesAction
    {
        public string Name { get; set; } = null!;
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }

    public class UpdateAuthenticationSchemeProviderPropertiesSuccessAction
    {
        public string Name { get; set; } = null!;
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }

    public class RemoveSelectedAuthenticationSchemeProviderMappersAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<string> MapperIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedAuthenticationSchemeProviderMappersSuccessAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<string> MapperIds { get; set; } = new List<string>();
    }

    public class ToggleAuthenticationSchemeProviderMapperAction
    {
        public string MapperId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllAuthenticationSchemeProviderSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class AddAuthenticationSchemeProviderMapperAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public MappingRuleTypes MapperType { get; set; }
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }

    public class AddAuthenticationSchemeProviderMapperSuccessAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public MappingRuleTypes MapperType { get; set; }
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }

    public class UpdateAuthenticationSchemeProviderMapperAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }

    public class UpdateAuthenticationSchemeProviderMapperSuccessAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }
}
