// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseTypes
{
    public class TokenResponseTypeHandler : IResponseTypeHandler
    {
        public static string RESPONSE_TYPE = "token";

        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public TokenResponseTypeHandler(IEnumerable<ITokenBuilder> tokenBuilders)
        {
            _tokenBuilders = tokenBuilders;
        }

        public virtual string GrantType => "implicit";
        public string ResponseType => RESPONSE_TYPE;
        public int Order => 2;

        public void Enrich(HandlerContext context)
        {
            _tokenBuilders.First(t => t.Name == AuthorizationResponseParameters.AccessToken).Build(context.Request.QueryParameters.GetScopesFromAuthorizationRequest(), context);
        }
    }
}