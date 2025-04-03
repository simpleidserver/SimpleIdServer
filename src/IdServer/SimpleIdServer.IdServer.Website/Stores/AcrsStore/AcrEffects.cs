// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models;
using FormBuilder.Models.Layout;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Helpers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore
{
    public class AcrEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly IRealmStore _realmStore;

        public AcrEffects(
            IWebsiteHttpClientFactory websiteHttpClientFactory, 
            IOptions<IdServerWebsiteOptions> options,
            IRealmStore realmStore)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _realmStore = realmStore;
        }


        [EffectMethod]
        public async Task Handle(GetAllAcrsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl)
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var acrs = SidJsonSerializer.Deserialize<IEnumerable<AuthenticationContextClassReference>>(json);
            dispatcher.Dispatch(new GetAllAcrsSuccessAction { Acrs = acrs });
        }

        [EffectMethod]
        public async Task Handle(AddAcrAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestJsonObj = new JsonObject
            {
                [AuthenticationContextClassReferenceNames.Name] = action.Name,
                [AuthenticationContextClassReferenceNames.DisplayName] = action.DisplayName
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(requestJsonObj.ToJsonString(), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newAcr = SidJsonSerializer.Deserialize<AuthenticationContextClassReference>(json);
                dispatcher.Dispatch(new AddAcrSuccessAction { Acr = newAcr });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddAcrFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(DeleteSelectedAcrsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
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

            dispatcher.Dispatch(new DeleteSelectedAcrsSuccessAction { Ids = action.Ids });
        }

        [EffectMethod]
        public async Task Handle(GetAllAuthenticationFormsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var httpResult = await httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/forms")
            });
            var json = await httpResult.Content.ReadAsStringAsync();
            var settings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var forms = JsonSerializer.Deserialize<List<FormRecord>>(json, settings);
            dispatcher.Dispatch(new GetAllAuthenticationFormsSuccessAction { AuthenticationForms = forms });
        }

        [EffectMethod]
        public async Task Handle(GetAcrAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var httpResult = await httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.Id}")
            });
            var json = await httpResult.Content.ReadAsStringAsync();
            var acr = SidJsonSerializer.Deserialize<AuthenticationContextClassReference>(json);
            dispatcher.Dispatch(new GetAcrSuccessAction { Acr = acr });
        }

        [EffectMethod]
        public async Task Handle(GetAllAuthenticationWorkflowLayoutsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var httpResult = await httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/workflowLayouts")
            });
            var json = await httpResult.Content.ReadAsStringAsync();
            var settings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var workflowLayouts = JsonSerializer.Deserialize<List<WorkflowLayout>>(json, settings);
            dispatcher.Dispatch(new GetAllAuthenticationWorkflowLayoutsSuccessAction { WorkflowLayouts = workflowLayouts });

        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realm = _realmStore.Realm;
                var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.Issuer}/{realmStr}/acrs";
            }

            return $"{_options.Issuer}/acrs";
        }
    }

    public class GetAllAcrsAction
    {

    }

    public class GetAllAcrsSuccessAction
    {
        public IEnumerable<AuthenticationContextClassReference> Acrs { get; set; }
    }

    public class AddAcrAction
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class AddAcrSuccessAction
    {
        public AuthenticationContextClassReference Acr { get; set; }
    }

    public class AddAcrFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class DeleteSelectedAcrsAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class DeleteSelectedAcrsSuccessAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class ToggleAllAcrSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleAcrSelectionAction
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class GetAllAuthenticationFormsAction
    {

    }

    public class GetAllAuthenticationFormsSuccessAction
    {
        public List<FormRecord> AuthenticationForms { get; set; }
    }

    public class GetAcrAction
    {
        public string Id { get; set; }
    }

    public class GetAcrSuccessAction
    {
        public AuthenticationContextClassReference Acr { get; set; }
    }

    public class GetAllAuthenticationWorkflowLayoutsAction
    {

    }

    public class GetAllAuthenticationWorkflowLayoutsSuccessAction
    {
        public List<WorkflowLayout> WorkflowLayouts { get; set; }
    }
}
