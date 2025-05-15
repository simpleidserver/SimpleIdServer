// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IUmaPermissionTicketHelper
    {
        Task<UMAPermissionTicket> GetTicket(string ticketId);
        Task<bool> SetTicket(UMAPermissionTicket umaPermissionTicket, CancellationToken cancellationToken);
    }

    public class UMAPermissionTicketHelper : IUmaPermissionTicketHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IdServerHostOptions _options;


        public UMAPermissionTicketHelper(IDistributedCache distributedCache, IOptions<IdServerHostOptions> options)
        {
            _distributedCache = distributedCache;
            _options = options.Value;
        }

        public async Task<UMAPermissionTicket> GetTicket(string ticketId)
        {
            var payload = await _distributedCache.GetAsync(ticketId);
            if (payload == null)
                return null;

            return JsonConvert.DeserializeObject<UMAPermissionTicket>(Encoding.UTF8.GetString(payload));
        }

        public async Task<bool> SetTicket(UMAPermissionTicket umaPermissionTicket, CancellationToken cancellationToken)
        {
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(umaPermissionTicket));
            await _distributedCache.SetAsync(umaPermissionTicket.Id, payload, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(_options.UmaValidityPeriodPermissionTicketInSeconds)
            }, cancellationToken);
            return true;
        }
    }
}
