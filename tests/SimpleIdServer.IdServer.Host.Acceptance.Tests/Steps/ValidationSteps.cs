// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Steps
{
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

        [Then("access_token audience contains '(.*)'")]
        public void ThenAccessTokenAudienceContains(string value)
        {
            var jwt = GetAccessToken();
            Assert.Contains(value, jwt.Audiences);
        }

        [Then("access_token scope contains '(.*)'")]
        public void ThenAccessTokenScopeContains(string value)
        {
            var jwt = GetAccessToken();
            var scopes = jwt.Claims.Where(c => c.Type == "scope").Select(s => s.Value);
            Assert.Contains(value, scopes);
        }

        [Then("access_token alg equals to '(.*)'")]
        public void ThenAccessTokenAlgEqualsTo(string alg)
        {
            var jwt = GetAccessToken();
            Assert.Equal(alg, jwt.Alg);
        }

        [Then("access_token kid equals to '(.*)'")]
        public void ThenAccessTokenKidEqualsTo(string kid)
        {
            var jwt = GetAccessToken();
            Assert.Equal(kid, jwt.Kid);
        }

        [Then("access_token contains the claim '(.*)'='(.*)'")]
        public void ThenAccessTokenContainsTheClaim(string key, string value)
        {
            var jwt = GetAccessToken();
            Assert.True(jwt.Claims.Any(c => c.Type == key && c.Value == value) == true);
        }

        [Then("access_token has permission to access to the resource id '(.*)'")]
        public void ThenAccessTokenHasPermissionToAccessToResourceId(string value)
        {
            value = WebApiSteps.ParseValue(_scenarioContext, value).ToString();
            var jwt = GetAccessToken();
            var str = JsonArray.Parse(jwt.GetClaim("permissions").Value)[0];
            Assert.Equal(value, str["resource_id"].GetValue<string>());
        }

        [Then("access_token has permission to access to the scope '(.*)'")]
        public void ThenAccessTokenHasPermissionToAccessToScope(string value)
        {
            value = WebApiSteps.ParseValue(_scenarioContext, value).ToString();
            var jwt = GetAccessToken();
            var str = JsonArray.Parse(jwt.GetClaim("permissions").Value)[0];
            var scopes = (str["resource_scopes"] as JsonArray).Select(s => s.GetValue<string>());
            Assert.True(scopes.Contains(value) == true);
        }

        [Then("access_token doesn't contain the claim '(.*)'='(.*)'")]
        public void ThenAccessTokenDoesntContainTheClaim(string key, string value)
        {
            var jwt = GetAccessToken();
            Assert.True(jwt.Claims.Any(c => c.Type == key && c.Value == value) == false);
        }

        [Then("access_token contains jkt")]
        public void ThenAccessTokenContainsJkt()
        {
            var cnf = JsonObject.Parse(GetAccessToken().Claims.First(c => c.Type == "cnf").Value).AsObject();
            Assert.True(cnf.ContainsKey("jkt"));
        }

        [Then("JWT contains '(.*)'")]
        public void ThenJWTContainsClaim(string key) => Assert.True(GetJWT().Claims.Any(c => c.Type == key) == true);

        [Then("JWT has '(.*)'='(.*)'")]
        public void ThenJWTHas(string key, string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == key && c.Value == value) == true);

        [Then("JWT has address street '(.*)'")]
        public void ThenJWTHasAddressStreet(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "address" && JsonObject.Parse(c.Value)["street"].GetValue<string>() == value) == true);

        [Then("JWT has address locality '(.*)'")]
        public void ThenJWTHasAddressLocality(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "address" && JsonObject.Parse(c.Value)["locality"].GetValue<string>() == value) == true);

        [Then("JWT has address region '(.*)'")]
        public void ThenJWTHasAddressRegion(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "address" && JsonObject.Parse(c.Value)["region"].GetValue<string>() == value) == true);

        [Then("JWT has address postal code '(.*)'")]
        public void ThenJWTHasAddressPostalCode(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "address" && JsonObject.Parse(c.Value)["postal_code"].GetValue<string>() == value) == true);

        [Then("JWT has address country '(.*)'")]
        public void ThenJWTHasAddressCountry(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "address" && JsonObject.Parse(c.Value)["country"].GetValue<string>() == value) == true);

        [Then("JWT has formatted address '(.*)'")]
        public void ThenJWTHasFormattedAddress(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "address" && JsonObject.Parse(c.Value)["formatted"].GetValue<string>() == value) == true);

        [Then("JWT has authorization_details type '(.*)'")]
        public void ThenJWTHasAuthorizationDetailsType(string value) => Assert.True(GetJWT().Claims.Any(c => c.Type == "authorization_details" && JsonObject.Parse(c.Value)["type"].GetValue<string>() == value) == true);

        [Then("JWT has authorization_details action '(.*)'")]
        public void ThenAccessTokenHasAuthorizationDetailsHasAction(string value) =>  Assert.True(GetJWT().Claims.Any(c => c.Type == "authorization_details" && JsonObject.Parse(c.Value)["actions"].AsArray().Select(a => a.GetValue<string>()).Contains(value)));

        [Then("JWT is encrypted")]
        public void ThenJWTIsEncrypted() => Assert.True(GetJWT().IsEncrypted);

        [Then("JWT alg = '(.*)'")]
        public void ThenJWTAlgEqualsTo(string alg) => Assert.True(GetJWT().Alg == alg);

        [Then("JWT enc = '(.*)'")]
        public void ThenJWTEncEqualsTo(string enc) => Assert.True(GetJWT().Enc == enc);

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

        private JsonWebToken GetAccessToken()
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JsonDocument;
            var handler = new JsonWebTokenHandler();
            return handler.ReadJsonWebToken(jsonHttpBody.SelectToken("$.access_token").Value.GetString());
        }

        private JsonWebToken GetJWT() => _scenarioContext["jwt"] as JsonWebToken;
    }
}
