using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Jwt;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.Uma.Host.Acceptance.Tests
{
    [Binding]
    public class SharedSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<FakeStartup> _factory;

        public SharedSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = new CustomWebApplicationFactory<FakeStartup>((o) =>
            {
                o.AddSingleton(scenarioContext);
            });
        }

        [When("execute HTTP POST JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPostJSONRequest(string url, Table table)
        {
            var jObj = new JObject();
            string authHeader = null;
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                if (key == "Authorization")
                {
                    authHeader = Parse(value);
                }
                else
                {
                    try
                    {
                        jObj.Add(key, JToken.Parse(value));
                    }
                    catch
                    {
                        jObj.Add(key, Parse(value.ToString()));
                    }
                }
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                httpRequestMessage.Headers.Add("Authorization", authHeader);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PUT JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPutJSONRequest(string url, Table table)
        {
            string authHeader = null;
            var jObj = new JObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                if (key == "Authorization")
                {
                    authHeader = Parse(value);
                }
                else
                {
                    try
                    {
                        jObj.Add(key, JToken.Parse(value));
                    }
                    catch
                    {
                        jObj.Add(key, value.ToString());
                    }
                }
            }

            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                httpRequestMessage.Headers.Add("Authorization", authHeader);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP POST request '(.*)'")]
        public async Task GivenExecuteHTTPPostRequest(string url, Table table)
        {
            string authHeader = null;
            var jObj = new List<KeyValuePair<string, string>>();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = Parse(record["Value"]);
                if (key == "Authorization")
                {
                    authHeader = value;
                }
                else
                {
                    jObj.Add(new KeyValuePair<string, string>(record["Key"], value));
                }
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new FormUrlEncodedContent(jObj)
            };
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                httpRequestMessage.Headers.Add("Authorization", authHeader);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP PATCH JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPatchJSONRequest(string url, Table table)
        {
            var jObj = new JObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                try
                {
                    jObj.Add(key, JToken.Parse(value));
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

        [When("execute HTTP GET against '(.*)' and pass authorization header '(.*)'")]
        public async Task WhenExecuteHTTPGETRequestWithAuthorizationHeader(string url, string authHeader)
        {
            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            httpRequestMessage.Headers.Add("Authorization", Parse(authHeader));
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

        [When("execute HTTP DELETE against '(.*)' and pass authorization header '(.*)'")]
        public async Task WhenExecuteHTTPDeleteRequestWithAuthorizationHeader(string url, string authHeader)
        {
            url = Parse(url);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url)
            };
            httpRequestMessage.Headers.Add("Authorization", Parse(authHeader));
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("build claim_token")]
        public void WhenBuildClaimToken(Table table)
        {
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
            var jws = jwtBuilder.Sign(jwsPayload, JwksStore.GetInstance().GetJsonWebKey());
            _scenarioContext.Set(jws, "claim_token");
        }

        [When("extract payload from JWS '(.*)'")]
        public void WhenExtractJwsPayloadFromAuthorizationRequest(string name)
        {
            var jws = Parse(name).ToString();
            var jwsGenerator = new JwsGeneratorFactory().BuildJwsGenerator();
            _scenarioContext.Set(JObject.Parse(JsonConvert.SerializeObject(jwsGenerator.ExtractPayload(jws))), "tokenPayload");
            _scenarioContext.Set(jwsGenerator.ExtractHeader(jws), "jwsHeader");
        }

        [When("extract JSON from body")]
        public async Task GivenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _scenarioContext.Set(JsonConvert.DeserializeObject<JObject>(json), "jsonHttpBody");
        }

        [When("extract '(.*)' from JSON body")]
        public void WhenExtractJSONKeyFromBody(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            var val = jsonHttpBody.SelectToken(key);
            if (val != null)
            {
                _scenarioContext.Set(val.ToString(), key);
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
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            var token = jsonHttpBody.SelectToken(key);
            Assert.NotNull(token);
        }

        [Then("JSON doesn't exists '(.*)'")]
        public void TheDoesntExist(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            var token = jsonHttpBody.SelectToken(key);
            Assert.Null(token);
        }

        [Then("JSON '(.*)'='(.*)'")]
        public void ThenJSONEqualsTo(string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.Equal(Parse(value).ToLowerInvariant(), jsonHttpBody.SelectToken(key).ToString().ToLowerInvariant());
        }

        [Then("HTTP HEADER contains '(.*)'")]
        public void ThenHTTPHeaderContains(string key)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.True(httpResponseMessage.Headers.Contains(key));
        }

        [Then("token contains '(.*)'")]
        public void ThenIdentityTokenContains(string key)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JObject;
            Assert.True(tokenPayload.SelectToken(key) != null);
        }

        [Then("token claim '(.*)'='(.*)'")]
        public void ThenIdentityTokenContainsClaim(string key, string value)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JObject;
            Assert.Equal(Parse(value).ToString(), tokenPayload.SelectToken(key).ToString().ToLowerInvariant());
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