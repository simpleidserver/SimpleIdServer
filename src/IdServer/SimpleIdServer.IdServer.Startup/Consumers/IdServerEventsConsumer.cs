// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.IdServer.Events;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Consumers
{
    public class IdServerEventsConsumer : IConsumer<UserLoginSuccessEvent>
    {
        public Task Consume(ConsumeContext<UserLoginSuccessEvent> context)
        {
            Console.WriteLine("User is authenticated");
            return Task.CompletedTask;
        }
    }
}
