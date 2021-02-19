using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEventStore;
using NEventStore.PollingClient;
using SimpleIdServer.OpenBankingApi.Domains;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static NEventStore.PollingClient.PollingClient2;

namespace SimpleIdServer.OpenBankingApi
{
    public class EventStoreJob : IJob
    {
        private readonly IStoreEvents _storeEvents;
        private readonly OpenBankingApiOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventStoreJob> _logger;
        private PollingClient2 _client;

        public EventStoreJob(
            IStoreEvents storeEvents,
            IServiceProvider serviceProvider,
            IOptions<OpenBankingApiOptions> options,
            ILogger<EventStoreJob> logger)
        {
            _storeEvents = storeEvents;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        public void Start()
        {
            _client = new PollingClient2(_storeEvents.Advanced, Handle, _options.WaitInterval);
            _client.StartFrom();
        }

        public void Stop()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }

        private HandlingResult Handle(ICommit commit)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var evt in commit.Events)
                    {
                        var domainEvt = evt.Body as DomainEvent;
                        var genericType = domainEvt.GetType();
                        var messageBrokerType = typeof(IEventHandler<>).MakeGenericType(genericType);
                        var lst = (IEnumerable<object>)_serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(messageBrokerType));
                        foreach (var r in lst)
                        {
                            var task = (Task)messageBrokerType.GetMethod("Handle").Invoke(r, new object[] { domainEvt, CancellationToken.None });
                            task.Wait();
                        }
                    }

                    transaction.Complete();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return HandlingResult.Retry;
            }

            return HandlingResult.MoveToNext;
        }
    }
}
