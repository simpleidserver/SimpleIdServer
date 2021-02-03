// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class SharedSteps
    {
        private ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<FakeStartup> _factory;

        public SharedSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = new CustomWebApplicationFactory<FakeStartup>((o) =>
            {
                o.AddSingleton(scenarioContext);
            });
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var mock = new Mock<Infrastructures.IHttpClientFactory>();
            mock.Setup(m => m.GetHttpClient()).Returns(client);
        }

        [When("add user consent : user='(.*)', scope='(.*)', clientId='(.*)'")]
        public async Task WhenAddUserConsent(string user, string scope, string clientId)
        {
            var repository = (IOAuthUserQueryRepository)_factory.Server.Host.Services.GetService(typeof(IOAuthUserQueryRepository));
            var oauthUser = await repository.FindOAuthUserByLogin(ParseValue(user).ToString(), CancellationToken.None);
            oauthUser.Consents.Add(new OAuthConsent
            {
                ClientId = ParseValue(clientId).ToString(),
                Scopes = scope.Split(' ').Select(s => new OAuthScope { Name = s })
            });
        }


        [When("execute HTTP GET request '(.*)'")]
        public async Task GivenExecuteHTTPGetRequest(string url, Table table)
        {
            string authHeader = null;

            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                if (value.StartsWith('$') && value.EndsWith('$'))
                {
                    value = value.TrimStart('$').TrimEnd('$');
                    value = _scenarioContext.Get<string>(value);
                }

                if (key == "Authorization")
                {
                    authHeader = value;
                }
                else
                {
                    url = QueryHelpers.AddQueryString(url, record["Key"], value);
                }
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                httpRequestMessage.Headers.Add("Authorization", $"Bearer {authHeader}");
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("add JSON web key to Authorization Server and store into '(.*)'")]
        public async Task WhenAddJsonWebKey(string name, Table table)
        {
            var repository = (IJsonWebKeyCommandRepository)_factory.Server.Host.Services.GetService(typeof(IJsonWebKeyCommandRepository));
            var jwks = ExtractJsonWebKeys(table);
            foreach (var jwk in jwks)
            {
                repository.Add(jwk);
            }

            await repository.SaveChanges(CancellationToken.None);
            _scenarioContext.Set(jwks, name);
        }

        [When("execute HTTP POST JSON request '(.*)'")]
        public async Task GivenExecuteHTTPPostJSONRequest(string url, Table table)
        {
            string authHeader = null;
            var jObj = new JObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                if (value.StartsWith('$') && value.EndsWith('$'))
                {
                    value = value.TrimStart('$').TrimEnd('$');
                    value = _scenarioContext.Get<string>(value);
                }

                if (key == "Authorization")
                {
                    authHeader = value;
                }
                else if (value.StartsWith('[') && value.EndsWith(']'))
                {
                    value = value.TrimStart('[').TrimEnd(']');
                    var splitted = value.Split(',');
                    jObj.Add(record["Key"], JArray.FromObject(splitted));
                }
                else
                {
                    jObj.Add(record["Key"], value);
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
                httpRequestMessage.Headers.Add("Authorization", $"Bearer {authHeader}");
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
                var value = record["Value"];
                if (value.StartsWith('$') && value.EndsWith('$'))
                {
                    value = value.TrimStart('$').TrimEnd('$');
                    value = _scenarioContext.Get<string>(value);
                }

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
                httpRequestMessage.Headers.Add("Authorization", $"Bearer {authHeader}");
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("extract JSON from body")]
        public async Task GivenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _scenarioContext.Set(JsonConvert.DeserializeObject<JObject>(json), "jsonHttpBody");
        }

        [When("extract query parameters into JSON")]
        public void WhenExtractQueryParametersIntoJSON()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queryValues = httpResponseMessage.RequestMessage.RequestUri.ParseQueryString();
            var jObj = new JObject();
            foreach (var key in queryValues.AllKeys)
            {
                jObj.Add(key, queryValues[key]);
            }

            _scenarioContext.Set(jObj, "jsonHttpBody");
        }
        
        [When("extract parameter '(.*)' from redirect url")]
        public void GivenExtractRedirectUrlParameter(string parameter)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queryValue = httpResponseMessage.RequestMessage.RequestUri.ParseQueryString()[parameter];
            _scenarioContext.Set(queryValue, parameter);
        }

        [When("extract parameter '(.*)' from JSON body")]
        public void GivenExtractParameterFromBody(string parameter)
        {
            var jObj = _scenarioContext.Get<JObject>("jsonHttpBody");
            _scenarioContext.Set(jObj[parameter].ToString(), parameter);
        }

        [When("build software statement")]
        public void WhenBuildSoftwareStatement(Table table)
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
            var jwk = FakeJwks.GetInstance().Jwks.First();
            var jws = jwtBuilder.Sign(jwsPayload, jwk, jwk.Alg);
            _scenarioContext.Set(jws, "softwareStatement");
        }

        [Then("JSON exists '(.*)'")]
        public void ThenExists(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.True(jsonHttpBody.ContainsKey(key));
        }

        [Then("JSON '(.*)'='(.*)'")]
        public void ThenEqualsTo(string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.Equal(value, jsonHttpBody[key].ToString());
        }

        [Then("HTTP status code equals to '(.*)'")]
        public void ThenCheckHttpStatusCode(int code)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.Equal(code, (int)httpResponseMessage.StatusCode);
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
                return JArray.FromObject(val.Split(','));
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


        private static IEnumerable<JsonWebKey> ExtractJsonWebKeys(Table table)
        {
            var builder = new JsonWebKeyBuilder();
            var jwks = new List<JsonWebKey>();
            foreach (var record in table.Rows)
            {
                var type = record["Type"];
                var kid = record["Kid"];
                var algName = record["AlgName"];
                JsonWebKey jwk = null;
                switch (type)
                {
                    case "SIG":
                        if (algName.StartsWith("ES"))
                        {
                            using (var ec = new ECDsaCng())
                            {
                                jwk = builder.NewSign(kid, new[]
                                {
                                    KeyOperations.Sign,
                                    KeyOperations.Verify
                                }).SetAlg(ec, algName).Build();
                            }
                        }
                        else if (algName.StartsWith("HS"))
                        {
                            using (var hmac = new HMACSHA256())
                            {
                                jwk = builder.NewSign(kid, new[]
                                {
                                    KeyOperations.Sign,
                                    KeyOperations.Verify
                                }).SetAlg(hmac, algName).Build();
                            }
                        }
                        else
                        {
                            using (var rsa = RSA.Create())
                            {
                                jwk = builder.NewSign(kid, new[]
                                {
                                    KeyOperations.Sign,
                                    KeyOperations.Verify
                                }).SetAlg(rsa, algName).Build();
                            }
                        }
                        break;
                    case "ENC":
                        using (var rsa = RSA.Create())
                        {
                            jwk = builder.NewEnc(kid, new[]
                            {
                                    KeyOperations.Encrypt,
                                    KeyOperations.Decrypt
                                }).SetAlg(rsa, algName).Build();
                        }
                        break;
                }

                jwks.Add(jwk);
            }

            return jwks;
        }
    }
}