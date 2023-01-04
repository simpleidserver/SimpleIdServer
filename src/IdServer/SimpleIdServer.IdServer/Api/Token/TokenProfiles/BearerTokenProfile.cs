// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;

namespace SimpleIdServer.IdServer.Api.Token.TokenProfiles
{
    public class BearerTokenProfile : ITokenProfile
    {
        public string Profile => DEFAULT_NAME;
        public const string DEFAULT_NAME = "Bearer";

        public void Enrich(HandlerContext context)
        {
            context.Response.Add(TokenResponseParameters.TokenType, Profile);
        }
    }
}