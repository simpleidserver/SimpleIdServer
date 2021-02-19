using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences.InMemory
{
    public class InMemoryAccountAccessConsentRepository : IAccountAccessConsentCommandRepository
    {
        private readonly ConcurrentBag<AccountAccessConsentAggregate> _accountAccessConsents;

        public InMemoryAccountAccessConsentRepository(ConcurrentBag<AccountAccessConsentAggregate> accountAccessConsents)
        {
            _accountAccessConsents = accountAccessConsents;
        }

        public Task Add(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken)
        {
            _accountAccessConsents.Add(accountAccessConsent);
            return Task.CompletedTask;
        }

        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
