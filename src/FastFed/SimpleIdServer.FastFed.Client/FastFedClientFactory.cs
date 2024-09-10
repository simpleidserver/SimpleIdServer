// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.FastFed.Client
{
    public interface IFastFedClientFactory
    {
        FastFedClient Build();
    }

    public class FastFedClientFactory : IFastFedClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FastFedClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public FastFedClient Build()
            => new FastFedClient(_httpClientFactory);
    }
}
