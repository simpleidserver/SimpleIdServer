// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.ScopeStore
{
    public class ScopeEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public ScopeEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }


        [EffectMethod]
        public async Task Handle(SearchScopesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var types = new List<ScopeProtocols>();
            if (!string.IsNullOrWhiteSpace(action.ClientType))
            {
                if (action.ClientType == SimpleIdServer.IdServer.WsFederation.WsFederationConstants.CLIENT_TYPE)
                    types = new List<ScopeProtocols> { ScopeProtocols.SAML };
                else
                    types = new List<ScopeProtocols> { ScopeProtocols.OAUTH, ScopeProtocols.OPENID };
            }

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/.search"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new SearchScopeRequest
                {
                    Filter = SanitizeExpression(action.Filter),
                    OrderBy = SanitizeExpression(action.OrderBy),
                    Skip = action.Skip,
                    Take = action.Take,
                    Protocols = types,
                    IsRole = action.IsRole
                    
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<SearchResult<Domains.Scope>>(json);
            dispatcher.Dispatch(new SearchScopesSuccessAction { Scopes = searchResult.Content, Count = searchResult.Count});

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedScopesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var id in action.ScopeIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{id}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedScopesSuccessAction { ScopeIds = action.ScopeIds, IsRole = action.IsRole });
        }

        [EffectMethod]
        public async Task Handle(AddIdentityScopeAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new Domains.Scope
            {
                Name = action.Name,
                Description = action.Description,
                Type = ScopeTypes.IDENTITY,
                IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp,
                Protocol = action.Protocol,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newScope = JsonSerializer.Deserialize<Domains.Scope>(json);
                dispatcher.Dispatch(new AddScopeSuccessAction { Id = newScope.Id, Name = action.Name, Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, Protocol = action.Protocol, Type = ScopeTypes.IDENTITY });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddScopeFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(AddApiScopeAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new Domains.Scope
            {
                Name = action.Name,
                Description = action.Description,
                Type = ScopeTypes.APIRESOURCE,
                IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp,
                Protocol = ScopeProtocols.OAUTH,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newScope = JsonSerializer.Deserialize<Domains.Scope>(json);
                dispatcher.Dispatch(new AddScopeSuccessAction { Id = newScope.Id, Name = action.Name, Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, Protocol = ScopeProtocols.OAUTH, Type = ScopeTypes.APIRESOURCE });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddScopeFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(GetScopeAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.ScopeId}"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<Domains.Scope>(json);
                dispatcher.Dispatch(new GetScopeSuccessAction { Scope = result });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new GetScopeFailureAction { ScopeId = action.ScopeId, ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateScopeAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new UpdateScopeRequest
            {
                Description = action.Description,
                IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.ScopeId}"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
             await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateScopeSuccessAction { Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, ScopeId = action.ScopeId });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedScopeMappersAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var mapperId in action.ScopeMapperIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.ScopeId}/mappers/{mapperId}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedScopeMappersSuccessAction { ScopeMapperIds = action.ScopeMapperIds, ScopeId = action.ScopeId });
        }

        [EffectMethod]
        public async Task Handle(AddScopeClaimMapperAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.ScopeId}/mappers"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(action.ClaimMapper), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newScopeClaimMapper = JsonSerializer.Deserialize<Domains.ScopeClaimMapper>(json);
                dispatcher.Dispatch(new AddScopeClaimMapperSuccessAction { ClaimMapper = newScopeClaimMapper, ScopeId = action.ScopeId });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddScopeClaimMapperFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateScopeClaimMapperAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetScopesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new UpdateScopeClaimRequest
            {
                SourceUserAttribute = action.ClaimMapper.SourceUserAttribute,
                SourceUserProperty = action.ClaimMapper.SourceUserProperty,
                TargetClaimPath = action.ClaimMapper.TargetClaimPath,
                SAMLAttributeName = action.ClaimMapper.SAMLAttributeName,
                TokenClaimJsonType = action.ClaimMapper.TokenClaimJsonType,
                IsMultiValued = action.ClaimMapper.IsMultiValued,
                IncludeInAccessToken = action.ClaimMapper.IncludeInAccessToken
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.ScopeId}/mappers/{action.ClaimMapper.Id}"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new UpdateScopeClaimMapperSuccessAction { ClaimMapper = action.ClaimMapper, ScopeId = action.ScopeId });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new UpdateScopeClaimMapperFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        private async Task<string> GetScopesUrl()
        {
            if (_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/scopes";
            }

            return $"{_options.IdServerBaseUrl}/scopes";
        }
    }

    public class SearchScopesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
        public string? ClientType { get; set; } = null;
        public bool IsRole { get; set; }
    }

    public class SearchScopesSuccessAction
    {
        public IEnumerable<Domains.Scope> Scopes { get; set; } = new List<Domains.Scope>();
        public int Count { get; set; }
    }

    public class ToggleScopeSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ScopeName { get; set; } = null!;
    }

    public class ToggleAllScopeSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedScopesAction
    {
        public ICollection<string> ScopeIds { get; set; } = new List<string>();
        public bool IsRole { get; set; }
    }

    public class RemoveSelectedScopesSuccessAction
    {
        public ICollection<string> ScopeIds { get; set; } = new List<string>();
        public bool IsRole { get; set; }
    }

    public class AddIdentityScopeAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public ScopeProtocols Protocol { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
    }

    public class AddScopeSuccessAction
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
        public ScopeProtocols Protocol { get; set; }
        public ScopeTypes Type { get; set; }
    }

    public class AddScopeFailureAction
    {
        public string Name { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class GetScopeAction
    {
        public string ScopeId { get; set; } = null!;
    }

    public class GetScopeSuccessAction
    {
        public Domains.Scope Scope { get; set; } = null!;
    }

    public class GetScopeFailureAction
    {
        public string ScopeId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateScopeAction
    {
        public string ScopeId { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; } = false;
    }

    public class UpdateScopeSuccessAction
    {
        public string ScopeId { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; } = false;
    }

    public class ToggleScopeMapperSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ScopeMapperId { get; set; } = null!;
    }

    public class ToggleAllScopeMapperSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedScopeMappersAction
    {
        public string ScopeId { get; set; } = null!;
        public ICollection<string> ScopeMapperIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedScopeMappersSuccessAction
    {
        public string ScopeId { get; set; } = null!;
        public ICollection<string> ScopeMapperIds { get; set; } = new List<string>();
    }

    public class AddScopeClaimMapperAction
    {
        public string ScopeId { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class AddScopeClaimMapperSuccessAction
    {
        public string ScopeId { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class AddScopeClaimMapperFailureAction
    {
        public string ScopeId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateScopeClaimMapperAction
    {
        public string ScopeId { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class UpdateScopeClaimMapperSuccessAction
    {
        public string ScopeId { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class UpdateScopeClaimMapperFailureAction
    {
        public string ScopeId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class AddApiScopeAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
    }
}