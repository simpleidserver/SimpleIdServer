using Website.Models;

namespace Website.Stores;

public interface IBankInfoStore
{
    IQueryable<BankInfo> GetAll();
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class BankInfoStore : IBankInfoStore
{
    private readonly ICollection<BankInfo> _bankInfos;

    public BankInfoStore(ICollection<BankInfo> bankInfos)
    {
        _bankInfos = bankInfos;
    }

    public IQueryable<BankInfo> GetAll() => _bankInfos.AsQueryable();

    public Task<int> SaveChanges(CancellationToken cancellationToken) => Task.FromResult(1);
}
