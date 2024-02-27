// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Website.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    public class RealmEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public RealmEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage protectedSessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = protectedSessionStorage;
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
                dispatcher.Dispatch(new SelectRealmAction
                {
                    Realm = action.Name
                });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddRealmFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(SelectRealmAction action, IDispatcher dispatcher)
        {
            await _sessionStorage.SetAsync("realm", action.Realm);
            dispatcher.Dispatch(new SelectRealmSuccessAction { Realm = action.Realm });
        }

        [EffectMethod]
        public async Task Handle(GetActiveRealmAction action, IDispatcher dispatcher)
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            dispatcher.Dispatch(new GetActiveSuccessRealmAction { Realm = realmStr });
        }

        private string GetRealmsUrl() => $"{_options.IdServerBaseUrl}/realms";
    }

    public class GetActiveRealmAction
    {

    }

    public class GetActiveSuccessRealmAction
    {
        public string Realm { get; set; }
    }

    public class GetAllRealmAction
    {
        public IEnumerable<Domains.Realm> Realms { get; set; }
    }

    public class GetAllRealmSuccessAction
    {
        public IEnumerable<Domains.Realm> Realms { get; set; }
    }

    public class SelectRealmAction
    {
        public string Realm { get; set; }
    }

    public class SelectRealmSuccessAction
    {
        public string Realm { get; set; }
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
}
