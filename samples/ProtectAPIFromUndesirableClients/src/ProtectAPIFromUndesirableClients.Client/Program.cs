// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProtectAPIFromUndesirableClients.Client
{
    class Program
    {
        private const string CLIENT_ID = "application";
        private const string CLIENT_SECRET = "applicationSecret";
        private const string BASE_OAUTH_URL = "https://localhost:60001";
        private const string BASE_API_URI = "https://localhost:5002";

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to start the execution");
            Console.ReadLine();
            AddAndGetUser().Wait();
            Console.WriteLine("Please press any key to quit the application");
            Console.ReadKey();
        }

        private static async Task AddAndGetUser()
        {
            var accessToken = await GetAccessToken(new[] { "get_user", "add_user" });
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine("Add user");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(new JObject
                    {
                        { "firstname", "firstname" },
                        { "lastname", "lastname" }
                    }.ToString(), Encoding.UTF8, "application/json"),
                    RequestUri = new Uri($"{BASE_API_URI}/users")
                };
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                var httpResult = await httpClient.SendAsync(request);
                var json = await httpResult.Content.ReadAsStringAsync();
                var userId = JObject.Parse(json)["_id"].ToString();
                Console.WriteLine($"User with identifier {userId} has been added");
                request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{BASE_API_URI}/users/{userId}")
                };
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                httpResult = await httpClient.SendAsync(request);
                json = await httpResult.Content.ReadAsStringAsync();
                Console.WriteLine($"User returned by the API : {json}");
            }
        }

        private static async Task<string> GetAccessToken(IEnumerable<string> scopes)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{BASE_OAUTH_URL}/token"),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", CLIENT_ID },
                        { "client_secret", CLIENT_SECRET },
                        { "grant_type", "client_credentials" },
                        { "scope", string.Join(" ", scopes) }
                    })
                };
                var httpResult = await httpClient.SendAsync(request);
                var json = await httpResult.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(json);
                return jObj["access_token"].ToString();
            }
        }
    }
}
