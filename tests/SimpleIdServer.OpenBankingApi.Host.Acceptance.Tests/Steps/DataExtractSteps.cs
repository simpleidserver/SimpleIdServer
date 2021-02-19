// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class DataExtractSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public DataExtractSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When("extract JSON from body")]
        public async Task GivenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _scenarioContext.Set(JsonConvert.DeserializeObject<JObject>(json), "jsonHttpBody");
        }

        [When("extract HTTP headers")]
        public void GivenExtractFromHeader()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var jObj = new JObject();
            foreach(var kvp in httpResponseMessage.Headers)
            {
                jObj.Add(kvp.Key, kvp.Value.ElementAt(0));
            }

            _scenarioContext.Set(jObj, "jsonHttpHeader");
        }

        [When("extract query parameters into JSON")]
        public void WhenExtractQueryParametersIntoJSON()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queryValues = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
            var jObj = new JObject();
            foreach (var kvp in queryValues)
            {
                jObj.Add(kvp.Key, queryValues[kvp.Key][0]);
            }

            _scenarioContext.Set(jObj, "jsonHttpBody");
        }

        [When("extract parameter '(.*)' from redirect url")]
        public void GivenExtractRedirectUrlParameter(string parameter)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queries = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
            var queryValue = queries[parameter][0];
            _scenarioContext.Set(queryValue, parameter);
        }

        [When("extract parameter '(.*)' from JSON body")]
        public void GivenExtractParameterFromBody(string parameter)
        {
            var jObj = _scenarioContext.Get<JObject>("jsonHttpBody");
            _scenarioContext.Set(jObj[parameter].ToString(), parameter);
        }

        [When("extract parameter '(.*)' from JSON body into '(.*)'")]
        public void GivenExtractParameterFromBody(string parameter, string key)
        {
            var jObj = _scenarioContext.Get<JObject>("jsonHttpBody");
            _scenarioContext.Set(jObj[parameter].ToString(), key);
        }
    }
}
