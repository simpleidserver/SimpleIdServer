// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.IdServer.IntegrationEvents;
using System.Threading.Tasks;
using cl = System.Console;

namespace SimpleIdServer.IdServer.Template.Startup.Consumers
{
    public class IdServerEventsConsumer : IConsumer<UserLoginSuccessEvent>
    {
        public Task Consume(ConsumeContext<UserLoginSuccessEvent> context)
        {
            cl.WriteLine("User is authenticated");
            return Task.CompletedTask;
        }
    }
}
