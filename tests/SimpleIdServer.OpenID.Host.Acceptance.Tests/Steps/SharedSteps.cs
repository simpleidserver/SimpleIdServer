using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TechTalk.SpecFlow;
using SimpleIdServer.OAuth.Extensions;
using Xunit;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class SharedSteps
    {
        private static IEnumerable<string> PARAMETERS_IN_HEADER = new[] { "Authorization", "X-Testing-ClientCert" };
        private ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<FakeStartup> _factory;

        public SharedSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = new CustomWebApplicationFactory<FakeStartup>((o, f) =>
            {
                o.AddSingleton(scenarioContext);
            });
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var mock = new Mock<OAuth.Infrastructures.IHttpClientFactory>();
            mock.Setup(m => m.GetHttpClient()).Returns(client);
        }

        [When("add '(.*)' seconds and store the result into '(.*)'")]
        public void WhenBuildDateTime(int nbSeconds, string name)
        {
            var currentDateTime = DateTime.UtcNow.AddSeconds(nbSeconds);
            _scenarioContext.Set(currentDateTime.ConvertToUnixTimestamp(), name);
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

        [When("add authentication class references")]
        public void WhenAddAcrValues(Table table)
        {
            var repository = (IAuthenticationContextClassReferenceCommandRepository)_factory.Server.Host.Services.GetService(typeof(IAuthenticationContextClassReferenceCommandRepository));
            foreach (var record in table.Rows)
            {
                repository.Add(new AuthenticationContextClassReference
                {
                    AuthenticationMethodReferences = record["Amrs"].Split(','),
                    DisplayName = record["DisplayName"],
                    Name = record["Name"]
                });
            }
        }

        [When("add user consent with claim : user='(.*)', scope='(.*)', clientId='(.*)', claim='(.*)'")]
        public async Task WhenAddUserConsent(string user, string scope, string clientId, string claim)
        {
            var repository = (IOAuthUserQueryRepository)_factory.Server.Host.Services.GetService(typeof(IOAuthUserQueryRepository));
            var oauthUser = await repository.FindOAuthUserByLogin(ParseValue(user).ToString(), CancellationToken.None);
            var claims = claim.Split(' ').Select(s =>
            {
                var splitted = s.Split('=');
                return splitted.First();
            });
            oauthUser.Consents.Add(new OAuthConsent
            {
                ClientId = ParseValue(clientId).ToString(),
                Scopes = scope.Split(' ').Select(s => new OAuthScope { Name = s }),
                Claims = claims
            });
        }

        [When("anonymous authentication")]
        public void WhenAnonymousAuthentication()
        {
            _scenarioContext.Set("anonymous", "anonymous");
        }

        [When("execute HTTP GET request '(.*)'")]
        public async Task WhenExecuteHTTPGetRequest(string url, Table table)
        {
            url = ParseValue(url).ToString();
            var headers = ExtractHeaders(table);
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]);
                if (PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                url = QueryHelpers.AddQueryString(url, record["Key"], value.ToString());
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            foreach (var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP POST request '(.*)'")]
        public async Task WhenExecuteHTTPPostRequest(string url, Table table)
        {
            var form = new List<KeyValuePair<string, string>>();
            var headers = ExtractHeaders(table);
            foreach(var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]);
                if (PARAMETERS_IN_HEADER.Contains(key))
                {
                    continue;
                }

                form.Add(new KeyValuePair<string, string>(key, value.ToString()));
            }

            var body = new FormUrlEncodedContent(form);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = body
            };
            foreach (var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("poll HTTP POST request '(.*)', until '(.*)'='(.*)'")]
        public async Task WhenPollHTTPPostRequest(string url, string key, string value, Table table)
        {
            var form = new List<KeyValuePair<string, string>>();
            var headers = ExtractHeaders(table);
            foreach (var record in table.Rows)
            {
                var k = record["Key"];
                var v = ParseValue(record["Value"]);
                if (PARAMETERS_IN_HEADER.Contains(k))
                {
                    continue;
                }

                form.Add(new KeyValuePair<string, string>(k, v.ToString()));
            }

            var body = new FormUrlEncodedContent(form);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = body
            };
            foreach (var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            var jObj = JsonConvert.DeserializeObject<JObject>(json);
            var token = jObj.SelectToken(key);
            if (token == null || token.ToString() != value)
            {
                Thread.Sleep(200);
                await WhenPollHTTPPostRequest(url, key, value, table);
                return;
            }

            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("execute HTTP POST JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPostJSONRequest(string url, Table table)
        {
            var jObj = new JObject();
            var headers = ExtractHeaders(table);
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]);
                jObj.Add(key, JToken.FromObject(value));
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            foreach (var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("poll HTTP POST JSON request '(.*)', until HTTP status code ='(.*)'")]
        public async Task WhenPollHTTPPostRequest(string url, int statusCode, Table table)
        {
            var jObj = new JObject();
            var headers = ExtractHeaders(table);
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = ParseValue(record["Value"]);
                jObj.Add(key, JToken.FromObject(value));
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            foreach (var kvp in headers)
            {
                httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            if ((int)httpResponseMessage.StatusCode != statusCode)
            {
                Thread.Sleep(200);
                await WhenPollHTTPPostRequest(url, statusCode, table);
                return;
            }

            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("build JSON Web Keys, store JWKS into '(.*)' and store the public keys into '(.*)'")]
        public void WhenBuildJwks(string jwksName, string publicKeys, Table table)
        {
            var keys = new JArray();
            var jwks = ExtractJsonWebKeys(table);
            foreach (var jsonWebKey in jwks)
            {
                var key = new JObject();
                keys.Add(jsonWebKey.GetPublicJwt());
            }

            var result = new JObject
            {
                { "keys", keys }
            };

            _scenarioContext.Set(jwks, jwksName);
            _scenarioContext.Set(result, publicKeys);
        }

        [When("add JSON web key to Authorization Server and store into '(.*)'")]
        public void WhenAddJsonWebKey(string name, Table table)
        {
            var repository = (IJsonWebKeyCommandRepository)_factory.Server.Host.Services.GetService(typeof(IJsonWebKeyCommandRepository));
            var jwks = ExtractJsonWebKeys(table);
            foreach(var jwk in jwks)
            {
                repository.Add(jwk);
            }

            _scenarioContext.Set(jwks, name);   
        }

        [When("use '(.*)' JWK from '(.*)' to build JWS and store into '(.*)'")]
        public void WhenBuildJWS(string kid, string jwksName, string name, Table table)
        {
            var jwks = _scenarioContext.Get<List<JsonWebKey>>(jwksName);
            kid = ParseValue(kid).ToString();
            name = ParseValue(name).ToString();
            var jwk = jwks.First(j => j.Kid == kid);
            var jwsPayload = new JwsPayload();
            foreach(var row in table.Rows)
            {
                var key = row["Key"];
                var value = ParseValue(row["Value"]);
                jwsPayload.Add(key, value);
            }
            
            var jwtBuilder = (IJwtBuilder)_factory.Server.Host.Services.GetService(typeof(IJwtBuilder));
            var jws = jwtBuilder.Sign(jwsPayload, jwk, jwk.Alg);
            _scenarioContext.Set(jws, name);
            var tokenCommandRepository = (ITokenCommandRepository)_factory.Server.Host.Services.GetService(typeof(ITokenCommandRepository));
            tokenCommandRepository.Add(new Token
            {
                Id = jws,
                CreateDateTime = DateTime.UtcNow,
                TokenType = "access_token"
            }, CancellationToken.None).Wait();
        }

        [When("use '(.*)' JWKS from '(.*)' to encrypt '(.*)' and enc '(.*)' and store the result into '(.*)'")]
        public void WhenBuildJwe(string kid, string jwksName, string jws, string encAlg, string name)
        {
            kid = ParseValue(kid).ToString();
            var jwks = _scenarioContext.Get<List<JsonWebKey>>(jwksName);
            jws = ParseValue(jws).ToString();
            var jwtBuilder = (IJwtBuilder)_factory.Server.Host.Services.GetService(typeof(IJwtBuilder));
            var jwe = jwtBuilder.Encrypt(jws, encAlg, jwks.First(j => j.Kid == kid));
            _scenarioContext.Set(jwe, name);
        }

        [When("client '(.*)' build JWS token using the algorithm '(.*)'")]
        public async Task WhenBuildJwsToken(string clientId, string algName, Table table)
        {
            clientId = ParseValue(clientId).ToString();
            var jwsPayload = new JwsPayload();
            foreach (var record in table.Rows)
            {
                var value = ParseValue(record["Value"]);

                jwsPayload.Add(record["Key"].ToString(), value);
            }

            var jwtBuilder = (IJwtBuilder)_factory.Server.Host.Services.GetService(typeof(IJwtBuilder));
            var clientRepository = (IOAuthClientQueryRepository)_factory.Server.Host.Services.GetService(typeof(IOAuthClientQueryRepository));
            var oauthClient = await clientRepository.FindOAuthClientById(clientId, CancellationToken.None);
            var jsonWebKey = oauthClient.JsonWebKeys.First(f => f.Use == Usages.SIG && f.Alg == algName);
            var requestParameter = jwtBuilder.Sign(jwsPayload, jsonWebKey, jsonWebKey.Alg);
            _scenarioContext.Set(requestParameter, "requestParameter");
        }

        [When("extract parameter '(.*)' from redirect url")]
        public void WhenExtractRedirectUrlParameter(string parameter)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queryValue = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query)[parameter];
            _scenarioContext.Set(queryValue.First(), parameter);
        }

        [When("extract JSON from body")]
        public async Task WhenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _scenarioContext.Set(JsonConvert.DeserializeObject<JObject>(json), "jsonHttpBody");
        }

        [When("extract query parameters into JSON")]
        public void WhenExtractQueryParametersIntoJSON()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queryValues = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
            var jObj = new JObject();
            foreach(var kvp in queryValues)
            {
                jObj.Add(kvp.Key, kvp.Value.First());
            }

            _scenarioContext.Set(jObj, "jsonHttpBody");
        }

        [When("extract parameter '(.*)' from JSON body")]
        public void GivenExtractParameterFromBody(string parameter)
        {
            var jObj = _scenarioContext.Get<JObject>("jsonHttpBody");
            _scenarioContext.Set(jObj[parameter].ToString(), parameter);
        }

        [When("execute HTTP request")]
        public async Task WhenExecuteHttpRequest()
        {
            var url = _scenarioContext["url"] as string;
            if (_scenarioContext.ContainsKey("queries"))
            {
                var queries = _scenarioContext["queries"] as IDictionary<string, string>;
                url = QueryHelpers.AddQueryString(url, queries);
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            if (_scenarioContext.ContainsKey("httpHeaders"))
            {
                var httpHeaders = _scenarioContext["httpHeaders"] as List<KeyValuePair<string, string>>;
                foreach (var kvp in httpHeaders)
                {
                    httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }

        [When("add '(.*)' seconds to authentication instant to user '(.*)'")]
        public async Task WhenSubstractAuthenticationInstant(int seconds, string login)
        {
            var oauthUserRepository = (IOAuthUserQueryRepository)_factory.Server.Host.Services.GetService(typeof(IOAuthUserQueryRepository));
            var user = await oauthUserRepository.FindOAuthUserByLogin(login, CancellationToken.None);
            user.GetActiveSession().AuthenticationDateTime = DateTime.UtcNow.AddSeconds(seconds);
        }

        [When("extract string from body")]
        public async Task WhenExtractStringFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var strHttpBody = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _scenarioContext.Set(strHttpBody, "strHttpBody");
        }

        [When("extract '(.*)' from callback")]
        public void WhenExtractTokenFromCallback(string paramName)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var paramValue = HttpUtility.ParseQueryString(httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri).Get(paramName);
            _scenarioContext.Set(paramValue, paramName);
        }

        [When("decrypt '(.*)' JWE into '(.*)'")]
        public void WhenExtractJWEPayload(string token, string name)
        {
            var jwtParser = (IJwtParser)_factory.Server.Host.Services.GetService(typeof(IJwtParser));
            var jwe = ParseValue(token).ToString();
            var jwePayload = jwtParser.Decrypt(jwe);
            var jweHeader = jwtParser.ExtractJweHeader(jwe);
            _scenarioContext.Set(jwePayload, name);
            _scenarioContext.Set(jweHeader, "jweHeader");
        }

        [When("use '(.*)' JWKS to decrypt '(.*)' JWE into '(.*)'")]
        public void WhenUseJwkToExtractJWEPayload(string jwksName, string token, string name)
        {
            var jwtParser = (IJwtParser)_factory.Server.Host.Services.GetService(typeof(IJwtParser));
            var jwks = _scenarioContext.Get<List<JsonWebKey>>(jwksName);
            var jwe = ParseValue(token).ToString();
            var jweHeader = jwtParser.ExtractJweHeader(jwe);
            var jwePayload = jwtParser.Decrypt(jwe, jwks.First(j => j.Kid == jweHeader.Kid));
            _scenarioContext.Set(jwePayload, name);
            _scenarioContext.Set(jweHeader, "jweHeader");
        }

        [When("extract payload from JWS '(.*)'")]
        public void WhenExtractJwsPayloadFromAuthorizationRequest(string name)
        {
            var jws = ParseValue(name).ToString();
            var jwsGenerator = new JwsGeneratorFactory().BuildJwsGenerator();
            _scenarioContext.Set(jwsGenerator.ExtractPayload(jws), "tokenPayload");
            _scenarioContext.Set(jwsGenerator.ExtractHeader(jws), "jwsHeader");
        }

        [When("confirm authorization request '(.*)'")]
        public async Task WhenConfirmAuthorizationRequest(string authRequestId)
        {
            authRequestId = ParseValue(authRequestId).ToString();
            var bcAuthorizeRepository = (IBCAuthorizeRepository)_factory.Server.Host.Services.GetService(typeof(IBCAuthorizeRepository));
            var auth = await bcAuthorizeRepository.Get(authRequestId, CancellationToken.None);
            auth.Confirm(new string[0]);
            await bcAuthorizeRepository.Update(auth, CancellationToken.None);
            await bcAuthorizeRepository.SaveChanges(CancellationToken.None);
        }

        [When("poll until '(.*)' is received")]
        public void WhenPollUntilResultIsReceived(string key)
        {
            if (!_scenarioContext.ContainsKey(key))
            {
                Thread.Sleep(200);
                WhenPollUntilResultIsReceived(key);
                return;
            }
        }

        [When("extract JSON from '(.*)'")]
        public void WhenExtractJSONFromKey(string key)
        {
            _scenarioContext.Set(JsonConvert.DeserializeObject<JObject>(_scenarioContext.Get<string>(key)), "jsonHttpBody");
        }

        [Then("HTTP status code equals to '(.*)'")]
        public void ThenCheckHttpStatusCode(int code)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.Equal(code, (int)httpResponseMessage.StatusCode);
        }

        [Then("HTTP header '(.*)' contains '(.*)'")]
        public void ThenContentType(string key, string value)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.True(httpResponseMessage.Content.Headers.Contains(key));
            Assert.True(httpResponseMessage.Content.Headers.GetValues(key).Contains(value) == true);
        }

        [Then("JSON contains '(.*)'")]
        public void ThenExists(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.True(jsonHttpBody.ContainsKey(key) == true);
        }

        [Then("'(.*)'='(.*)'")]
        public void ThenKeyEqualsTo(string key, string value)
        {
            var currentValue = ParseValue(key).ToString();
            Assert.Equal(value, currentValue);
        }

        [Then("JSON '(.*)'='(.*)'")]
        public void ThenEqualsTo(string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.Equal(ParseValue(value).ToString(), jsonHttpBody.SelectToken(key).ToString());
        }

        [Then("JSON '(.*)'=(.*)")]
        public void ThenEqualsTo(string key, int value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.Equal(value, jsonHttpBody.SelectToken(key));
        }

        [Then("JSON '(.*)'=(.*)")]
        public void ThenEqualsTo(string key, bool value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.Equal(value, jsonHttpBody.SelectToken(key));
        }

        [Then("token contains '(.*)'")]
        public void ThenIdentityTokenContains(string key)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JwsPayload;
            Assert.True(tokenPayload.ContainsKey(key));
        }

        [Then("JWS Alg equals to '(.*)'")]
        public void ThenJwsAlgEqualsTo(string value)
        {
            var tokenHeader = _scenarioContext["jwsHeader"] as JwsHeader;
            Assert.Equal(value, tokenHeader.Alg);
        }

        [Then("JWS Kid equals to '(.*)'")]
        public void ThenJwsKidEqualsTo(string value)
        {
            var tokenHeader = _scenarioContext["jwsHeader"] as JwsHeader;
            Assert.Equal(value, tokenHeader.Kid);
        }

        [Then("JWE Alg equals to '(.*)'")]
        public void ThenJweAlgEqualsTo(string value)
        {
            var tokenHeader = _scenarioContext["jweHeader"] as JweHeader;
            Assert.Equal(value, tokenHeader.Alg);
        }

        [Then("JWE Enc equals to '(.*)'")]
        public void ThenJweEncEqualsTo(string value)
        {
            var tokenHeader = _scenarioContext["jweHeader"] as JweHeader;
            Assert.Equal(value, tokenHeader.Enc);
        }

        [Then("token claim doesn't contain '(.*)'")]
        public void ThenIdentityTokenContainsClaim(string key)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JwsPayload;
            Assert.False(tokenPayload.ContainsKey(key));
        }

        [Then("token claim '(.*)'='(.*)'")]
        public void ThenIdentityTokenContainsClaim(string key, string value)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JwsPayload;
            Assert.True(tokenPayload.ContainsKey(key));
            Assert.Equal(ParseValue(value).ToString(), tokenPayload[key].ToString());
        }

        [Then("token claim '(.*)' contains '(.*)'")]
        public void ThenIdentityTokenClaimContains(string key, string value)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JwsPayload;
            Assert.True(tokenPayload.ContainsKey(key));
            var values = tokenPayload.GetArrayClaim(key);
            Assert.True(values.Contains(value) == true);
        }

        [Then("token claim '(.*)'!='(.*)'")]
        public void ThenIdentityTokenDoesntContainClaim(string key, string value)
        {
            var tokenPayload = _scenarioContext["tokenPayload"] as JwsPayload;
            Assert.True(tokenPayload.ContainsKey(key));
            Assert.NotEqual(value, tokenPayload[key].ToString());
        }

        [Then("redirect url contains '(.*)'")]
        public void ThenRedirectUriContains(string url)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.True(httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.Contains(url) == true);
        }

        private Dictionary<string, string> ExtractHeaders(Table table)
        {
            var result = new Dictionary<string, string>();
            foreach (var record in table.Rows)
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
                return JArray.FromObject(val.Split(','));
            }

            if (val.StartsWith('{') && val.EndsWith('}'))
            {
                return JObject.Parse(val);
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