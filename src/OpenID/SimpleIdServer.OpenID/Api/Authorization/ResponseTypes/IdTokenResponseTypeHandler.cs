// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenID.Api.Authorization.ResponseTypes
{
    public class IdTokenResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public IdTokenResponseTypeHandler(IEnumerable<ITokenBuilder> tokenBuilders)
        {
            _tokenBuilders = tokenBuilders;
        }

        public string GrantType => "implicit";
        public string ResponseType => "id_token";
        public int Order => 3;

        public void Enrich(HandlerContext context)
        {
            _tokenBuilders.First(t => t.Name == AuthorizationResponseParameters.IdToken).Build(context.Request.Data.GetScopesFromAuthorizationRequest(), context);
        }
    }
}
