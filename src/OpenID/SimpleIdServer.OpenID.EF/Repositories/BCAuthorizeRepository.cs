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
    public class BCAuthorizeRepository : IBCAuthorizeRepository
    {
        private readonly OpenIdDBContext _dbContext;

        public BCAuthorizeRepository(OpenIdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(BCAuthorize bcAuthorize, CancellationToken cancellationToken)
        {
            _dbContext.BCAuthorizeLst.Add(bcAuthorize);
            return Task.CompletedTask;
        }

        public Task Delete(BCAuthorize bcAuthorize, CancellationToken cancellationToken)
        {
            _dbContext.BCAuthorizeLst.Remove(bcAuthorize);
            return Task.CompletedTask;
        }

        public Task Update(BCAuthorize bcAuhtorize, CancellationToken cancellationToken)
        {
            _dbContext.BCAuthorizeLst.Update(bcAuhtorize);
            return Task.CompletedTask;
        }

        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<BCAuthorize> Get(string id, CancellationToken cancellationToken)
        {
            return _dbContext.BCAuthorizeLst.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<BCAuthorize>> GetConfirmedAuthorizationRequest(CancellationToken cancellationToken)
        {
            IEnumerable<BCAuthorize>  result = await _dbContext.BCAuthorizeLst.Where(b => b.Status == BCAuthorizeStatus.Confirmed).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<BCAuthorize>> GetNotSentRejectedAuthorizationRequest(CancellationToken cancellationToken)
        {
            IEnumerable<BCAuthorize> result = await _dbContext.BCAuthorizeLst.Where(b => b.Status == BCAuthorizeStatus.Rejected && b.RejectionSentDateTime == null).ToListAsync(cancellationToken);
            return result;
        }
    }
}
