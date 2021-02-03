// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using System.Collections.Generic;
using System.Threading;
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

        public async virtual Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, JObject claims = null)
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

        public virtual Task Refresh(JObject previousRequest, HandlerContext currentContext, CancellationToken token)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            return Build(scopes, currentContext);
        }
    }
}