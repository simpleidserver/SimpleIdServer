using System;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ICommandRepository<T> : IDisposable
    {
        bool Add(T data);
        bool Update(T data);
        bool Delete(T data);
        Task<int> SaveChanges();
    }
}
