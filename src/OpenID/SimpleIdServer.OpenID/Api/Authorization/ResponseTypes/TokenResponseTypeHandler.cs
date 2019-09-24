// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Linq;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Authorization.ResponseTypes
{
    public class TokenResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public TokenResponseTypeHandler(IEnumerable<ITokenBuilder> tokenBuilders)
        {
            _tokenBuilders = tokenBuilders;
        }

        public string GrantType => "implicit";
        public string ResponseType => "token";
        public int Order => 2;

        public void Enrich(HandlerContext context)
        {
            var dic = new Dictionary<string, object>
            {
                { UserClaims.Subject, context.User.Id },
                { OAuthClaims.AuthenticationTime, context.Request.AuthDateTime.Value.ConvertToUnixTimestamp() }
            };
            if (context.Request.QueryParameters.ContainsKey(AuthorizationRequestParameters.Claims))
            {
                dic.Add(OAuthClaims.Claims, context.Request.QueryParameters[AuthorizationRequestParameters.Claims]);
            }

            _tokenBuilders.First(t => t.Name == OAuth.DTOs.AuthorizationResponseParameters.AccessToken).Build(context.Request.QueryParameters.GetScopesFromAuthorizationRequest(), context, dic);
        }
    }
}