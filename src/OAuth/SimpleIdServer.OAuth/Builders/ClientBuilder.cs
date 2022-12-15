// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;

namespace SimpleIdServer.OAuth.Builders
{
    public class ClientBuilder
    {
        public static ApiClientBuilder BuildApiClient(string clientId, string clientSecret)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
            client.GrantTypes.Add(ClientCredentialsHandler.GRANT_TYPE);
            return new ApiClientBuilder(client);
        }

        public static TraditionalWebsiteClientBuilder BuildTraditionalWebsiteClient(string clientId, string clientSecret, params string[] redirectUrls)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectionUrls = redirectUrls,
                ResponseTypes = new[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }
            };
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            return new TraditionalWebsiteClientBuilder(client);
        }
    }
}
