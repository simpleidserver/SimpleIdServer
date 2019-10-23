// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class OpenIDAccessTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IJwtBuilder _jwtBuilder;

        public OpenIDAccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IJwtBuilder jwtBuilder)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _jwtBuilder = jwtBuilder;
        }

        public string Name => TokenResponseParameters.AccessToken;

        public async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, JObject claims = null)
        {
            var jwsPayload = _grantedTokenHelper.BuildAccessToken(new[]
            {
                handlerContext.Client.ClientId
            }, scopes, handlerContext.Request.IssuerName, handlerContext.Client.TokenExpirationTimeInSeconds);
            if (claims != null)
            {
                foreach(var cl in claims)
                {
                    jwsPayload.Add(cl.Key, cl.Value);
                }
            }

            var accessToken = await _jwtBuilder.BuildAccessToken(handlerContext.Client, jwsPayload);
            handlerContext.Response.Add(TokenResponseParameters.AccessToken, accessToken);
        }

        public Task Refresh(JObject previousRequest, HandlerContext currentContext)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            return Build(scopes, currentContext);
        }
    }
}