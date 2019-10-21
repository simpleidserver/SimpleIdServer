using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleIdServer.Uma.Domains;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Helpers
{
    public class UMAPermissionTicketHelper : IUMAPermissionTicketHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly UMAHostOptions _umaHostOptions;


        public UMAPermissionTicketHelper(IDistributedCache distributedCache, IOptions<UMAHostOptions> umaHostOptions)
        {
            _distributedCache = distributedCache;
            _umaHostOptions = umaHostOptions.Value;
        }

        public async Task<UMAPermissionTicket> GetTicket(string ticketId)
        {
            var payload = await _distributedCache.GetAsync(ticketId);
            if (payload == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<UMAPermissionTicket>(Encoding.UTF8.GetString(payload));
        }

        public async Task<bool> SetTicket(UMAPermissionTicket umaPermissionTicket)
        {
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(umaPermissionTicket));
            await _distributedCache.SetAsync(umaPermissionTicket.Id, payload, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(_umaHostOptions.ValidityPeriodPermissionTicketInSeconds)
            });
            return true;
        }
    }
}
