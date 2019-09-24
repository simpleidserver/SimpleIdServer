// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Domains.Jwks
{
    public class DefaultJsonWebKeyCommandRepository : InMemoryCommandRepository<JsonWebKey>, IJsonWebKeyCommandRepository
    {
        public DefaultJsonWebKeyCommandRepository(List<JsonWebKey> jwks) : base(jwks)
        {
        }
    }
}