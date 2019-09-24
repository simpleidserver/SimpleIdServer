using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private Dictionary<string, HttpResponseMessage> _responseMessages;

        public FakeHttpMessageHandler(Dictionary<string, HttpResponseMessage> responseMessages)
        {
            _responseMessages = responseMessages;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseMessages[request.RequestUri.AbsoluteUri]);
        }
    }
}
