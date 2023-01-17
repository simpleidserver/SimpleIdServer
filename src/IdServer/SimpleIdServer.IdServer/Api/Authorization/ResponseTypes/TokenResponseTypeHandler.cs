// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseTypes
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

        public Task Enrich(IEnumerable<string> scopes, IEnumerable<string> audiences, IEnumerable<AuthorizationRequestClaimParameter> claims, HandlerContext context, CancellationToken cancellationToken)
        {
            return _tokenBuilders.First(t => t.Name == AuthorizationResponseParameters.AccessToken).Build(scopes, audiences, claims, context, cancellationToken);
        }
    }
}