// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public Task Enrich(HandlerContext context, CancellationToken cancellationToken)
        {
            var dic = new JObject
            {
                { UserClaims.Subject, context.User.Id }
            };
            if (context.User.AuthenticationTime != null)
            {
                dic.Add(OAuthClaims.AuthenticationTime, context.User.AuthenticationTime.Value.ConvertToUnixTimestamp());
            }

            if (context.Request.Data.ContainsKey(AuthorizationRequestParameters.Claims))
            {
                dic.Add(OAuthClaims.Claims, context.Request.Data[AuthorizationRequestParameters.Claims]);
            }

            return _tokenBuilders.First(t => t.Name == OAuth.DTOs.AuthorizationResponseParameters.AccessToken).Build(context.Request.Data.GetScopesFromAuthorizationRequest(), context, cancellationToken, dic);
        }
    }
}