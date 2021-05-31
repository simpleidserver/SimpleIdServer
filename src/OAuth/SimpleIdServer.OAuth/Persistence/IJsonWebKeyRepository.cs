// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface IJsonWebKeyRepository : ICommandRepository<JsonWebKey>
    {
        Task<List<JsonWebKey>> GetActiveJsonWebKeys(CancellationToken cancellationToken);
        Task<List<JsonWebKey>> GetNotRotatedJsonWebKeys(CancellationToken cancellationToken);
        Task<JsonWebKey> FindJsonWebKeyById(string kid, CancellationToken cancellationToken);
        Task<List<JsonWebKey>> FindJsonWebKeys(Usages usage, string alg, KeyOperations[] operations, CancellationToken cancellationToken);
    }
}