// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net.Http;

namespace SimpleIdServer.IdServer.Infrastructures
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();
        HttpClient GetHttpClient(HttpClientHandler handler);
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient GetHttpClient() => new HttpClient();

        public HttpClient GetHttpClient(HttpClientHandler handler) => new HttpClient(handler);
    }
}
