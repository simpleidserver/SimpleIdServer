// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseTypes
{
    public class AuthorizationCodeResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;

        public AuthorizationCodeResponseTypeHandler(IGrantedTokenHelper grantedTokenHelper)
        {
            _grantedTokenHelper = grantedTokenHelper;
        }

        public string GrantType => AuthorizationCodeHandler.GRANT_TYPE;
        public string ResponseType => RESPONSE_TYPE;
        public int Order => 1;
        public static string RESPONSE_TYPE = "code";

        public void Enrich(HandlerContext context)
        {
            JwsPayload jwsPayload = _grantedTokenHelper.BuildAccessToken(
                    new[] { context.Client.ClientId },
                    context.Request.QueryParameters.GetScopesFromAuthorizationRequest(),
                    context.Request.IssuerName, context.Client.TokenExpirationTimeInSeconds
            );
            var authCode = _grantedTokenHelper.BuildAuthorizationCode(jwsPayload);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
        }
    }
}