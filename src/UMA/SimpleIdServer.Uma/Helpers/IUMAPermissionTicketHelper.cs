// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
