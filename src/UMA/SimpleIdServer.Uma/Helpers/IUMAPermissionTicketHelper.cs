using SimpleIdServer.Uma.Domains;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Helpers
{
    public interface IUMAPermissionTicketHelper
    {
        Task<UMAPermissionTicket> GetTicket(string ticketId);
        Task<bool> SetTicket(UMAPermissionTicket umaPermissionTicket);
    }
}
