using System.Collections.Generic;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAPermissionTicket
    {
        public UMAPermissionTicket(string id, ICollection<UMAPermissionTicketRecord> records)
        {
            Id = id;
            Records = records;
        }

        public string Id { get; set; }
        public ICollection<UMAPermissionTicketRecord> Records { get; set; }
    }
}
