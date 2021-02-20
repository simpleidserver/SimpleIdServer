// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Net.Http;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class ValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public ValidationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }


        [Then("JSON exists '(.*)'")]
        public void ThenExists(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            var token = jsonHttpBody.SelectToken(key);
            Assert.NotNull(token);
        }

        [Then("JSON '(.*)'='(.*)'")]
        public void ThenEqualsTo(string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            var token = jsonHttpBody.SelectToken(key).ToString();
            Assert.Equal(value, token);
        }

        [Then("HTTP Header '(.*)'='(.*)'")]
        public void ThenHttpHeaderEqualsTo(string key, string value)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpHeader"] as JObject;
            var token = jsonHttpBody.SelectToken(key).ToString();
            Assert.Equal(value, token);
        }

        [Then("HTTP status code equals to '(.*)'")]
        public void ThenCheckHttpStatusCode(int code)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.Equal(code, (int)httpResponseMessage.StatusCode);
        }
    }
}
