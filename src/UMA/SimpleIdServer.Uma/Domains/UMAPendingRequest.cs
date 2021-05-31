// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Domains
{
    public enum UMAPendingRequestStatus
    {
        TOBECONFIRMED = 0,
        CONFIRMED = 1,
        REJECTED = 2
    }

    public class UMAPendingRequest : ICloneable
    {
        public UMAPendingRequest(string ticketId, string owner, DateTime createDateTime)
        {
            TicketId = ticketId;
            Owner = owner;
            CreateDateTime = createDateTime;
            Scopes = new List<string>();
            Status = UMAPendingRequestStatus.TOBECONFIRMED;
        }

        public string TicketId { get; set; }
        public string Requester { get; set; }
        public string Owner { get; set; }
        public ICollection<string> Scopes { get; set; }
        public DateTime CreateDateTime { get; set; }
        public UMAPendingRequestStatus Status { get; set; }
        public UMAResource Resource { get; set; }

        public void Confirm()
        {
            Status = UMAPendingRequestStatus.CONFIRMED;
        }

        public void Reject()
        {
            Status = UMAPendingRequestStatus.REJECTED;
        }

        public object Clone()
        {
            return new UMAPendingRequest(TicketId, Owner, CreateDateTime)
            {
                Requester = Requester,
                Scopes = Scopes.ToList(),
                Resource = (UMAResource)Resource.Clone(),
                Status = Status
            };
        }
    }
}
