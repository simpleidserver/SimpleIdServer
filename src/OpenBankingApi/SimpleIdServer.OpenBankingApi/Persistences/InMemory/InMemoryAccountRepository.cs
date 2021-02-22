using SimpleIdServer.OpenBankingApi.Domains.Account;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences.InMemory
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        private readonly ConcurrentBag<AccountAggregate> _accounts;

        public InMemoryAccountRepository(ConcurrentBag<AccountAggregate> accounts)
        {
            _accounts = accounts;
        }

        public Task<IEnumerable<AccountAggregate>> GetBySubject(string subject, CancellationToken cancellationToken)
        {
            return Task.FromResult(_accounts.Where(_ => _.Subject == subject));
        }

        public Task<AccountAggregate> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.AggregateId == id));
        }
    }
}
