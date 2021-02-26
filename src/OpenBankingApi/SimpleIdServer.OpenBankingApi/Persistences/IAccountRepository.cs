using SimpleIdServer.OpenBankingApi.Domains.Account;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences
{
    public interface IAccountRepository
    {
        Task<IEnumerable<AccountAggregate>> GetBySubject(string subject, CancellationToken cancellationToken);
        Task<AccountAggregate> Get(string id, CancellationToken cancellationToken);
        Task<IEnumerable<AccountAggregate>> Get(IEnumerable<string> ids, CancellationToken cancellationToken);
    }
}
