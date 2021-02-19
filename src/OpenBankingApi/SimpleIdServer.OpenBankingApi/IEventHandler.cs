using SimpleIdServer.OpenBankingApi.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi
{
    public interface IEventHandler<T> where T : DomainEvent
    {
        Task Handle(T evt, CancellationToken cancellationToken);
    }
}
