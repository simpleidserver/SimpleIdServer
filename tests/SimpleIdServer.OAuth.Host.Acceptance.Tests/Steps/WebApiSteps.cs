// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class WebApiSteps
    {
        private static IEnumerable<string> PARAMETERS_IN_HEADER = new[] { "Authorization", "X-Testing-ClientCert" };
        private ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<Program> _factory;

        public WebApiSteps(ScenarioContext scenarioContext, CustomWebApplicationFactory<Program> factory)
        {
            _scenarioContext = scenarioContext;
            _factory = factory;
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var mock = new Mock<Infrastructures.IHttpClientFactory>();
            mock.Setup(m => m.GetHttpClient()).Returns(client);
        }

        [When("add user consent : user='(.*)', scope='(.*)', clientId='(.*)'")]
        public Task WhenAddUserConsent(string user, string scope, string clientId)
        {
            return Task.CompletedTask;
        }

        [When("add JSON web key to Authorization Server and store into '(.*)'")]
        public Task WhenAddJsonWebKey(string name, Table table)
        {
            return Task.CompletedTask;
        }

        [When("execute HTTP GET request '(.*)'")]
        public async Task GivenExecuteHTTPGetRequest(string url, Table table)
        {
            url = ExtractUrl(url, table);
            var headers = ExtractHeaders(table);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            foreach(var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PUT request '(.*)'")]
        public async Task GivenExecuteHTTPPutRequest(string url, Table table)
        {
            url = ExtractUrl(url, table);
            var headers = ExtractHeaders(table);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };
            foreach (var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP DELETE request '(.*)'")]
        public async Task GivenExecuteHTTPDeleteRequest(string url, Table table)
        {
            url = ExtractUrl(url, table);
            var headers = ExtractHeaders(table);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url)
            };
            foreach(var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PUT JSON request '(.*)'")]
        public async Task GivenExecuteHTTPPutJSONRequest(string url, Table table)
        {
            url = ParseValue(url).ToString();
            var headers = ExtractHeaders(table);
            var jObj = ExtractBody(table);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            foreach(var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP POST JSON request '(.*)'")]
        public async Task GivenExecuteHTTPPostJSONRequest(string url, Table table)
        {
            url = ParseValue(url).ToString();
            var headers = ExtractHeaders(table);
            var jObj = ExtractBody(table);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };

            foreach(var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP POST request '(.*)'")]
        public async Task GivenExecuteHTTPPostRequest(string url, Table table)
        {
            var jObj = new List<KeyValuePair<string, string>>();
            var headers = ExtractHeaders(table);
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]).ToString();
                if (PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                jObj.Add(new KeyValuePair<string, string>(record["Key"], value));
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new FormUrlEncodedContent(jObj)
            };
            
            foreach(var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpClient = _factory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("build software statement")]
        public void WhenBuildSoftwareStatement(Table table)
        {
            /*
            var jwsPayload = new JwsPayload();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                if (value.StartsWith('[') && value.EndsWith(']'))
                {
                    value = value.TrimStart('[').TrimEnd(']');
                    var splitted = value.Split(',');
                    jwsPayload.Add(key, JArray.FromObject(splitted));
                }
                else
                {
                    jwsPayload.Add(key, value);
                }
            }

            var jwtBuilder = (IJwtBuilder)_factory.Server.Host.Services.GetService(typeof(IJwtBuilder));
            // var jwk = FakeJwks.GetInstance().Jwks.First();
            // var jws = jwtBuilder.Sign(jwsPayload, jwk, jwk.Alg);
            // _scenarioContext.Set(jws, "softwareStatement");
            */
        }

        private string ExtractUrl(string url, Table table)
        {
            url = ParseValue(url).ToString();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]).ToString();
                if (PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                url = QueryHelpers.AddQueryString(url, record["Key"], value);
            }

            return url;
        }

        private JsonObject ExtractBody(Table table)
        {
            var result = new JsonObject();
            foreach(var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]);
                if (PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                if (value is JsonArray)
                {
                    result.Add(key, value as JsonArray);
                }
                else
                {
                    result.Add(key, value.ToString());
                }
            }

            return result;
        }

        private Dictionary<string, string> ExtractHeaders(Table table)
        {
            var result = new Dictionary<string, string>();
            foreach(var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]).ToString();
                if (!PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                result.Add(key, value.ToString());
            }

            return result;
        }

        private object ParseValue(string val)
        {
            if (val.StartsWith('$') && val.EndsWith('$'))
            {
                val = val.TrimStart('$').TrimEnd('$');
                return _scenarioContext.Get<object>(val);
            }

            if (val.StartsWith('[') && val.EndsWith(']'))
            {
                val = val.TrimStart('[').TrimEnd(']');
                var res = new JsonArray();
                foreach (var item in val.Split(',')) res.Add(item);
                return res;
            }

            var regularExpression = new Regex(@"\$([a-zA-Z]|_)*\$");
            var result = regularExpression.Replace(val, (m) =>
            {
                if (string.IsNullOrWhiteSpace(m.Value))
                {
                    return string.Empty;
                }

                return _scenarioContext.Get<string>(m.Value.TrimStart('$').TrimEnd('$'));
            });

            return result;
        }
    }
}