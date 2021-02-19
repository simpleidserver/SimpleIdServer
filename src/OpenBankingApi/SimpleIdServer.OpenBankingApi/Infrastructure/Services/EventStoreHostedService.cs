using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Infrastructure.Services
{
    public class EventStoreHostedService : IHostedService
    {
        private readonly IJob _eventStoreJob;

        public EventStoreHostedService(IJob eventStoreJob)
        {
            _eventStoreJob = eventStoreJob;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _eventStoreJob.Start();
            return Task.FromResult(true);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _eventStoreJob.Stop();
            return Task.FromResult(true);
        }
    }
}
