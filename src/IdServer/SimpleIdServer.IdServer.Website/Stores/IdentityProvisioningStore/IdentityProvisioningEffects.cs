// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Provisioning;
using SimpleIdServer.IdServer.Domains;
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
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/.search"),
                Content = new StringContent(JsonSerializer.Serialize(searchRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DTOs.SearchResult<IdentityProvisioningResult>>(json);
            dispatcher.Dispatch(new SearchIdentityProvisioningSuccessAction { IdentityProvisioningLst = result.Content, Count = result.Count });

            string SanitizeExpression(string expression) => expression?.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var id in action.Ids)
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{id}")
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedIdentityProvisioningSuccessAction { Ids = action.Ids });
        }

        [EffectMethod]
        public async Task Handle(GetIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IdentityProvisioningResult>(json);
            dispatcher.Dispatch(new GetIdentityProvisioningSuccessAction { IdentityProvisioning = result });
        }

        [EffectMethod]
        public async Task Handle(LaunchIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Name}/{action.Id}/enqueue")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new LaunchIdentityProvisioningSuccessAction { Id = action.Id, Name = action.Name });
        }

        [EffectMethod]
        public async Task Handle(UpdateIdProvisioningPropertiesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateIdentityProvisioningPropertiesRequest
            {
                Values = action.Properties
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}/values"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateIdProvisioningPropertiesSuccessAction { Id = action.Id, Properties = action.Properties });
        }

        [EffectMethod]
        public async Task Handle(UpdateIdProvisioningDetailsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new UpdateIdentityProvisioningDetailsRequest
            {
                Description = action.Description
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}/details"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateIdProvisioningDetailsSuccessAction { Description = action.Description, Id = action.Id });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedIdentityProvisioningMappingRulesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var id in action.MappingRuleIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}/mappers/{id}")
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedIdentityProvisioningMappingRulesSuccessAction { Id = action.Id, MappingRuleIds = action.MappingRuleIds });
        }

        [EffectMethod]
        public async Task Handle(AddIdentityProvisioningMappingRuleAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var addRequest = new AddIdentityProvisioningMapperRequest
            {
                MappingRule = action.MappingRule,
                From = action.From,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty,
                HasMultipleAttribute = action.HasMultipleAttribute
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}/mappers"),
                Content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var newMapper = JsonSerializer.Deserialize<IdentityProvisioningMappingRuleResult>(json);
            dispatcher.Dispatch(new AddIdentityProvisioningMappingRuleSuccessAction { NewId = newMapper.Id, Id = action.Id, MappingRule = action.MappingRule, From = action.From, TargetUserAttribute = action.TargetUserAttribute, TargetUserProperty = action.TargetUserProperty });
        }

        [EffectMethod]
        public async Task Handle(TestIdentityProvisioningAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}/test")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var extractionResult = JsonSerializer.Deserialize<TestConnectionResult>(json);
                dispatcher.Dispatch(new TestIdentityProvisioningSuccessAction { ConnectionResult = extractionResult });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new TestIdentityProvisioningFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(GetIdentityProvisioningAllowedAttributesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_options.IdServerBaseUrl}/{realm}/provisioning/{action.Id}/allowedattributes")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var allowedAttributes = JsonSerializer.Deserialize<List<string>>(json);
                dispatcher.Dispatch(new GetIdentityProvisioningAllowedAttributesSuccessAction { AllowedAttributes = allowedAttributes });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new GetIdentityProvisioningAllowedAttributesFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
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

    public class RemoveSelectedIdentityProvisioningAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class RemoveSelectedIdentityProvisioningSuccessAction
    {
        public IEnumerable<string> Ids { get; set; }
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
    }

    public class AddIdentityProvisioningMappingRuleSuccessAction
    {
        public string Id { get; set; }
        public string NewId { get; set; }
        public MappingRuleTypes MappingRule { get; set; }
        public string From { get; set; } = null!;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
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

    public class GetIdentityProvisioningAllowedAttributesAction
    {
        public string Id { get; set; }
    }

    public class GetIdentityProvisioningAllowedAttributesSuccessAction
    {
        public List<string> AllowedAttributes { get; set; }
    }

    public class GetIdentityProvisioningAllowedAttributesFailureAction
    {
        public string ErrorMessage { get; set; }
    }
}
