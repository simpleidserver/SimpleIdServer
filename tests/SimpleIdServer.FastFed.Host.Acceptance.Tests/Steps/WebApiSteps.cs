// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.FastFed.Host.Acceptance.Tests;

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
            _scenarioContext.Set(client, "Client");
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

    [When("execute HTTP POST request '(.*)', content-type '(.*)', content '(.*)'")]
    public async Task WhenExecuteHttpPostWithContentType(string url, string contentType, string content)
    {
        content = ParseValue(_scenarioContext, content).ToString(); ;
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(content, Encoding.UTF8, contentType)
        };

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

    [When("extract header '(.*)' to '(.*)'")]
    public void WhenExtractHTTPHeader(string parameter, string name)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        var value = httpResponseMessage.Headers.First(c => c.Key == parameter).Value.First();
        _scenarioContext.Set(value, name);
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