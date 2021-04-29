// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public string ResponseType => RESPONSE_TYPE;
        public int Order => 3;
        public static string RESPONSE_TYPE = "id_token";

        public Task Enrich(HandlerContext context, CancellationToken cancellationToken)
        {
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            return _tokenBuilders.First(t => t.Name == AuthorizationResponseParameters.IdToken).Build(scopes, context, cancellationToken);
        }
    }
}
