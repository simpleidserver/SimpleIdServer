// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.FastFed.Host.Acceptance.Tests;

[Binding]
public class ValidationSteps
{
    private readonly ScenarioContext _scenarioContext;

    public ValidationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Then("parameter '(.*)'='(.*)'")]
    public void ThenParametersEqualsTo(string parameter, string value)
    {
        var val = _scenarioContext.Get<string>(parameter);
        Assert.Equal(value, val);
    }


    [Then("JSON exists '(.*)'")]
    public void ThenExists(string key)
    {
        var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonDocument;
        if(jsonHttpBody == null)
        {
            var jsonObj = _scenarioContext["jsonHttpBody"] as JsonObject;
            if (jsonObj != null) jsonHttpBody = JsonDocument.Parse(jsonObj.ToJsonString());
        }

        Assert.True(jsonHttpBody.SelectToken(key) != null);
    }


    [Then("redirect uri equals to '(.*)'")]
    public void ThenRedirectUriEuals(string redirectUri)
    {
        var requestUri = _scenarioContext["requestUri"].ToString();
        Assert.True(requestUri == redirectUri);
    }


    [Then("JSON doesn't exist '(.*)'")]
    public void ThenDoesntExist(string key)
    {
        var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonDocument;
        Assert.True(jsonHttpBody.SelectToken(key) == null);
    }

    [Then("JSON '(.*)'='(.*)'")]
    public void ThenEqualsTo(string key, string expectedValue)
    {
        var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonDocument;
        var value = GetValue(jsonHttpBody);
        expectedValue = WebApiSteps.ParseValue(_scenarioContext, expectedValue).ToString();
        Assert.Equal(expectedValue, value);

        string GetValue(JsonDocument elt)
        {
            var selectedToken = elt.SelectToken(key);
            switch (selectedToken.Value.ValueKind)
            {
                case JsonValueKind.True:
                    return "true";
                case JsonValueKind.False:
                    return "false";
                case JsonValueKind.Number:
                    return selectedToken?.GetInt32().ToString();
                default:
                    return selectedToken?.GetString();
            }

        }
    }

    [Then("HTTP status code equals to '(.*)'")]
    public void ThenCheckHttpStatusCode(int code)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        Assert.Equal(code, (int)httpResponseMessage.StatusCode);
    }

    [Then("redirection url doesn't contain the parameter '(.*)'")]
    public void ThenRedirectionUrlDoesntContain(string parameter)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        var queries = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
        Assert.True(queries.ContainsKey(parameter) == false);
    }

    [Then("redirection url contains the parameter '(.*)'")]
    public void ThenRedirectionUrlContainsQuery(string parameter)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        var queries = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
        Assert.True(queries.ContainsKey(parameter) == true);
    }

    [Then("redirection url contains the parameter value '(.*)'='(.*)'")]
    public void ThenRedirectionUrlContainsQuery(string key, string value)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        var queries = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
        Assert.True(queries.Any(q => q.Key == key && q.Value == value) == true);
    }

    [Then("redirection url contains '(.*)'")]
    public void ThenRedirectionUrlContains(string baseUrl)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        Assert.True(httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.Contains(baseUrl) == true);
    }

    [Then("HTTP header '(.*)' exists")]
    public void ThenHttpHeaderExists(string key)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        Assert.True(httpResponseMessage.Headers.Contains(key));
    }

    [Then("HTTP header has '(.*)'='(.*)'")]
    public void ThenHttpHeaderContains(string key, string value)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        Assert.True(httpResponseMessage.Content.Headers.Contains(key));
        Assert.True(httpResponseMessage.Content.Headers.GetValues(key).Contains(value) == true);
    }
}
