// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders
{
    public class ClientBuilder
    {
        /// <summary>
        /// Build client for REST.API.
        /// By default client_credentials grant-type is used to obtain an access token outside of the context of a user.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Build client for traditional website like ASP.NET CORE.
        /// By default authorization_code grant-type is used by confidential and public clients to exchange an authorization code for an access token.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static TraditionalWebsiteClientBuilder BuildTraditionalWebsiteClient(string clientId, string clientSecret, params string[] redirectUrls)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectionUrls = redirectUrls,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }
            };
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            return new TraditionalWebsiteClientBuilder(client);
        }

        /// <summary>
        /// Build client for user-agent based application for example : SPA, angular etc...
        /// By default implicit grant-type is used.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static UserAgentClientBuilder BuildUserAgentClient(string clientId, string clientSecret, params string[] redirectUrls)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectionUrls = redirectUrls,
                ResponseTypes = new List<string> { IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE }
            };
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.GrantTypes.Add("implicit");
            return new UserAgentClientBuilder(client);
        }
    }
}
