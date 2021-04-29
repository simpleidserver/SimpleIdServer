// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class OpenIDRefreshTokenBuilder : RefreshTokenBuilder
    {
        public OpenIDRefreshTokenBuilder(IGrantedTokenHelper grantedTokenHelper) : base(grantedTokenHelper) { }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var dic = new JObject();
            if (handlerContext.Request.RequestData != null)
            {
                foreach (var record in handlerContext.Request.RequestData)
                {
                    dic.Add(record.Key, record.Value);
                }
            }

            if (handlerContext.User != null)
            {
                dic.Add(UserClaims.Subject, handlerContext.User.Id);
            }

            var authorizationCode = string.Empty;
            handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode);
            var refreshToken = await GrantedTokenHelper.AddRefreshToken(handlerContext.Client.ClientId, authorizationCode, dic, handlerContext.Client.RefreshTokenExpirationTimeInSeconds, cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
        }
    }
}
