// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UMAPermissionTicketRecord
    {
        public UMAPermissionTicketRecord() { }

        public UMAPermissionTicketRecord(string resourceId, ICollection<string> scopes)
        {
            ResourceId = resourceId;
            Scopes = scopes;
        }

        public string ResourceId { get; set; } = null!;
        public ICollection<string> Scopes { get; set; } = new List<string>(); 
    }
}
