// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence
{
    public interface IAuthenticationContextClassReferenceRepository : ICommandRepository<AuthenticationContextClassReference>
    {
        Task<IEnumerable<AuthenticationContextClassReference>> FindACRByNames(IEnumerable<string> names, CancellationToken cancellationToken);
        Task<ICollection<AuthenticationContextClassReference>> GetAllACR(CancellationToken token);
    }
}
