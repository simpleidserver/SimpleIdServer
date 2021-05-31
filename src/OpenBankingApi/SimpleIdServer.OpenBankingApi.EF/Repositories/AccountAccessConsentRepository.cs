// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Persistences;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.EF.Repositories
{
    public class AccountAccessConsentRepository : IAccountAccessConsentRepository
    {
        private readonly OpenBankingDbContext _dbContext;

        public AccountAccessConsentRepository(OpenBankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken)
        {
            _dbContext.AccountAccessConsents.Add(accountAccessConsent);
            return Task.CompletedTask;
        }

        public Task<AccountAccessConsentAggregate> Get(string id, CancellationToken cancellationToken)
        {
            return _dbContext.AccountAccessConsents.FirstOrDefaultAsync(c => c.AggregateId == id, cancellationToken);
        }

        public async Task<IEnumerable<AccountAccessConsentAggregate>> GetAwaitingAuthorisationAccountAccessConsents(string clientId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.AccountAccessConsents.Where(a => a.ClientId == clientId && a.Status == AccountAccessConsentStatus.AwaitingAuthorisation).ToListAsync(cancellationToken);
            return result;
        }

        public Task Update(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken)
        {
            _dbContext.AccountAccessConsents.Update(accountAccessConsent);
            return Task.CompletedTask;
        }

        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
