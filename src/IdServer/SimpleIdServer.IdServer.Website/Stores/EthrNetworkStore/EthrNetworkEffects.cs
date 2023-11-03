// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.EthrNetworkStore
{
    public class EthrNetworkEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionsStorage;

        public EthrNetworkEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionsStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionsStorage = sessionsStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchEthrNetworksAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var networks = JsonSerializer.Deserialize<IEnumerable<SimpleIdServer.IdServer.Domains.NetworkConfiguration>>(json);
            var nb = networks.Count();
            dispatcher.Dispatch(new SearchEthrNetworksSuccessAction { Networks = networks, Count = nb });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(AddEthrNetworkAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var request = new JsonObject
            {
                { "name", action.Name },
                { "rpc_url", action.RpcUrl },
                { "private_accountkey", action.PrivateAccountKey }
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(request.ToJsonString(), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new AddEthrNetworkSuccessAction { Name = action.Name, RpcUrl = action.RpcUrl, PrivateAccountKey = action.PrivateAccountKey });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddEthrNetworkFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedEthrContractAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var name in action.Names)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{name}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedEthrContractSuccessAction { Names = action.Names });
        }

        [EffectMethod]
        public async Task Handle(DeployEthrContractAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.Name}/deploy"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var jObj = JsonObject.Parse(json);
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new DeployEthrContractSuccessAction { Name = action.Name, ContractAdr = jObj["contract_adr"].GetValue<string>() });
            }
            catch
            {
                dispatcher.Dispatch(new DeployEthrContractFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realm = await _sessionsStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/networks";
            }

            return $"{_options.IdServerBaseUrl}/networks";
        }
    }

    public class SearchEthrNetworksAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchEthrNetworksSuccessAction
    {
        public IEnumerable<NetworkConfiguration> Networks { get; set; } = new List<NetworkConfiguration>();
        public int Count { get; set; }
    }

    public class AddEthrNetworkAction
    {
        public string Name { get; set; }
        public string RpcUrl { get; set; }
        public string PrivateAccountKey { get; set; }
    }

    public class AddEthrNetworkSuccessAction
    {
        public string Name { get; set; }
        public string RpcUrl { get; set; }
        public string PrivateAccountKey { get; set; }
    }

    public class AddEthrNetworkFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class DeployEthrContractAction
    {
        public string Name { get; set; }
    }

    public class DeployEthrContractSuccessAction
    {
        public string Name { get; set; }
        public string ContractAdr { get; set; }
    }

    public class DeployEthrContractFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class RemoveSelectedEthrContractAction
    {
        public IEnumerable<string> Names { get; set; }
    }

    public class RemoveSelectedEthrContractSuccessAction
    {
        public IEnumerable<string> Names { get; set; }
    }

    public class ToggleEthrContractAction
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SelectOneEthrContractAction
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllEthrContractAction
    {
        public bool IsSelected { get; set; }
    }
}
