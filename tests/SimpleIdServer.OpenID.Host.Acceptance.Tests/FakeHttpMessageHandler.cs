using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private ScenarioContext _scenarioContext;
        private Dictionary<string, HttpResponseMessage> _responseMessages;

        public FakeHttpMessageHandler(
            ScenarioContext scenarioContext,
            Dictionary<string, HttpResponseMessage> responseMessages)
        {
            _scenarioContext = scenarioContext;
            _responseMessages = responseMessages;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var json = await request.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(json))
            {
                _scenarioContext.Add("callbackResponse", json);
            }

            return _responseMessages[request.RequestUri.AbsoluteUri];
        }
    }
}
