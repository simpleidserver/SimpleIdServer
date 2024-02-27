// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Provisioning;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Provisioning;
using SimpleIdServer.IdServer.Store;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    public class IdentityProvisioningEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public IdentityProvisioningEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var searchRequest = new SearchRequest
            {
                Filter = SanitizeExpression(action.Filter),
                OrderBy = SanitizeExpression(action.OrderBy),
                Skip = action.Skip,
                Take = action.Take
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}/.search"),
                Content = new StringContent(JsonSerializer.Serialize(searchRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = SidJsonSerializer.Deserialize<DTOs.SearchResult<IdentityProvisioningResult>>(json);
            dispatcher.Dispatch(new SearchIdentityProvisioningSuccessAction { IdentityProvisioningLst = result.Content, Count = result.Count });

            string SanitizeExpression(string expression) => expression?.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(GetIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/{action.Id}")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = SidJsonSerializer.Deserialize<IdentityProvisioningResult>(json);
            dispatcher.Dispatch(new GetIdentityProvisioningSuccessAction { IdentityProvisioning = result });
        }

        [EffectMethod]
        public async Task Handle(LaunchIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.Id}/extract")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var launchedProcess = SidJsonSerializer.Deserialize<IdentityProvisioningLaunchedResult>(json);
                dispatcher.Dispatch(new LaunchIdentityProvisioningSuccessAction { Id = action.Id, Name = action.Name, ProcessId = launchedProcess.Id });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new LaunchIdentityProvisioningFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }

        }

        [EffectMethod]
        public async Task Handle(UpdateIdProvisioningPropertiesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateIdentityProvisioningPropertiesRequest
            {
                Values = action.Properties
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/values"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateIdProvisioningPropertiesSuccessAction { Id = action.Id, Properties = action.Properties });
        }

        [EffectMethod]
        public async Task Handle(UpdateIdProvisioningDetailsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateIdentityProvisioningDetailsRequest
            {
                Description = action.Description
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/details"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateIdProvisioningDetailsSuccessAction { Description = action.Description, Id = action.Id });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedIdentityProvisioningMappingRulesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var id in action.MappingRuleIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{baseUrl}/{action.Id}/mappers/{id}")
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedIdentityProvisioningMappingRulesSuccessAction { Id = action.Id, MappingRuleIds = action.MappingRuleIds });
        }

        [EffectMethod]
        public async Task Handle(AddIdentityProvisioningMappingRuleAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new AddIdentityProvisioningMapperRequest
            {
                MappingRule = action.MappingRule,
                From = action.From,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty,
                HasMultipleAttribute = action.HasMultipleAttribute,
                Usage = action.Usage
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/mappers"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newMapper = SidJsonSerializer.Deserialize<IdentityProvisioningMappingRuleResult>(json);
                dispatcher.Dispatch(new AddIdentityProvisioningMappingRuleSuccessAction { Usage = action.Usage, NewId = newMapper.Id, Id = action.Id, MappingRule = action.MappingRule, From = action.From, TargetUserAttribute = action.TargetUserAttribute, TargetUserProperty = action.TargetUserProperty });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddIdentityProvisioningMappingRuleFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }


        [EffectMethod]
        public async Task Handle(GetIdentityProvisioningMappingRuleAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/mappers/{action.MappingRuleId}")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var mappingRule = SidJsonSerializer.Deserialize<IdentityProvisioningMappingRuleResult>(json);
                dispatcher.Dispatch(new GetIdentityPriovisioningMappingRuleSuccessAction { MappingRule = mappingRule });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new GetIdentityPriovisioningMappingRuleFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateIdentityProvisioningMappingRuleAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var request = new UpdateIdentityProvisioningMapperRequest
            {
                From = action.From,
                HasMultipleAttribute = action.HasMultipleAttribute,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/mappers/{action.MappingRuleId}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new UpdateIdentityProvisioningMappingRuleSuccessAction { Id = action.Id, From = action.From, HasMultipleAttribute = action.HasMultipleAttribute, TargetUserAttribute = action.TargetUserAttribute, MappingRuleId = action.MappingRuleId, TargetUserProperty = action.TargetUserProperty });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new UpdateIdentityProvisioningMappingRuleFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(TestIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/{action.Id}/test")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var extractionResult = SidJsonSerializer.Deserialize<TestConnectionResult>(json);
                dispatcher.Dispatch(new TestIdentityProvisioningSuccessAction { ConnectionResult = extractionResult });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new TestIdentityProvisioningFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(LaunchIdentityProvisioningImportAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/{action.InstanceId}/{action.ProcessId}/import")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new LaunchIdentityProvisioningImportSuccessAction
                {
                    InstanceId = action.InstanceId,
                    ProcessId = action.ProcessId
                });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new LaunchIdentityProvisioningImportFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/provisioning";
            }

            return $"{_options.IdServerBaseUrl}/provisioning";
        }
    }

    public class SearchIdentityProvisioningAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchIdentityProvisioningSuccessAction
    {
        public IEnumerable<IdentityProvisioningResult> IdentityProvisioningLst { get; set; }
        public int Count { get; set; }
    }

    public class GetIdentityProvisioningAction
    {
        public string Id { get; set; }
    }

    public class GetIdentityProvisioningFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class GetIdentityProvisioningSuccessAction
    {
        public IdentityProvisioningResult IdentityProvisioning { get; set; }
    }

    public class ToggleAllIdentityProvisioningAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleIdentityProvisioningSelectionAction
    {
        public bool IsSelected { get; set; }
        public string IdentityProvisioningId { get; set; }
    }

    public class LaunchIdentityProvisioningAction
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class LaunchIdentityProvisioningSuccessAction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProcessId { get; set; }
    }

    public class LaunchIdentityProvisioningFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class UpdateIdProvisioningPropertiesAction
    {
        public string Id { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class UpdateIdProvisioningPropertiesSuccessAction
    {
        public string Id { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class UpdateIdProvisioningDetailsAction
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class UpdateIdProvisioningDetailsSuccessAction
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class SelectIdentityProvisioningMappingRuleAction
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }
    }

    public class SelectAllIdentityProvisioningMappingRulesAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedIdentityProvisioningMappingRulesAction
    {
        public IEnumerable<string> MappingRuleIds { get; set; }
        public string Id { get; set; }
    }

    public class RemoveSelectedIdentityProvisioningMappingRulesSuccessAction
    {
        public IEnumerable<string> MappingRuleIds { get; set; }
        public string Id { get; set; }
    }

    public class AddIdentityProvisioningMappingRuleAction
    {
        public string Id { get; set; }
        public MappingRuleTypes MappingRule { get; set; }
        public string From { get; set; } = null!;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
        public bool HasMultipleAttribute { get; set; }
        public IdentityProvisioningMappingUsage Usage { get; set; }
    }

    public class AddIdentityProvisioningMappingRuleSuccessAction
    {
        public string Id { get; set; }
        public string NewId { get; set; }
        public MappingRuleTypes MappingRule { get; set; }
        public string From { get; set; } = null!;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
        public IdentityProvisioningMappingUsage Usage { get; set; } 
    }

    public class AddIdentityProvisioningMappingRuleFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class TestIdentityProvisioningAction
    {
        public string Id { get; set; }
    }

    public class TestIdentityProvisioningFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class TestIdentityProvisioningSuccessAction
    {
        public TestConnectionResult ConnectionResult { get; set; }
    }

    public class GetIdentityProvisioningMappingRuleAction
    {
        public string Id { get; set; }
        public string MappingRuleId { get; set; }
    }

    public class GetIdentityPriovisioningMappingRuleSuccessAction
    {
        public string Id { get; set; }
        public IdentityProvisioningMappingRuleResult MappingRule { get; set; }
    }

    public class GetIdentityPriovisioningMappingRuleFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class UpdateIdentityProvisioningMappingRuleAction
    {
        public string Id { get; set; }
        public string MappingRuleId { get; set; }
        public string From { get; set; } = null!;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
        public bool HasMultipleAttribute { get; set; }
    }

    public class UpdateIdentityProvisioningMappingRuleSuccessAction
    {
        public string Id { get; set; }
        public string MappingRuleId { get; set; }
        public string From { get; set; } = null!;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
        public bool HasMultipleAttribute { get; set; }
    }

    public class UpdateIdentityProvisioningMappingRuleFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class LaunchIdentityProvisioningImportAction
    {
        public string InstanceId { get; set; }
        public string ProcessId { get; set; }
    }

    public class LaunchIdentityProvisioningImportSuccessAction
    {
        public string InstanceId { get; set; }
        public string ProcessId { get; set; }
    }

    public class LaunchIdentityProvisioningImportFailureAction
    {
        public string ErrorMessage { get; set; }
    }
}
