﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Benchmark
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 150)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    [CsvExporter]
    [HtmlExporter]
    [RPlotExporter]
    public class ScimBenchmark
    {
        private const int _maxIterations = 150;
        private const string baseUrl = "https://localhost:5003";
        private string _groupId;
        private string _secondGroupId;
        private ConcurrentBag<string> _userIds = new ConcurrentBag<string>();

        [Benchmark]
        public async Task AddUserToGroupOneByOne()
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
        public async Task AddLargeSetOfUsersToOneGroup()
        {
            using (var httpClient = new HttpClient())
            {
                if (string.IsNullOrWhiteSpace(_secondGroupId))
                {
                    _secondGroupId = await AddGroup(httpClient);
                }

                Console.WriteLine(_userIds.Count());
                if(_userIds.Count() == _maxIterations - 1)
                {
                    await PatchGroup(httpClient, _secondGroupId, _userIds.ToList());
                    return;
                }

                var userId = await AddUser(httpClient);
                _userIds.Add(userId);
            }
        }

        // [Benchmark]
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

        private static Task PatchGroup(HttpClient httpClient, string groupId, string userId) => PatchGroup(httpClient, groupId, new List<string> { userId });

        private static async Task PatchGroup(HttpClient httpClient, string groupId, List<string> userIds)
        {
            var vals = new JArray();
            foreach (var userId in userIds)
            {
                var record = new JObject();
                record["value"] = userId;
                vals.Add(record);
            }

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
