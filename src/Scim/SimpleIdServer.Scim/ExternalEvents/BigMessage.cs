// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class BigMessage
    {
        public string Name { get; set; }
        public MessageData<byte[]> Payload { get; set; }
    }
}
