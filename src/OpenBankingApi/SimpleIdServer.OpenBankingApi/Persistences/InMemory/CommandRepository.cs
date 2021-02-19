using Microsoft.Extensions.Options;
using NEventStore;
using SimpleIdServer.OpenBankingApi.Domains;
using System;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Persistences.InMemory
{
    public class CommandRepository : ICommandRepository
    {
        private readonly IStoreEvents _eventStore;
        private readonly OpenBankingApiOptions _options;

        public CommandRepository(
            IStoreEvents eventStore, 
            IOptions<OpenBankingApiOptions> options)
        {
            _eventStore = eventStore;
            _options = options.Value;
        }

        public T GetLastAggregate<T>(string id) where T : BaseAggregate
        {
            var latestSnapshot = _eventStore.Advanced.GetSnapshot(id, int.MaxValue);
            using (var stream = _eventStore.OpenStream(id))
            {
                stream.CommittedEvents.Select(e => (DomainEvent)e.Body);
            }

            return null;
        }

        public void Commit(BaseAggregate aggregate)
        {
            using (var evtStream = _eventStore.CreateStream(aggregate.AggregateId))
            {
                foreach(var domainEvent in aggregate.DomainEvents)
                {
                    evtStream.Add(new EventMessage { Body = domainEvent });
                }

                evtStream.CommitChanges(Guid.NewGuid());
            }
        }
    }
}
