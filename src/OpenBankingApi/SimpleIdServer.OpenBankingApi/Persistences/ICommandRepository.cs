using SimpleIdServer.OpenBankingApi.Domains;

namespace SimpleIdServer.OpenBankingApi.Persistences
{
    public interface ICommandRepository
    {
        T GetLastAggregate<T>(string id) where T : BaseAggregate;
        void Commit(BaseAggregate aggregate);
    }
}
