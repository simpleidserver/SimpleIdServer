// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class AccessTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IJwtBuilder _jwtBuilder;

        public AccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IJwtBuilder jwtBuilder)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _jwtBuilder = jwtBuilder;
        }

        public string Name => TokenResponseParameters.AccessToken;

        public Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, Dictionary<string, object> claims = null)
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

            return InternalBuild(jwsPayload, handlerContext);
        }

        public Task Build(JwsPayload jwsPayload, HandlerContext handlerContext)
        {
            _grantedTokenHelper.RefreshAccessToken(jwsPayload, handlerContext.Client.TokenExpirationTimeInSeconds);
            return InternalBuild(jwsPayload, handlerContext);
        }

        private async Task InternalBuild(JwsPayload jwsPayload, HandlerContext handlerContext)
        {
            var accessToken = await _jwtBuilder.BuildAccessToken(handlerContext.Client, jwsPayload);
            handlerContext.Response.Add(TokenResponseParameters.AccessToken, accessToken);
        }
    }
}