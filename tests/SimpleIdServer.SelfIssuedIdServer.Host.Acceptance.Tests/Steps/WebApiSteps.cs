// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.SelfIdServer.Host.Acceptance.Tests.Steps;

[Binding]
public class WebApiSteps
{
    private static object _lck = new object();
    private static IEnumerable<string> PARAMETERS_IN_HEADER = new[] { "Authorization", "X-Testing-ClientCert", "DPoP" };
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
            var mock = new Mock<SimpleIdServer.IdServer.Helpers.IHttpClientFactory>();
            mock.Setup(m => m.GetHttpClient()).Returns(client);
            _scenarioContext.Set(new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "sidClient.crt")), "sidClient.crt");
        }
    }

    [Given("build authorization request callback '(.*)'")]
    public async Task GivenCreateAuthorizationRequestCallback(string nonce)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var grantedTokenHelper = scope.ServiceProvider.GetRequiredService<IGrantedTokenHelper>();
            await grantedTokenHelper.AddAuthorizationRequestCallback(nonce, new System.Text.Json.Nodes.JsonObject
            {
                { AuthorizationRequestParameters.RedirectUri, "http://localhost" }
            }, 200, CancellationToken.None);
        }
    }

    [Given("build JWE by encrypting the '(.*)' JWS with the client secret '(.*)' and store the result into '(.*)'")]
    public void GivenBuildJweUsingClientJws(string jwsKey, string clientSecret, string key)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret));
        var handler = new JsonWebTokenHandler();
        var jwe = handler.EncryptToken(ParseValue(_scenarioContext, jwsKey).ToString(), new EncryptingCredentials(securityKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256));
        _scenarioContext.Set(jwe, key);
    }

    [Given("build JWS id_token_hint and sign with the key '(.*)'")]
    public void GivenBuildJWSIdTokenHint(string keyId, Table table)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var keyStore = scope.ServiceProvider.GetRequiredService<IKeyStore>();
            var signKey = keyStore.GetAllSigningKeys("master").First(k => k.Key.KeyId == keyId);
            var handler = new JsonWebTokenHandler();
            var claims = new Dictionary<string, object>();
            foreach (var row in table.Rows)
                claims.Add(row["Key"].ToString(), row["Value"].ToString());

            var descritor = new SecurityTokenDescriptor
            {
                Claims = claims,
                SigningCredentials = signKey
            };
            var request = handler.CreateToken(descritor);
            _scenarioContext.Set(request, "id_token_hint");
        }
    }

    [Given("build access_token and sign with the key '(.*)'")]
    public async Task GivenBuildAccessToken(string keyId, Table table)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var keyStore = scope.ServiceProvider.GetRequiredService<IKeyStore>();
            var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
            var transactionBuilder = scope.ServiceProvider.GetRequiredService<ITransactionBuilder>();
            using (var transaction = transactionBuilder.Build())
            {
                var signKey = keyStore.GetAllSigningKeys("master").First(k => k.Key.KeyId == keyId);
                var handler = new JsonWebTokenHandler();
                var claims = new Dictionary<string, object>();
                foreach (var row in table.Rows)
                    claims.Add(row["Key"].ToString(), row["Value"].ToString());
                var descritor = new SecurityTokenDescriptor
                {
                    Claims = claims,
                    SigningCredentials = signKey
                };
                var request = handler.CreateToken(descritor);
                var record = new IdServer.Domains.Token
                {
                    Id = request,
                    ClientId = "clientId",
                    TokenType = IdServer.DTOs.TokenResponseParameters.AccessToken,
                    ExpirationTime = DateTime.UtcNow.AddDays(2)
                };
                tokenRepository.Add(record);
                await transaction.Commit(CancellationToken.None);
                _scenarioContext.Set(request, "access_token");
            }
        }
    }

    [When("execute HTTP GET request '(.*)'")]
    public async Task WhenExecuteHTTPGetRequest(string url, Table table)
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
        httpRequestMessage.Headers.Add("Cookie", $"{CookieAuthenticationDefaults.CookiePrefix}Session=sessionId");
        var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
        _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
    }

    [When("execute HTTP PUT request '(.*)'")]
    public async Task WhenExecuteHTTPPutRequest(string url, Table table)
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
    public async Task WhenExecuteHTTPDeleteRequest(string url, Table table)
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
    public async Task WhenExecuteHTTPPutJSONRequest(string url, Table table)
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
    public async Task WhenExecuteHTTPPostJSONRequest(string url, Table table)
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
    public async Task WhenExecuteHTTPPostRequest(string url, Table table)
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

    [When("polls until notification is received")]
    public void WhenPollsUntilNotificationIsReceived()
    {
        if(!_scenarioContext.ContainsKey("notificationResponse"))
        {
            Thread.Sleep(200);
            WhenPollsUntilNotificationIsReceived();
        }
        else
        {
            _scenarioContext.Set(JsonDocument.Parse(_scenarioContext.Get<string>("notificationResponse")), "jsonHttpBody");
        }
    }

    [When("extract header '(.*)' to '(.*)'")]
    public void WhenExtractHTTPHeader(string parameter, string name)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        var value = httpResponseMessage.Headers.First(c => c.Key == parameter).Value.First();
        _scenarioContext.Set(value, name);
    }

    [When("build DPoP proof")]
    public void WhenBuildDPoPProof(Table table)
    {
        var dpopHandler = new DPoPHandler();
        var claims = new List<Claim>();
        foreach (var row in table.Rows)
        {
            var value = WebApiSteps.ParseValue(_scenarioContext, row["Value"].ToString());
            claims.Add(new Claim(row["Key"], value.ToString()));
        }

        if(!_scenarioContext.ContainsKey("securityKey"))
        {
            var dpopProof = dpopHandler.CreateES(claims);
            _scenarioContext.Set(dpopProof.Token, "DPOP");
            return;
        }

        var securityKey = _scenarioContext.Get<ECDsaSecurityKey>("securityKey");
        _scenarioContext.Set(dpopHandler.Create(claims, securityKey, SecurityAlgorithms.EcdsaSha256).Token, "DPOP");
    }

    [When("build DPoP proof and store into '(.*)'")]
    public void WhenBuildDPoPProofAndStore(string key, Table table)
    {
        var dpopHandler = new DPoPHandler();
        var claims = new List<Claim>();
        foreach (var row in table.Rows) claims.Add(new Claim(row["Key"], row["Value"]));
        var dpopProof = dpopHandler.CreateES(claims);
        _scenarioContext.Set(dpopProof.Token, key);
    }

    [When("build DPoP proof with big lifetime")]
    public void WhenBuildDPoPProofWithBigLifetime(Table table)
    {
        var dpopHandler = new DPoPHandler();
        var claims = new List<Claim>();
        foreach (var row in table.Rows) claims.Add(new Claim(row["Key"], row["Value"]));
        var dpopProof = dpopHandler.CreateRSA(claims, expiresInSeconds: 500);
        _scenarioContext.Set(dpopProof.Token, "DPOP");
    }

    [When("build security key")]
    public void WhenBuildSecurityKeyAndStore()
    {
        var curve = ECCurve.NamedCurves.nistP256;
        var securityKey = new ECDsaSecurityKey(ECDsa.Create(curve));
        var jkt = Base64UrlEncoder.Encode(securityKey.ComputeJwkThumbprint());
        _scenarioContext.Set(securityKey, "securityKey");
        _scenarioContext.Set(jkt, "jkt");
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
            object value = ParseValue(_scenarioContext, record["Value"], true);
            try
            {
                value = JsonNode.Parse(value.ToString());
            }
            catch { }

            if (PARAMETERS_IN_HEADER.Contains(key))
            {
                continue;
            }

            if (value is JsonNode)
            {
                result.Add(key, value as JsonNode);
            }
            else
            {
                result.Add(key, value.ToString());
            }
        }

        return result;
    }

    private List<KeyValuePair<string, string>> ExtractHeaders(Table table)
    {
        var result = new List<KeyValuePair<string, string>>();
        foreach(var record in table.Rows)
        {
            var key = record["Key"];
            var value = ParseValue(_scenarioContext, record["Value"]).ToString();
            if (!PARAMETERS_IN_HEADER.Contains(key)) continue;
            result.Add(new KeyValuePair<string, string>(key, value.ToString()));
        }

        return result;
    }

    public static object ParseValue(ScenarioContext scenarioContext, string val, bool ignoreArray = false)
    {
        if (val.StartsWith('$') && val.EndsWith('$'))
        {
            val = val.TrimStart('$').TrimEnd('$');
            return scenarioContext.Get<object>(val);
        }
        
        if (!ignoreArray && val.StartsWith('[') && val.EndsWith(']'))
        {
            val = val.TrimStart('[').TrimEnd(']');
            var res = new JsonArray();
            foreach (var item in val.Split(',')) res.Add(item.Trim(' ').Trim('"'));
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