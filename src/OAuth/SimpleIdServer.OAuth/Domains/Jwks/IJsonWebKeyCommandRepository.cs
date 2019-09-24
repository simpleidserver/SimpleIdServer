// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;

namespace SimpleIdServer.OAuth.Domains.Jwks
{
    public interface IJsonWebKeyCommandRepository : ICommandRepository<JsonWebKey>
    {
    }
}