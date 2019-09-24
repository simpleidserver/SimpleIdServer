// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class RefreshTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;

        public RefreshTokenBuilder(IGrantedTokenHelper grantedTokenHelper)
        {
            _grantedTokenHelper = grantedTokenHelper;
        }

        public string Name => TokenResponseParameters.RefreshToken;

        public Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, Dictionary<string, object> claims = null)
        {
            var jwsPayload = _grantedTokenHelper.BuildAccessToken(new[]
            {
                handlerContext.Client.ClientId
            }, scopes, handlerContext.Request.IssuerName, handlerContext.Client.TokenExpirationTimeInSeconds);
            if (claims != null)
            {
                foreach (var cl in claims)
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

        private Task InternalBuild(JwsPayload jwsPayload, HandlerContext handlerContext)
        {
            var refreshToken = _grantedTokenHelper.BuildRefreshToken(jwsPayload, handlerContext.Client.RefreshTokenExpirationTimeInSeconds);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
            return Task.FromResult(0);
        }
    }
}
