// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class UMAPendingRequestBuilder
    {
        private readonly UMAPendingRequest _pendingRequest;

        private UMAPendingRequestBuilder(string ticketId, string requester, string owner, UMAResource resource)
        {
            _pendingRequest= new UMAPendingRequest
            {
                TicketId = ticketId,
                Requester = requester,
                Owner = owner,
                Resource = resource
            };
        }

        public static UMAPendingRequestBuilder Create(string ticketId, string requester, string owner, UMAResource resource)
        { 
            var result = new UMAPendingRequestBuilder(ticketId, requester, owner, resource);
            return result;
        }

        public UMAPendingRequest Build() => _pendingRequest;
    }
}
