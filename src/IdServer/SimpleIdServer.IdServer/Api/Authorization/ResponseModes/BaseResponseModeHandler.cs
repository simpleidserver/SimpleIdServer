// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Jwt;
using System.Linq;
using System.Threading;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public class BaseResponseModeHandler
    {
        private IJwtBuilder _jwtBuilder;

        public BaseResponseModeHandler(IJwtBuilder jwtBuilder)
        {
            _jwtBuilder = jwtBuilder;
        }

        protected string BuildJWT(RedirectURLAuthorizationResponse response, HandlerContext context)
        {
            var claims = response.QueryParameters.ToDictionary(q => q.Key, q => (object)q.Value);
            var descriptor = new SecurityTokenDescriptor
            {
                Claims = claims
            };
            var jwt = _jwtBuilder.BuildClientToken(context.Realm, context.Client, descriptor, context.Client.AuthorizationSignedResponseAlg ?? SecurityAlgorithms.RsaSha256, context.Client.AuthorizationEncryptedResponseAlg, context.Client.AuthorizationEncryptedResponseEnc, CancellationToken.None).Result;
            return jwt;
        }
    }
}
