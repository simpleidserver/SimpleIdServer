using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class SharedSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private CustomWebApplicationFactory<FakeStartup> _factory;

        public SharedSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = new CustomWebApplicationFactory<FakeStartup>((o) =>
            {
                o.AddSingleton(scenarioContext);
            });
        }

        [When("execute HTTP POST JSON request '(.*)'")]
        public async Task WhenExecuteHTTPPostJSONRequest(string url, Table table)
        {
            var jObj = new JObject();
            foreach (var record in table.Rows)
            {
                var key = record["Key"];
                var value = record["Value"];
                try
                {
                    jObj.Add(key, JToken.Parse(value));
                }
                catch
                {
                    jObj.Add(key, value.ToString());
                }
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await _factory.CreateClient().SendAsync(httpRequestMessage).ConfigureAwait(false);
            _scenarioContext.Set(httpResponseMessage, "httpResponseMessage");
        }


        [When("extract JSON from body")]
        public async Task GivenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _scenarioContext.Set(JsonConvert.DeserializeObject<JObject>(json), "jsonHttpBody");
        }

        [Then("HTTP status code equals to '(.*)'")]
        public void ThenCheckHttpStatusCode(int code)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.Equal(code, (int)httpResponseMessage.StatusCode);
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
            Assert.Equal(value, jsonHttpBody.SelectToken(key).ToString());
        }
    }
}