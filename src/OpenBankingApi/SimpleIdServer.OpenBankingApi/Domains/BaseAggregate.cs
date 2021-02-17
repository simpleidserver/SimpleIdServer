using SimpleIdServer.OpenBankingApi.Common;
using System.Collections.Concurrent;

namespace SimpleIdServer.OpenBankingApi.Domains
{
    public abstract class BaseAggregate
    {   public BaseAggregate()
        {
            DomainEvents = new BlockingCollection<DomainEvent>();
        }

        public string AggregateId { get; set; }
        public int Version { get; set; }
        public BlockingCollection<DomainEvent> DomainEvents { get; set; }

        public abstract object Clone();

        public abstract void Handle(dynamic evt);
    }
}
