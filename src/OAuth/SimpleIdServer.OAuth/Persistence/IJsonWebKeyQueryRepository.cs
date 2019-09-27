// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface IJsonWebKeyQueryRepository
    {
        Task<List<JsonWebKey>> GetAllJsonWebKeys();
        Task<JsonWebKey> FindJsonWebKeyById(string kid);
        Task<List<JsonWebKey>> FindJsonWebKeys(Usages usage, string alg, KeyOperations[] operations);
    }
}