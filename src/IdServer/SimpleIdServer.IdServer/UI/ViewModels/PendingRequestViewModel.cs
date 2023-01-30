// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class PendingRequestViewModel
    {
        public string TicketId { get; set; }
        public string Requester { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public ICollection<string> Scopes { get; set; }
        public UMAPendingRequestStatus Status { get; set; }
        public string Owner { get; set; }
    }
}
