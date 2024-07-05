// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Helpers
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();
        HttpClient GetHttpClient(HttpClientHandler handler);
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient GetHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => {
                return true;
            };
            return new HttpClient(handler);
        }

        public HttpClient GetHttpClient(HttpClientHandler handler) => new HttpClient(handler);
    }
}
