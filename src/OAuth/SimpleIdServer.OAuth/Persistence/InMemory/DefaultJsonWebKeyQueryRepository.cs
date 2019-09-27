// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultJsonWebKeyQueryRepository : IJsonWebKeyQueryRepository
    {
        private List<JsonWebKey> _jsonWebKeys;

        public DefaultJsonWebKeyQueryRepository(List<JsonWebKey> jsonWebKeys)
        {
            _jsonWebKeys = jsonWebKeys;
        }

        public Task<List<JsonWebKey>> GetAllJsonWebKeys()
        {
            return Task.FromResult(_jsonWebKeys);
        }

        public Task<JsonWebKey> FindJsonWebKeyById(string kid)
        {
            return Task.FromResult(_jsonWebKeys.FirstOrDefault(j => j.Kid == kid));
        }

        public Task<List<JsonWebKey>> FindJsonWebKeys(Usages usage, string alg, KeyOperations[] operations)
        {
            var result = _jsonWebKeys.Where(j => j.Use == usage && j.Alg == alg && operations.All(o => j.KeyOps.Contains(o))).ToList();
            return Task.FromResult(result);
        }
    }
}
