// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests.Steps;

[Binding]
public class WebApiSteps
{
    private static IEnumerable<string> PARAMETERS_IN_HEADER = new[] { "Authorization" };
    private static object _lck = new object();
    private readonly ScenarioContext _scenarioContext;
    private CustomWebApplicationFactory<Program> _factory;

    public WebApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        lock (_lck)
        {
            _factory = new CustomWebApplicationFactory<Program>(scenarioContext);
        }
    }

    [When("execute HTTP GET request '(.*)'")]
    public async Task WhenExecuteHTTPGetRequest(string url)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
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

        foreach (var kvp in headers)
        {
            httpRequestMessage.Headers.Add(kvp.Key, kvp.Value);
        }

        var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
        _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
    }

    private List<KeyValuePair<string, string>> ExtractHeaders(Table table)
    {
        var result = new List<KeyValuePair<string, string>>();
        foreach (var record in table.Rows)
        {
            var key = record["Key"];
            var value = ParseValue(_scenarioContext, record["Value"]).ToString();
            if (!PARAMETERS_IN_HEADER.Contains(key)) continue;
            result.Add(new KeyValuePair<string, string>(key, value.ToString()));
        }

        return result;
    }

    private JsonObject ExtractBody(Table table)
    {
        var result = new JsonObject();
        foreach (var record in table.Rows)
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
