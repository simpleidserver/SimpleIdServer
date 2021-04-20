using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences.InMemory
{
    public class InMemoryAccountAccessConsentRepository : IAccountAccessConsentRepository
    {
        private readonly ConcurrentBag<AccountAccessConsentAggregate> _accountAccessConsents;

        public InMemoryAccountAccessConsentRepository(ConcurrentBag<AccountAccessConsentAggregate> accountAccessConsents)
        {
            _accountAccessConsents = accountAccessConsents;
        }

        public Task<AccountAccessConsentAggregate> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_accountAccessConsents.FirstOrDefault(_ => _.AggregateId == id));
        }

        public Task<IEnumerable<AccountAccessConsentAggregate>> GetAwaitingAuthorisationAccountAccessConsents(string clientId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_accountAccessConsents.Where(a => a.ClientId == clientId && a.Status == AccountAccessConsentStatus.AwaitingAuthorisation));
        }

        public Task Add(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken)
        {
            _accountAccessConsents.Add(accountAccessConsent);
            return Task.CompletedTask;
        }
        public Task Update(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken)
        {
            var record = _accountAccessConsents.First(_ => _.AggregateId == accountAccessConsent.AggregateId);
            _accountAccessConsents.Remove(record);
            _accountAccessConsents.Add(accountAccessConsent);
            return Task.CompletedTask;
        }

        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
