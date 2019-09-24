// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token
{
    public interface ITokenRequestHandler
    {
        Task<JObject> Handle(HandlerContext context);
    }

    public class TokenRequestHandler : ITokenRequestHandler
    {
        private readonly IEnumerable<IGrantTypeHandler> _handlers;

        public TokenRequestHandler(IEnumerable<IGrantTypeHandler> handlers)
        {
            _handlers = handlers;
        }

        public Task<JObject> Handle(HandlerContext context)
        {
            var handler = _handlers.FirstOrDefault(h => h.GrantType == context.Request.HttpBody.GetGrantType());
            if (handler == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_GRANT, ErrorMessages.BAD_GRANT_TYPE);
            }

            return handler.Handle(context);
        }
    }
}