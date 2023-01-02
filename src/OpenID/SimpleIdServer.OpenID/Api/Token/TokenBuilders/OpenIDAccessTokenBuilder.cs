// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class OpenIDAccessTokenBuilder : AccessTokenBuilder
    {
        public OpenIDAccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, 
            IJwtBuilder jwtBuilder,
            ClientRepository clientRepository,
            IOptions<OAuthHostOptions> options) : base(grantedTokenHelper, jwtBuilder, clientRepository, options) { }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var jwsPayload = await BuildOpenIdPayload(scopes, handlerContext.Request.RequestData, handlerContext, cancellationToken);
            await SetResponse(handlerContext, jwsPayload, cancellationToken);
        }

        public override async Task Refresh(JsonObject previousRequest, HandlerContext currentContext, CancellationToken cancellationToken)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            var jwsPayload = await BuildOpenIdPayload(scopes, previousRequest, currentContext, cancellationToken);
            await SetResponse(currentContext, jwsPayload, cancellationToken);
        }

        protected virtual async Task<SecurityTokenDescriptor> BuildOpenIdPayload(IEnumerable<string> scopes, JsonObject queryParameters, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var jwsPayload = await BuildTokenDescriptor(scopes, handlerContext, cancellationToken);            
            if (handlerContext.User != null)
            {
                jwsPayload.Claims.Add(JwtRegisteredClaimNames.Sub, handlerContext.User.Id);
                var activeSession = handlerContext.User.ActiveSession;
                if (activeSession != null)
                    jwsPayload.Claims.Add(JwtRegisteredClaimNames.AuthTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());
            }

            if (queryParameters.ContainsKey(AuthorizationRequestParameters.Claims))
            {
                var value = JsonSerializer.SerializeToNode(queryParameters[AuthorizationRequestParameters.Claims].ToString()).AsObject();
                jwsPayload.Claims.Add(AuthorizationRequestParameters.Claims, value);
            }

            return jwsPayload;
        }
    }
}