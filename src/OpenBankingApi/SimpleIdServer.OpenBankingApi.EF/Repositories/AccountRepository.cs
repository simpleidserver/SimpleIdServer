// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Persistences;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.EF.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly OpenBankingDbContext _dbContext;

        public AccountRepository(OpenBankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<AccountAggregate> Get(string id, CancellationToken cancellationToken)
        {
            return GetAccounts().FirstOrDefaultAsync(a => a.AggregateId == id, cancellationToken);
        }

        public async Task<IEnumerable<AccountAggregate>> Get(IEnumerable<string> ids, CancellationToken cancellationToken)
        {
            IEnumerable<AccountAggregate> result = await GetAccounts().Where(a => ids.Contains(a.AggregateId)).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<AccountAggregate>> Get(IEnumerable<string> ids, string subject, CancellationToken cancellationToken)
        {
            IEnumerable<AccountAggregate> result = await GetAccounts().Where(a => ids.Contains(a.AggregateId) && a.Subject == subject).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<AccountAggregate>> GetBySubject(string subject, CancellationToken cancellationToken)
        {
            IEnumerable<AccountAggregate> result = await GetAccounts().Where(_ => _.Subject == subject).ToListAsync(cancellationToken);
            return result;
        }

        private IQueryable<AccountAggregate> GetAccounts()
        {
            return _dbContext.Accounts.Include(a => a.Accounts)
                .Include(a => a.Servicer);
        }
    }
}
