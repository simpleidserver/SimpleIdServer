// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class OpenIDRefreshTokenBuilder : RefreshTokenBuilder
    {
        public OpenIDRefreshTokenBuilder(IGrantedTokenHelper grantedTokenHelper) : base(grantedTokenHelper) { }

        public override Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, JObject claims = null)
        {
            var dic = new JObject();
            if (handlerContext.Request.Data != null)
            {
                foreach (var record in handlerContext.Request.Data)
                {
                    dic.Add(record.Key, record.Value);
                }
            }

            if (handlerContext.User != null)
            {
                dic.Add(UserClaims.Subject, handlerContext.User.Id);
            }

            var refreshToken = GrantedTokenHelper.BuildRefreshToken(dic, handlerContext.Client.RefreshTokenExpirationTimeInSeconds);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
            return Task.FromResult(0);
        }
    }
}
