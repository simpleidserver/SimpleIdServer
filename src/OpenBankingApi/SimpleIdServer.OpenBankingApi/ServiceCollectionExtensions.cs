using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Persistences.InMemory;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBankingApi(this IServiceCollection services)
        {
            var accounts = new ConcurrentBag<AccountAggregate>();
            services.AddSingleton<IAccountQueryRepository>(new InMemoryAccountQueryRepository(accounts));
            return services;
        }
    }
}
