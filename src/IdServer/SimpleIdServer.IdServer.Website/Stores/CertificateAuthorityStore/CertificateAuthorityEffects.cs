// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.CertificateAuthorities;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Store;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    public class CertificateAuthorityEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public CertificateAuthorityEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchCertificateAuthoritiesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
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
            var searchResult = SidJsonSerializer.Deserialize<SearchResult<CertificateAuthority>>(json);
            dispatcher.Dispatch(new SearchCertificateAuthoritiesSuccessAction { CertificateAuthorities = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(GenerateCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/generate"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new GenerateCertificateAuthorityRequest
                {
                    NumberOfDays = action.NumberOfDays,
                    SubjectName = action.SubjectName
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var certificateAuthority = SidJsonSerializer.Deserialize<CertificateAuthority>(json);
                dispatcher.Dispatch(new GenerateCertificateAuthoritySuccessAction { CertificateAuthority = certificateAuthority });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new GenerateCertificateAuthorityFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(ImportCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/import"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new ImportCertificateAuthorityRequest
                {
                    FindType = action.FindType,
                    FindValue = action.FindValue,
                    StoreLocation = action.StoreLocation,
                    StoreName = action.StoreName
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var certificateAuthority = SidJsonSerializer.Deserialize<CertificateAuthority>(json);
                dispatcher.Dispatch(new GenerateCertificateAuthoritySuccessAction { CertificateAuthority = certificateAuthority });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new GenerateCertificateAuthorityFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(SaveCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            action.CertificateAuthority.StartDateTime = action.CertificateAuthority.StartDateTime.ToUniversalTime();
            action.CertificateAuthority.EndDateTime = action.CertificateAuthority.EndDateTime.ToUniversalTime();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(action.CertificateAuthority), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var certificateAuthority = SidJsonSerializer.Deserialize<CertificateAuthority>(json);
                dispatcher.Dispatch(new SaveCertificateAuthoritySuccessAction { CertificateAuthority = certificateAuthority });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new SaveCertificateAuthorityFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCertificateAuthoritiesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var id in action.Ids)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{id}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedCertificateAuthoritiesSuccessAction { Ids = action.Ids });
        }

        [EffectMethod]
        public async Task Handle(GetCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.Id}"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var certificateAuthority = SidJsonSerializer.Deserialize<CertificateAuthority>(json);
            var store = new IdServer.Stores.CertificateAuthorityStore(null);
            var certificate = store.Get(certificateAuthority);
            dispatcher.Dispatch(new GetCertificateAuthoritySuccessAction { CertificateAuthority = certificateAuthority, Certificate = certificate });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedClientCertificatesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var id in action.CertificateClientIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.CertificateAuthorityId}/clientcertificates/{id}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedClientCertificatesSuccessAction { CertificateAuthorityId = action.CertificateAuthorityId, CertificateClientIds = action.CertificateClientIds });
        }

        [EffectMethod]
        public async Task Handle(AddClientCertificateAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetBaseUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.CertificateAuthorityId}/clientcertificates"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new AddClientCertificateRequest
                {
                    NbDays = action.NbDays,
                    SubjectName = action.SubjectName
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var clientCertificate = SidJsonSerializer.Deserialize<ClientCertificate>(json);
                dispatcher.Dispatch(new AddClientCertificateSuccessAction { CertificateAuthorityId = action.CertificateAuthorityId, ClientCertificate = clientCertificate });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddClientCertificateFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        private async Task<string> GetBaseUrl()
        {
            if (_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/cas";
            }

            return $"{_options.IdServerBaseUrl}/cas";
        }
    }

    public class SearchCertificateAuthoritiesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchCertificateAuthoritiesSuccessAction
    {
        public IEnumerable<CertificateAuthority> CertificateAuthorities { get; set; } = new List<CertificateAuthority>();
        public int Count { get; set; }
    }

    public class ToggleAllCertificateAuthoritySelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCertificateAuthoritySelectionAction
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }
    }

    public class RemoveSelectedCertificateAuthoritiesAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class RemoveSelectedCertificateAuthoritiesSuccessAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class GenerateCertificateAuthorityAction
    {
        public string SubjectName { get; set; }
        public int NumberOfDays { get; set; }
    }

    public class ImportCertificateAuthorityAction
    {
        public StoreLocation StoreLocation { get; set; }
        public StoreName StoreName { get; set; }
        public X509FindType FindType { get; set; }
        public string FindValue { get; set; }
    }

    public class GenerateCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class GenerateCertificateAuthorityFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class SaveCertificateAuthorityAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class SaveCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class SaveCertificateAuthorityFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class SelectCertificateAuthoritySourceAction
    {
        public CertificateAuthoritySources Source { get; set; }
    }

    public class GetCertificateAuthorityAction
    {
        public string Id { get; set; }
    }

    public class GetCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }

    public class ToggleClientCertificateSelectionAction
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllClientCertificatesSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedClientCertificatesAction
    {
        public string CertificateAuthorityId { get; set; }
        public IEnumerable<string> CertificateClientIds { get; set; }
    }

    public class RemoveSelectedClientCertificatesSuccessAction
    {
        public string CertificateAuthorityId { get; set; }
        public IEnumerable<string> CertificateClientIds { get; set; }
    }

    public class AddClientCertificateAction
    {
        public string CertificateAuthorityId { get; set; }
        public string SubjectName { get; set; }
        public int NbDays { get; set; }
    }

    public class AddClientCertificateSuccessAction
    {
        public string CertificateAuthorityId { get; set; }
        public ClientCertificate ClientCertificate { get; set; }
    }

    public class AddClientCertificateFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class StartAddCertificateAuthorityAction
    {

    }
}
