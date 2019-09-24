// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Jwt;

namespace SimpleIdServer.OAuth.Api.Token.TokenProfiles
{
    public class BearerTokenProfile : ITokenProfile
    {
        private readonly IJwtBuilder _jwtBuilder;

        public BearerTokenProfile(IJwtBuilder jwtBuilder)
        {
            _jwtBuilder = jwtBuilder;
        }

        public string Profile => DEFAULT_NAME;
        public static string DEFAULT_NAME = "Bearer";

        public void Enrich(HandlerContext context)
        {
            context.Response.Add(TokenResponseParameters.TokenType, Profile);
        }
    }
}