using NEventStore;
using SimpleIdServer.OpenBankingApi.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.OpenBankingApi.Persistences.InMemory
{
    public class CommandRepository : ICommandRepository
    {
        private readonly IStoreEvents _eventStore;
        private readonly IServiceProvider _serviceProvider;

        public CommandRepository(
            IStoreEvents eventStore, 
            IServiceProvider serviceProvider)
        {
            _eventStore = eventStore;
            _serviceProvider = serviceProvider;
        }

        public T GetLastAggregate<T>(string id) where T : BaseAggregate
        {
            // var latestSnapshot = _eventStore.Advanced.GetSnapshot(id, int.MaxValue);
            using (var stream = _eventStore.OpenStream(id))
            {
                var domainEvents = stream.CommittedEvents.Select(e => (DomainEvent)e.Body);
                var instance = Construct<T>();
                foreach(var domainEvt in domainEvents)
                {
                    instance.Handle(domainEvt);
                }

                return instance;
            }
        }

        public async Task Commit(BaseAggregate aggregate, CancellationToken cancellationToken)
        {
            using (var evtStream = _eventStore.OpenStream(aggregate.AggregateId))
            {
                foreach (var domainEvent in aggregate.DomainEvents)
                {
                    evtStream.Add(new EventMessage { Body = domainEvent });
                }

                evtStream.CommitChanges(Guid.NewGuid());
            }

            await Commit(aggregate.DomainEvents, cancellationToken);
        }

        public static T Construct<T>()
        {
            Type t = typeof(T);
            var ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { }, null);
            return (T)ci.Invoke(new object[] { });
        }

        private async Task Commit(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var domainEvt in domainEvents)
                {
                    var genericType = domainEvt.GetType();
                    var messageBrokerType = typeof(IEventHandler<>).MakeGenericType(genericType);
                    var lst = (IEnumerable<object>)_serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(messageBrokerType));
                    foreach (var r in lst)
                    {
                        var task = (Task)messageBrokerType.GetMethod("Handle").Invoke(r, new object[] { domainEvt, cancellationToken});
                        await task;
                    }
                }

                transaction.Complete();
            }
        }
    }
}
