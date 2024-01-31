// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;
using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;
using BlushingPenguin.JsonPath;
using System.Net.Http;
using System.Linq;

namespace SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests.Steps;

[Binding]
public class ValidationSteps
{
    private readonly ScenarioContext _scenarioContext;

    public ValidationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Then("HTTP status code equals to '(.*)'")]
    public void ThenCheckHttpStatusCode(int code)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        Assert.Equal(code, (int)httpResponseMessage.StatusCode);
    }

    [Then("HTTP header has '(.*)'='(.*)'")]
    public void ThenHttpHeaderContains(string key, string value)
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        Assert.True(httpResponseMessage.Content.Headers.Contains(key));
        Assert.True(httpResponseMessage.Content.Headers.GetValues(key).Contains(value) == true);
    }

    [Then("JSON exists '(.*)'")]
    public void ThenExists(string key)
    {
        var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonDocument;
        if (jsonHttpBody == null)
        {
            var jsonObj = _scenarioContext["jsonHttpBody"] as JsonObject;
            if (jsonObj != null) jsonHttpBody = JsonDocument.Parse(jsonObj.ToJsonString());
        }

        Assert.True(jsonHttpBody.SelectToken(key) != null);
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
}
