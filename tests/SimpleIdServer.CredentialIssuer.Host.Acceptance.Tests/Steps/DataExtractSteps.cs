// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests.Steps;

[Binding]
public class DataExtractSteps
{
    private readonly ScenarioContext _scenarioContext;

    public DataExtractSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [When("extract JSON from body")]
    public async Task WhenExtractFromBody()
    {
        var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
        var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        _scenarioContext.Set(JsonDocument.Parse(json), "jsonHttpBody");
    }
}
