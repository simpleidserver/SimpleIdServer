// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class OpenIDAccessTokenBuilder : AccessTokenBuilder
    {
        public OpenIDAccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, 
            IJwtBuilder jwtBuilder, 
            IOptions<OAuthHostOptions> options) : base(grantedTokenHelper, jwtBuilder, options) {
        }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var jwsPayload = await BuildOpenIdPayload(scopes, handlerContext.Request.RequestData, handlerContext, cancellationToken);
            await SetResponse(handlerContext, jwsPayload, cancellationToken);
        }

        public override async Task Refresh(JObject previousRequest, HandlerContext currentContext, CancellationToken cancellationToken)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            var jwsPayload = await BuildOpenIdPayload(scopes, previousRequest, currentContext, cancellationToken);
            await SetResponse(currentContext, jwsPayload, cancellationToken);
        }

        protected virtual Task<JwsPayload> BuildOpenIdPayload(IEnumerable<string> scopes, JObject queryParameters, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var jwsPayload = BuildPayload(scopes, handlerContext);
            if (handlerContext.User != null)
            {
                jwsPayload.Add(UserClaims.Subject, handlerContext.User.Id);
                var activeSession = handlerContext.User.GetActiveSession();
                if (activeSession != null)
                {
                    jwsPayload.Add(OAuthClaims.AuthenticationTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());
                }
            }

            if (queryParameters.ContainsKey(AuthorizationRequestParameters.Claims))
            {
                var value = JObject.Parse(queryParameters[AuthorizationRequestParameters.Claims].ToString());
                jwsPayload.Add(AuthorizationRequestParameters.Claims, value);
            }

            return Task.FromResult(jwsPayload);
        }
    }
}