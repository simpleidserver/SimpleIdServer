// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.Webfinger.Client;

public interface IWebfingerClientFactory
{
    WebfingerClient Build();
}

public class WebfingerClientFactory : IWebfingerClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebfingerClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;    
    }

    public WebfingerClient Build()
        => new WebfingerClient(_httpClientFactory);
}
