// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IAuthenticationContextClassReferenceRepository
{
    Task<AuthenticationContextClassReference> Get(string realm, string id, CancellationToken cancellationToken);
    Task<AuthenticationContextClassReference> GetByName(string realm, string name, CancellationToken cancellationToken);
    Task<List<AuthenticationContextClassReference>> GetAll(CancellationToken cancellationToken);
    Task<List<AuthenticationContextClassReference>> GetAll(string realm, CancellationToken cancellationToken);
    void Add(AuthenticationContextClassReference record);
    void Delete(AuthenticationContextClassReference record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}