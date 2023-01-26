// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public enum UMAPendingRequestStatus
    {
        TOBECONFIRMED = 0,
        CONFIRMED = 1,
        REJECTED = 2
    }

    public class UMAPendingRequest
    {
        public UMAPendingRequest() { }

        public UMAPendingRequest(string ticketId, string owner, DateTime createDateTime)
        {
            TicketId = ticketId;
            Owner = owner;
            CreateDateTime = createDateTime;
            Scopes = new List<string>();
            Status = UMAPendingRequestStatus.TOBECONFIRMED;
        }

        public string TicketId { get; set; } = null!;
        public string? Requester { get; set; } = null;
        public string Owner { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }
        public UMAPendingRequestStatus Status { get; set; }
        public UMAResource Resource { get; set; }
        public ICollection<string> Scopes { get; set; } = new List<string>();

        public void Confirm()
        {
            Status = UMAPendingRequestStatus.CONFIRMED;
        }

        public void Reject()
        {
            Status = UMAPendingRequestStatus.REJECTED;
        }
    }
}
