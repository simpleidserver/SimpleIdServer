using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences
{
    public interface IAccountAccessConsentRepository
    {
        Task<AccountAccessConsentAggregate> Get(string id, CancellationToken cancellationToken);
        Task<IEnumerable<AccountAccessConsentAggregate>> GetAwaitingAuthorisationAccountAccessConsents(string clientId, CancellationToken cancellationToken);
        Task Update(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken);
        Task Add(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken);
        Task SaveChanges(CancellationToken cancellationToken);
    }
}
