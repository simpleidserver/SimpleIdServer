﻿using MassTransit;
using Newtonsoft.Json;
using SimpleIdServer.Scim.ExternalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Startup.Consumers
{
    public class BigMessageConsumer : IConsumer<BigMessage>
    {
        private static List<Type> _types = new List<Type>
        {
            typeof(RepresentationAddedEvent),
            typeof(RepresentationRemovedEvent),
            typeof(RepresentationUpdatedEvent),
            typeof(RepresentationRefAttributeAddedEvent),
            typeof(RepresentationRefAttributeRemovedEvent),
            typeof(RepresentationRefAttributeUpdatedEvent)
        };


        public async Task Consume(ConsumeContext<BigMessage> context)
        {
            var bigPayload = await context.Message.Payload.Value;
            var type = _types.Single(t => t.Name == context.Message.Name);
            var message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bigPayload), type);
            // CONTINUE...
        }
    }
}
