// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.AuthenticationClassReferences;
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
            if (action.AuthenticationMethodReferences != null && action.AuthenticationMethodReferences.Any())
                requestJsonObj.Add(AuthenticationContextClassReferenceNames.AuthenticationMethodReferences, JsonArray.Parse(JsonSerializer.Serialize(action.AuthenticationMethodReferences)));

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
        public async Task Handle(AssignWorkflowRegistrationAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var request = new AssignRegistrationWorkflowRequest
            {
                WorkflowId = action.RegistrationWorkflowId
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.AcrId}/assign"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realm = _realmStore.Realm;
                var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/acrs";
            }

            return $"{_options.IdServerBaseUrl}/acrs";
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
        public IEnumerable<string> AuthenticationMethodReferences { get; set; }
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

    public class AssignWorkflowRegistrationAction
    {
        public string AcrId { get; set; }
        public string RegistrationWorkflowId { get; set; }
    }
}
