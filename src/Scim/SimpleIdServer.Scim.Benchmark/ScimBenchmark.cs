// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Benchmark
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 300)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    [CsvExporter]
    [HtmlExporter]
    [RPlotExporter]
    public class ScimBenchmark
    {
        private const string baseUrl = "http://localhost:60002";
        private string _groupId;

        // [Benchmark]
        public async Task AddUserToGroup()
        {
            using (var httpClient = new HttpClient())
            {
                if(string.IsNullOrWhiteSpace(_groupId))
                {
                    _groupId = await AddGroup(httpClient);
                }

                var userId = await AddUser(httpClient);
                await PatchGroup(httpClient, _groupId, userId);
                // await PatchUser(httpClient, userId, _groupId);
            }
        }

        [Benchmark]
        public async Task SearchUsers()
        {
            using (var httpClient = new HttpClient())
            {
                await SearchUsers(httpClient);
            }
        }

        private static async Task<string> AddUser(HttpClient httpClient)
        {
            string id = Guid.NewGuid().ToString();
            var jObj = new JObject
            {
                { "schemas", new JArray(new string[] { "urn:ietf:params:scim:schemas:core:2.0:User" }) },
                { "userName", id },
                { "externalId", "externalId" },
                { "name", new JObject
                {
                    { "formatted", "formatted" },
                    { "familyName", "familyName" },
                    { "givenName", "givenName" }
                } }
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}/Users"),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResponse = await httpClient.SendAsync(request);
            var json = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());
            return json["id"].ToString();
        }

        private static async Task<string> AddGroup(HttpClient httpClient)
        {
            string id = Guid.NewGuid().ToString();
            var jObj = new JObject
            {
                { "schemas", new JArray(new string[] { "urn:ietf:params:scim:schemas:core:2.0:Group" }) },
                { "displayName", id }
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}/Groups"),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResponse = await httpClient.SendAsync(request);
            var json = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());
            return json["id"].ToString();
        }

        private static async Task SearchUsers(HttpClient httpClient)
        {
            await httpClient.GetAsync($"{baseUrl}/Users?excludedAttributes=groups");
        }

        private static async Task PatchGroup(HttpClient httpClient, string groupId, string userId)
        {
            var vals = new JArray();
            vals.Add(new JObject
            {
                { "value", userId }
            });
            var ops = new JArray();
            ops.Add(new JObject
            {
                { "op", "add" },
                { "path", "members" },
                { "value", vals }
            });
            var jObj = new JObject
            {
                { "schemas", new JArray(new string[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" }) },
                { "Operations", ops }
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"{baseUrl}/Groups/{groupId}"),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(request);
        }

        private static async Task PatchUser(HttpClient httpClient, string userId, string groupId)
        {
            var vals = new JArray();
            vals.Add(new JObject
            {
                { "value", groupId }
            });
            var ops = new JArray();
            ops.Add(new JObject
            {
                { "op", "add" },
                { "path", "groups" },
                { "value", vals }
            });
            var jObj = new JObject
            {
                { "schemas", new JArray(new string[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" }) },
                { "Operations", ops }
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"{baseUrl}/Users/{userId}"),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(request);
        }
    }
}
