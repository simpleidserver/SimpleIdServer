// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Provisioning
{
    public class ExtractUsersFaultConsumer : IConsumer<Fault<ExtractUsersCommand>>
    {
        public ExtractUsersFaultConsumer()
        {
            
        }

        public Task Consume(ConsumeContext<Fault<ExtractUsersCommand>> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
