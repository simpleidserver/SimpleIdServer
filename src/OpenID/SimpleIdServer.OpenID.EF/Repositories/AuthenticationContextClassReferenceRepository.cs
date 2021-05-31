// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.EF.Repositories
{
    public class AuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
    {
        private readonly OpenIdDBContext _dbContext;

        public AuthenticationContextClassReferenceRepository(OpenIdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AuthenticationContextClassReference>> FindACRByNames(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            IEnumerable<AuthenticationContextClassReference> result = await _dbContext.Acrs.Where(a => names.Contains(a.Name)).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<ICollection<AuthenticationContextClassReference>> GetAllACR(CancellationToken token)
        {
            ICollection<AuthenticationContextClassReference> result = await _dbContext.Acrs.ToListAsync(token);
            return result;
        }

        public Task<bool> Add(AuthenticationContextClassReference data, CancellationToken token)
        {
            _dbContext.Acrs.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(AuthenticationContextClassReference data, CancellationToken token)
        {
            _dbContext.Acrs.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(AuthenticationContextClassReference data, CancellationToken token)
        {
            _dbContext.Acrs.Update(data);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}
