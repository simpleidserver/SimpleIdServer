using SimpleIdServer.OpenBankingApi.Domains.Account;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences.InMemory
{
    public class InMemoryAccountQueryRepository : IAccountQueryRepository
    {
        private readonly ConcurrentBag<AccountAggregate> _accounts;

        public InMemoryAccountQueryRepository(ConcurrentBag<AccountAggregate> accounts)
        {
            _accounts = accounts;
        }

        public Task<AccountAggregate> Get(string id)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.AggregateId == id));
        }
    }
}
