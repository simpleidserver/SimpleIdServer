﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Persistences;
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

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class WebApiSteps
    {
        private static IEnumerable<string> PARAMETERS_IN_HEADER = new[] { "x-fapi-interaction-id", "Authorization", "X-Testing-ClientCert" };
        private ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<FakeStartup> _factory;

        public WebApiSteps(ScenarioContext scenarioContext)
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
            var mock = new Mock<SimpleIdServer.OAuth.Infrastructures.IHttpClientFactory>();
            mock.Setup(m => m.GetHttpClient()).Returns(client);
        }

        public static object ParseValue(string val, ScenarioContext scenarioContext)
        {
            if (val == "$tomorrow$")
            {
                return DateTime.UtcNow.AddDays(1).ConvertToUnixTimestamp().ToString();
            }

            if (val.StartsWith('$') && val.EndsWith('$'))
            {
                val = val.TrimStart('$').TrimEnd('$');
                return scenarioContext.Get<object>(val);
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

                return scenarioContext.Get<string>(m.Value.TrimStart('$').TrimEnd('$'));
            });

            return result;
        }

        [When("build JSON Web Keys, store JWKS into '(.*)' and store the public keys into '(.*)'")]
        public void WhenBuildJwks(string jwksName, string publicKeys, Table table)
        {
            var keys = new JArray();
            var jwks = ExtractJsonWebKeys(table);
            foreach (var jsonWebKey in jwks)
            {
                keys.Add(jsonWebKey.GetPublicJwt());
            }

            var result = new JObject
            {
                { "keys", keys }
            };

            _scenarioContext.Set(jwks, jwksName);
            _scenarioContext.Set(result, publicKeys);
        }

        [When("use '(.*)' JWK from '(.*)' to build JWS and store into '(.*)'")]
        public void WhenBuildJWS(string kid, string jwksName, string name, Table table)
        {
            var jwks = _scenarioContext.Get<List<JsonWebKey>>(jwksName);
            kid = ParseValue(kid).ToString();
            name = ParseValue(name).ToString();
            var jwk = jwks.First(j => j.Kid == kid);
            var jwsPayload = new JwsPayload();
            foreach (var row in table.Rows)
            {
                var key = row["Key"];
                var value = ParseValue(row["Value"]);
                jwsPayload.Add(key, value);
            }

            var jwtBuilder = (IJwtBuilder)_factory.Server.Host.Services.GetService(typeof(IJwtBuilder));
            var jws = jwtBuilder.Sign(jwsPayload, jwk, jwk.Alg);
            _scenarioContext.Set(jws, name);
            var tokenCommandRepository = (ITokenRepository)_factory.Server.Host.Services.GetService(typeof(ITokenRepository));
            tokenCommandRepository.Add(new Token
            {
                Id = jws,
                CreateDateTime = DateTime.UtcNow,
                TokenType = "access_token"
            }, CancellationToken.None).Wait();
        }

        [When("add user consent : user='(.*)', scope='(.*)', clientId='(.*)'")]
        public async Task WhenAddUserConsent(string user, string scope, string clientId)
        {
            var repository = (IOAuthUserRepository)_factory.Server.Host.Services.GetService(typeof(IOAuthUserRepository));
            var oauthUser = await repository.FindOAuthUserByLogin(ParseValue(user).ToString(), CancellationToken.None);
            oauthUser.Consents.Add(new OAuthConsent
            {
                ClientId = ParseValue(clientId).ToString(),
                Scopes = scope.Split(' ').Select(s => new OAuthScope { Name = s })
            });
        }

        [When("add JSON web key to Authorization Server and store into '(.*)'")]
        public async Task WhenAddJsonWebKey(string name, Table table)
        {
            var repository = (IJsonWebKeyRepository)_factory.Server.Host.Services.GetService(typeof(IJsonWebKeyRepository));
            var jwks = ExtractJsonWebKeys(table);
            foreach (var jwk in jwks)
            {
                repository.Add(jwk, CancellationToken.None).Wait();
            }

            await repository.SaveChanges(CancellationToken.None);
            _scenarioContext.Set(jwks, name);
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

        [When("add authorized Account Access Consent '(.*)'")]
        public async Task GivenAddAuthorizedAccountAccessConsent(string clientId)
        {
            clientId = ParseValue(clientId).ToString();
            var accountAccessConsentRepository = _factory.Server.Host.Services.GetService(typeof(IAccountAccessConsentRepository)) as IAccountAccessConsentRepository;
            var accountAccessConsent = AccountAccessConsentAggregate.Create(clientId, new List<string>
            {
                AccountAccessConsentPermission.ReadAccountsBasic.Name
            }, null, null, null, null);
            accountAccessConsent.Confirm(new List<string> { });
            await accountAccessConsentRepository.Add(accountAccessConsent, CancellationToken.None);
            _scenarioContext.Set(accountAccessConsent.AggregateId, "consentId");
        }

        [When("add rejected Account Access Consent '(.*)'")]
        public async Task GivenAddRejectedAccountAccessConsent(string clientId)
        {
            clientId = ParseValue(clientId).ToString();
            var accountAccessConsentRepository = _factory.Server.Host.Services.GetService(typeof(IAccountAccessConsentRepository)) as IAccountAccessConsentRepository;
            var accountAccessConsent = AccountAccessConsentAggregate.Create(clientId, new List<string>
            {
                AccountAccessConsentPermission.ReadAccountsBasic.Name
            }, null, null, null, null);
            accountAccessConsent.Reject();
            await accountAccessConsentRepository.Add(accountAccessConsent, CancellationToken.None);
            _scenarioContext.Set(accountAccessConsent.AggregateId, "consentId");
        }

        [When("'(.*)' confirm consent '(.*)' for accounts '(.*)', with scopes '(.*)'")]
        public async Task GivenConfirmConsent(string login, string consentId, string accountId, string scopes)
        {
            consentId = ParseValue(consentId).ToString();
            var accountAccessConsentRepository = _factory.Server.Host.Services.GetService(typeof(IAccountAccessConsentRepository)) as IAccountAccessConsentRepository;
            var oauthUserRepository = _factory.Server.Host.Services.GetService(typeof(IOAuthUserRepository)) as IOAuthUserRepository;
            var oauthUser = await oauthUserRepository.FindOAuthUserByLogin(login, CancellationToken.None);
            oauthUser.Consents.Add(new OAuthConsent
            {
                Scopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "accounts"
                    }
                }
            });
            await oauthUserRepository.Update(oauthUser, CancellationToken.None);
            var consent = await accountAccessConsentRepository.Get(consentId, CancellationToken.None);
            consent.Confirm(accountId.Split(','));
            await accountAccessConsentRepository.Update(consent, CancellationToken.None);
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

        private JObject ExtractBody(Table table)
        {
            var result = new JObject();
            foreach(var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]);
                if (PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                if (value is JArray)
                {
                    result.Add(key, value as JArray);
                }
                else
                {
                    try
                    {
                        result.Add(key, JObject.Parse(value.ToString()));
                    }
                    catch
                    {
                        result.Add(key, value.ToString());
                    }
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
            return ParseValue(val, _scenarioContext);
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