// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net.Http;

namespace SimpleIdServer.OAuth.Infrastructures
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient GetHttpClient() => new HttpClient();

        public HttpClient GetHttpClient(HttpClientHandler handler) => new HttpClient(handler);
    }
}
