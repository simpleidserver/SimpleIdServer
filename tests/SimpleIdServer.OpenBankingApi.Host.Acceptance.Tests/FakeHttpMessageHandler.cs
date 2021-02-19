// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests
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
