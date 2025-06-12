// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Client.DTOs;
using SimpleIdServer.Scim.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Client
{
    public class SCIMClient : IDisposable
    {
        private readonly HttpClientHandler _handler = null;
        private readonly string _baseUrl;
        private HttpClient _httpClient;
        private SearchResult<ResourceTypeResult> _resourceTypes;

        public SCIMClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public SCIMClient(string baseUrl, HttpClientHandler handler) : this(baseUrl)
        {
            _handler = handler;
        }

        public SCIMClient(string baseUrl, HttpClient httpClient) : this(baseUrl)
        {
            _httpClient = httpClient;
        }

        public async Task<SchemasResult> GetSchemas(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(GetPath("Schemas"))
            };
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SchemasResult>(json);
            return result;
        }

        public async Task<SearchResult<ResourceTypeResult>> GetResourceTypes(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(GetPath("ResourceTypes"))
            };
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync();
            _resourceTypes = JsonSerializer.Deserialize<SearchResult<ResourceTypeResult>>(json);
            return _resourceTypes;
        }

        public async Task<(SearchResult<RepresentationResult>, string)> SearchUsers(SearchRequest searchRequest, string accessToken, CancellationToken cancellationToken)
        {
            if (_resourceTypes == null) await GetResourceTypes(cancellationToken);
            var userEdp = _resourceTypes.Resources.Single(r => r.Name == "User").Endpoint;
            var queryString = SerializeQueryString(searchRequest);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{GetPath(userEdp)}?{queryString}")
            };
            if (!string.IsNullOrWhiteSpace(accessToken)) request.Headers.Add("Authorization", accessToken);
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var jsonObj = JsonObject.Parse(json).AsObject();
            return (RepresentationSerializer.DeserializeSearchRepresentations(jsonObj), json);
        }

        public async Task<(SearchResult<RepresentationResult>, string)> SearchGroups(SearchRequest searchRequest, string accessToken, CancellationToken cancellationToken)
        {
            if (_resourceTypes == null) await GetResourceTypes(cancellationToken);
            var groupEdp = _resourceTypes.Resources.Single(r => r.Name == "Group").Endpoint;
            var queryString = SerializeQueryString(searchRequest);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{GetPath(groupEdp)}?{queryString}")
            };
            if (!string.IsNullOrWhiteSpace(accessToken)) request.Headers.Add("Authorization", accessToken);
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var jsonObj = JsonObject.Parse(json).AsObject();
            return (RepresentationSerializer.DeserializeSearchRepresentations(jsonObj), json);
        }

        public async Task<RepresentationResult> GetGroup(string id, string accessToken, CancellationToken cancellationToken)
        {
            if (_resourceTypes == null) await GetResourceTypes(cancellationToken);
            var groupEdp = _resourceTypes.Resources.Single(r => r.Name == "Group").Endpoint;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{GetPath(groupEdp)}/{id}")
            };
            if (!string.IsNullOrWhiteSpace(accessToken)) request.Headers.Add("Authorization", accessToken);
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var jsonObj = JsonObject.Parse(json).AsObject();
            return RepresentationSerializer.DeserializeRepresentation(jsonObj);
        }

        public async Task<JsonObject> GetUser(string id, string accessToken, CancellationToken cancellationToken)
        {
            if (_resourceTypes == null) await GetResourceTypes(cancellationToken);
            var groupEdp = _resourceTypes.Resources.Single(r => r.Name == "User").Endpoint;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{GetPath(groupEdp)}/{id}")
            };
            if (!string.IsNullOrWhiteSpace(accessToken)) request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var jsonObj = JsonObject.Parse(json).AsObject();
            return jsonObj;
        }

        public async Task<SCIMErrorRepresentation> AddUser(JsonObject jsonObject, string accessToken, CancellationToken cancellationToken)
        {
            if (_resourceTypes == null) await GetResourceTypes(cancellationToken);
            var userEdp = _resourceTypes.Resources.Single(r => r.Name == "User").Endpoint;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(GetPath(userEdp)),
                Content = new StringContent(jsonObject.ToJsonString(), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(accessToken)) request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpClient = GetHttpClient();
            var httpResult = await httpClient.SendAsync(request, cancellationToken);
            if (httpResult.IsSuccessStatusCode) return null;
            var content = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SCIMErrorRepresentation>(content);
        }

        private HttpClient GetHttpClient()
        {
            if (_httpClient != null) return _httpClient;
            _httpClient = _httpClient == null ? new HttpClient() : new HttpClient(_handler);
            return _httpClient;
        }

        private string GetPath(string subPath)
        {
            var url = SanitizePath(_baseUrl);
            subPath = SanitizePath(subPath);
            return $"{url}/{subPath}";
        }

        private static string SanitizePath(string path)
        {
            if (path.EndsWith("/")) path = path.TrimEnd('/');
            if (path.StartsWith("/")) path = path.TrimStart('/');
            return path;
        }

        private static string SerializeQueryString(object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var dic = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        public void Dispose()
        {
            if(_httpClient != null) _httpClient.Dispose();
        }
    }
}
