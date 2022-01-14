// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenIDUma.Full.Startup.TokensBuilders
{
    public class CustomAccessTokenBuilder : OpenIDAccessTokenBuilder
    {
        public CustomAccessTokenBuilder(
            IGrantedTokenHelper grantedTokenHelper, 
            IJwtBuilder jwtBuilder, 
            IOptions<OAuthHostOptions> options) : base(grantedTokenHelper, jwtBuilder, options)
        {
        }

        protected override JwsPayload BuildPayload(IEnumerable<string> scopes, HandlerContext handlerContext)
        {
            var result = base.BuildPayload(scopes, handlerContext);
            if (handlerContext.Request.RequestData.ContainsKey("uniqueid"))
            {
                result.Add("uniqueid", handlerContext.Request.RequestData["uniqueid"].ToString());
            }

            return result;
        }
    }
}
