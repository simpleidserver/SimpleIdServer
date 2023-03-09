// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Domains;
using System;
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
                ClientSecret = clientSecret,
                ClientType = ClientTypes.MACHINE,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            client.Realms.Add(Constants.StandardRealms.Master);
            client.GrantTypes.Add(ClientCredentialsHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
            return new ApiClientBuilder(client);
        }

        /// <summary>
        /// Build client for traditional website like ASP.NET CORE.
        /// By default authorization_code grant-type PKCE is used by confidential and public clients to exchange an authorization code for an access token.
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
                ClientType = ClientTypes.WEBSITE,
                RedirectionUrls = redirectUrls,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }
            };
            client.Realms.Add(Constants.StandardRealms.Master);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
            return new TraditionalWebsiteClientBuilder(client);
        }

        /// <summary>
        /// Build external authentication device client.
        /// CIBA is enabled.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clietnSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static TraditionalWebsiteClientBuilder BuildExternalAuthDeviceClient(string clientId, string clientSecret, params string[] redirectUrls)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                ClientType = ClientTypes.EXTERNAL,
                RedirectionUrls = redirectUrls,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }
            };
            client.Realms.Add(Constants.StandardRealms.Master);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
            var result = new TraditionalWebsiteClientBuilder(client);
            result.EnableCIBAGrantType();
            return result;
        }

        /// <summary>
        /// Build mobile application.
        /// Authorization code + PKCE.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static MobileClientBuilder BuildMobileApplication(string clientId, string clientSecret, params string[] redirectUrls)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                ClientType = ClientTypes.MOBILE,
                RedirectionUrls = redirectUrls,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }
            };
            client.Realms.Add(Constants.StandardRealms.Master);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthPKCEAuthenticationHandler.AUTH_METHOD;
            return new MobileClientBuilder(client);
        }

        /// <summary>
        /// Build client for user-agent based application for example : SPA, angular etc...
        /// Authorization code + PKCE.
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
                ClientType = ClientTypes.SPA,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }
            };
            client.Realms.Add(Constants.StandardRealms.Master);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthPKCEAuthenticationHandler.AUTH_METHOD;
            return new UserAgentClientBuilder(client);
        }
    }
}
