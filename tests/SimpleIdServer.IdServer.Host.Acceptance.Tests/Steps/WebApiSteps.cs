// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class WebApiSteps
    {
        private static object _lck = new object();
        private static IEnumerable<string> PARAMETERS_IN_HEADER = new[] { "Authorization", "X-Testing-ClientCert" };
        private ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<Program> _factory;

        public WebApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            lock(_lck)
            {
                _factory = new CustomWebApplicationFactory<Program>(scenarioContext);
                var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
                _scenarioContext.Set(_factory, "Factory");
                var mock = new Mock<Infrastructures.IHttpClientFactory>();
                mock.Setup(m => m.GetHttpClient()).Returns(client);
            }
        }

        [Given("build JWS by signing with the key '(.*)' coming from the client '(.*)' and store the result into '(.*)'")]
        public void GivenBuildJwsByUsingClientJwk(string keyid, string clientId, string key, Table table)
        {
            BuildJwsByUsingClientJwk(keyid, clientId, key, table);
        }

        [Given("build expired JWS by signing with the key '(.*)' coming from the client '(.*)' and store the result into '(.*)'")]
        public void GivenBuildExpiredJwsByUsingClientJwk(string keyid, string clientId, string key, Table table)
        {
            BuildJwsByUsingClientJwk(keyid, clientId, key, table, true);
        }

        [Given("build JWE by encrypting the '(.*)' JWS with the client secret '(.*)' and store the result into '(.*)'")]
        public void GivenBuildJweUsingClientJws(string jwsKey, string clientSecret, string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret));
            var handler = new JsonWebTokenHandler();
            var jwe = handler.EncryptToken(ParseValue(_scenarioContext, jwsKey).ToString(), new EncryptingCredentials(securityKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256));
            _scenarioContext.Set(jwe, key);
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
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);

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
            url = ParseValue(_scenarioContext, url).ToString();
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
            url = ParseValue(_scenarioContext, url).ToString();
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
                var value = ParseValue(_scenarioContext, record["Value"]).ToString();
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

        private void BuildJwsByUsingClientJwk(string keyid, string clientId, string key, Table table, bool isExpired = false)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
                var client = clientRepository.Query().Include(c => c.SerializedJsonWebKeys).Single(c => c.ClientId == clientId);
                var jwk = client.JsonWebKeys.Single(j => j.KeyId == keyid);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Claims = new Dictionary<string, object>(),
                    SigningCredentials = new SigningCredentials(jwk, jwk.Alg)
                };
                if (isExpired) tokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(-2);
                foreach (var row in table.Rows)
                    tokenDescriptor.Claims.Add(row["Key"].ToString(), row["Value"].ToString());
                var handler = new JsonWebTokenHandler();
                var jws = handler.CreateToken(tokenDescriptor);
                _scenarioContext.Set(jws, key);
            }
        }

        private string ExtractUrl(string url, Table table)
        {
            url = ParseValue(_scenarioContext, url).ToString();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(_scenarioContext, record["Value"]).ToString();
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
                var value = ParseValue(_scenarioContext, record["Value"]);
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
                var value = ParseValue(_scenarioContext, record["Value"]).ToString();
                if (!PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                result.Add(key, value.ToString());
            }

            return result;
        }

        public static object ParseValue(ScenarioContext scenarioContext, string val)
        {
            if (val.StartsWith('$') && val.EndsWith('$'))
            {
                val = val.TrimStart('$').TrimEnd('$');
                return scenarioContext.Get<object>(val);
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

                return scenarioContext.Get<string>(m.Value.TrimStart('$').TrimEnd('$'));
            });

            return result;
        }
    }
}