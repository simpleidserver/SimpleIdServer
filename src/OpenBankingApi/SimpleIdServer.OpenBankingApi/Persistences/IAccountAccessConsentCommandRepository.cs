using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences
{
    public interface IAccountAccessConsentCommandRepository
    {
        Task Add(AccountAccessConsentAggregate accountAccessConsent, CancellationToken cancellationToken);
        Task SaveChanges(CancellationToken cancellationToken);
    }
}
