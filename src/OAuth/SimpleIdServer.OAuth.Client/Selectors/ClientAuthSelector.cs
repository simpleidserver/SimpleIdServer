// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Client.Selectors
{
    public interface IClientAuthSelector
    {
        IOAuthActionSelector UseClientSecretPost(string clientId, string clientSecret);
    }

    internal class ClientAuthSelector : IClientAuthSelector
    {
        private readonly TokenRequestBuilder _tokenRequestBuilder;
        private readonly IOAuthActionSelector _oAuthActionSelector;

        public ClientAuthSelector(IHttpClientFactory httpClientFactory, TokenRequestBuilder tokenRequestBuilder)
        {
            _tokenRequestBuilder = tokenRequestBuilder;
            _oAuthActionSelector = new OAuthActionSelector(httpClientFactory, tokenRequestBuilder);
        }

        public IOAuthActionSelector UseClientSecretPost(string clientId, string clientSecret)
        {
            _tokenRequestBuilder.AddBody("client_id", clientId);
            _tokenRequestBuilder.AddBody("client_secret", clientSecret);
            return _oAuthActionSelector;
        }
    }
}