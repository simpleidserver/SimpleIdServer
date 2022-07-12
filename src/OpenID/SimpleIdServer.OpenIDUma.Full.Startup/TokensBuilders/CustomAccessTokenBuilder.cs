// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenIDUma.Full.Startup.TokensBuilders
{
    public class CustomAccessTokenBuilder : OpenIDAccessTokenBuilder
    {
        public CustomAccessTokenBuilder(
            IGrantedTokenHelper grantedTokenHelper, 
            IJwtBuilder jwtBuilder,
            IOAuthClientRepository oauthClientRepository) : base(grantedTokenHelper, jwtBuilder, oauthClientRepository)
        {
        }

        protected override async Task<JwsPayload> BuildPayload(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var result = await base.BuildPayload(scopes, handlerContext, cancellationToken);
            if (handlerContext.Request.RequestData.ContainsKey("uniqueid"))
            {
                result.Add("uniqueid", handlerContext.Request.RequestData["uniqueid"].ToString());
            }

            return result;
        }
    }
}
