// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Extensions;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class SharedSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<Program> _factory;

        public SharedSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = new CustomWebApplicationFactory<Program>((o) =>
            {
                o.AddSingleton(scenarioContext);
            });
        }

        [When("execute HTTP POST JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPostJSONRequest(string url, Table table)
        {
            var jObj = new JsonObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = Parse(record["Value"]);
                try
                {
                    jObj.Add(key, JsonNode.Parse(value));
                }
                catch
                {
                    jObj.Add(key, value.ToString());


                }
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP POST JSON request '(.*)' with body '(.*)'")]
        public async Task WhenExecuteHTTPPostJSONRequestWithBody(string url, string body)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PUT JSON request '(.*)' with body '(.*)'")]
        public async Task WhenExecuteHTTPPutJSONRequestWithBody(string url, string body)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PATCH JSON request '(.*)' with body '(.*)'")]
        public async Task WhenExecuteHTTPPatchJSONRequestWithBody(string url, string body)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PUT JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPutJSONRequest(string url, Table table)
        {
            var jObj = new JsonObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = Parse(record["Value"]);
                try
                {
                    jObj.Add(key, JsonNode.Parse(value));
                }
                catch
                {
                    jObj.Add(key, value.ToString());
                }
            }

            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PATCH JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPatchJSONRequest(string url, Table table)
        {
            var jObj = new JsonObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = Parse(record["Value"]);
                try
                {
                    jObj.Add(key, JsonNode.Parse(value));
                }
                catch
                {
                    jObj.Add(key, value.ToString());
                }
            }

            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP GET request '(.*)'")]
        public async Task WhenExecuteHTTPGETRequest(string url)
        {
            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP DELETE request '(.*)'")]
        public async Task WhenExecuteHTTPDELETERequest(string url)
        {
            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url)
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("extract JSON from body")]
        public async Task GivenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(json))
            {
                json = "{}";
            }

            _scenarioContext.Set(JsonSerializer.Deserialize<JsonNode>(json), "jsonHttpBody");
        }

        [When("extract '(.*)' from JSON body")]
        public void WhenExtractJSONKeyFromBody(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            var val = jsonHttpBody.SelectToken(key);
            if (val != null)
            {
                _scenarioContext.Set(val.ToString(), key);
            }
        }

        [When("extract '(.*)' from JSON body into '(.*)'")]
        public void WhenExtractJSONKeyFromBodyInto(string source, string target)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            var val = jsonHttpBody.SelectToken(source);
            if (val != null)
            {
                _scenarioContext.Set(val.ToString(), target);
            }
        }

        [Then("HTTP status code equals to '(.*)'")]
        public void ThenCheckHttpStatusCode(int code)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.Equal(code, (int)httpResponseMessage.StatusCode);
        }

        [Then("JSON exists '(.*)'")]
        public void ThenExists(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            var token = jsonHttpBody.SelectToken(key);
            Assert.NotNull(token);
        }

        [Then("JSON doesn't exists '(.*)'")]
        public void TheDoesntExist(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            var token = jsonHttpBody.SelectToken(key);
            Assert.Null(token);
        }

        [Then("JSON '(.*)'='(.*)'")]
        public void ThenJSONEqualsTo(string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            Assert.Equal(Parse(value).ToLowerInvariant(), jsonHttpBody.SelectToken(key).ToString().ToLowerInvariant());
        }

        [Then("JSON with namespace '(.*)' '(.*)'='(.*)'")]
        public void ThenJSONWithNamespaceEqualsTo(string ns, string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            var token = jsonHttpBody[ns].SelectToken(key);
            Assert.Equal(value.ToLowerInvariant(), token.ToString().ToLowerInvariant());
        }

        [Then("HTTP HEADER contains '(.*)'")]
        public void ThenHTTPHeaderContains(string key)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.True(httpResponseMessage.Headers.Contains(key));
        }

        [Then("'(.*)' length is equals to '(.*)'")]
        public void ThenLengthIsEqualsTo(string name, int length)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonNode;
            Assert.Equal((jsonHttpBody.SelectToken(name) as JsonArray).Count(), length);
        }

        private string Parse(string val)
        {
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