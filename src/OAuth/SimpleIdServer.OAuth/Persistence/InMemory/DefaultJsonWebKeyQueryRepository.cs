// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task<List<JsonWebKey>> GetActiveJsonWebKeys()
        {
            var currentDateTime = DateTime.UtcNow;
            var result = _jsonWebKeys.Where(j => j.ExpirationDateTime == null || currentDateTime < j.ExpirationDateTime).Select(j => (JsonWebKey)j.Clone()).ToList();
            return Task.FromResult(result);
        }

        public Task<List<JsonWebKey>> GetNotRotatedJsonWebKeys(CancellationToken cancellationToken)
        {
            var result = _jsonWebKeys.Where(j => string.IsNullOrWhiteSpace(j.RotationJWKId)).Select(j => (JsonWebKey)j.Clone()).ToList();
            return Task.FromResult(result);
        }

        public Task<JsonWebKey> FindJsonWebKeyById(string kid)
        {
            var result = _jsonWebKeys.FirstOrDefault(j => j.Kid == kid);
            return Task.FromResult(result == null ? null : (JsonWebKey)result.Clone());
        }

        public Task<List<JsonWebKey>> FindJsonWebKeys(Usages usage, string alg, KeyOperations[] operations)
        {
            var result = _jsonWebKeys.Where(j => j.Use == usage && j.Alg == alg && operations.All(o => j.KeyOps.Contains(o))).Select(j => (JsonWebKey)j.Clone()).ToList();
            return Task.FromResult(result);
        }
    }
}
