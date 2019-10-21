using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAPendingRequest : ICloneable
    {
        public UMAPendingRequest(string ticketId, string owner, DateTime createDateTime)
        {
            TicketId = ticketId;
            Owner = owner;
            CreateDateTime = createDateTime;
            Scopes = new List<string>();
        }

        public string TicketId { get; set; }
        public string Requester { get; set; }
        public UMAResource Resource { get; set; }
        public string Owner { get; set; }
        public ICollection<string> Scopes { get; set; }
        public DateTime CreateDateTime { get; set; }

        public object Clone()
        {
            return new UMAPendingRequest(TicketId, Owner, CreateDateTime)
            {
                Requester = Requester,
                Scopes = Scopes.ToList(),
                Resource = (UMAResource)Resource.Clone()
            };
        }
    }
}
