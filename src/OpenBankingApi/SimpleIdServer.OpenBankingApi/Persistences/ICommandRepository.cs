using SimpleIdServer.OpenBankingApi.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Persistences
{
    public interface ICommandRepository
    {
        T GetLastAggregate<T>(string id) where T : BaseAggregate;
        Task Commit(BaseAggregate aggregate, CancellationToken cancellationToken);
    }
}
