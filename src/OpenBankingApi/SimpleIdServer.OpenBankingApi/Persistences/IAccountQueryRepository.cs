using SimpleIdServer.OpenBankingApi.Domains.Account;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences
{
    public interface IAccountQueryRepository
    {
        Task<AccountAggregate> Get(string id);
    }
}
