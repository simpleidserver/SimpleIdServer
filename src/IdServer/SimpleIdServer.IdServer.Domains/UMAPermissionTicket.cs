// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UMAPermissionTicket
    {
        public UMAPermissionTicket() { }

        public UMAPermissionTicket(string id, ICollection<UMAPermissionTicketRecord> records)
        {
            Id = id;
            Records = records;
        }

        public string Id { get; set; } = null!;
        public ICollection<UMAPermissionTicketRecord> Records { get; set; } = new List<UMAPermissionTicketRecord>();
    }
}
