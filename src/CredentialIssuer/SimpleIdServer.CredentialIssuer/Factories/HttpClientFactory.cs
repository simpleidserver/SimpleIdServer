// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.CredentialIssuer;
using System.Net.Http;

namespace SimpleIdServer.CredentialIssuer.Factories;

public interface IHttpClientFactory
{
    HttpClient Build();
}

public class HttpClientFactory : IHttpClientFactory
{
    private readonly CredentialIssuerOptions _options;

    public HttpClientFactory(IOptions<CredentialIssuerOptions> options)
    {
        _options = options.Value;
    }

    public HttpClient Build()
    {
        var handler = new HttpClientHandler();
        if(_options.IgnoreHttpsCertificateError)
        {
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };
        }

        var result = new HttpClient(handler);
        return result;
    }
}
