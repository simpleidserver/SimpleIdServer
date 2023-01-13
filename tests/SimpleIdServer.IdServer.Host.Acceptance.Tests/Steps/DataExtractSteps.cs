// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Steps
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
            _scenarioContext.Set(JsonDocument.Parse(json), "jsonHttpBody");
        }

        [When("extract query parameters into JSON")]
        public void WhenExtractQueryParametersIntoJSON()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queryValues = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
            var jObj = new JsonObject();
            foreach (var kvp in queryValues)
            {
                jObj.Add(kvp.Key, queryValues[kvp.Key][0]);
            }

            _scenarioContext.Set(jObj, "jsonHttpBody");
        }

        [When("extract parameter '(.*)' from redirect url")]
        public void WhenExtractRedirectUrlParameter(string parameter)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var queries = QueryHelpers.ParseQuery(httpResponseMessage.RequestMessage.RequestUri.Query);
            var queryValue = queries[parameter][0];
            _scenarioContext.Set(queryValue, parameter);
        }

        [When("extract parameter '(.*)' from JSON body")]
        public void WhenExtractParameterFromBody(string parameter)
        {
            var jObj = _scenarioContext.Get<JsonDocument>("jsonHttpBody");
            _scenarioContext.Set(jObj.SelectToken(parameter).Value.GetString(), parameter);
        }

        [When("extract parameter '(.*)' from JSON body into '(.*)'")]
        public void WhenExtractParameterFromBody(string parameter, string key)
        {
            var jObj = _scenarioContext.Get<JsonDocument>("jsonHttpBody");
            _scenarioContext.Set(jObj.SelectToken(parameter).Value.GetString(), key);
        }

        [When("extract payload from JWT '(.*)'")]
        public void WhenExtractPayloadFromJWT(string key)
        {
            var str = WebApiSteps.ParseValue(_scenarioContext, key);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(str.ToString());
            _scenarioContext.Set(jwt, "jwt");
        }

        [When("extract payload from HTTP body")]
        public async Task WhenExtractPayloadFromHTTPBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(json);
            _scenarioContext.Set(jwt, "jwt");
        }
    }
}
