// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class OpenIDAccessTokenBuilder : AccessTokenBuilder
    {
        public OpenIDAccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IJwtBuilder jwtBuilder) : base(grantedTokenHelper, jwtBuilder) { }        

        public override Task Refresh(JObject previousRequest, HandlerContext currentContext, CancellationToken token)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            var jObj = new JObject();
            if (previousRequest.ContainsKey(UserClaims.Subject))
            {
                jObj.Add(UserClaims.Subject, previousRequest[UserClaims.Subject].ToString());
            }

            return Build(scopes, currentContext, jObj);
        }
    }
}