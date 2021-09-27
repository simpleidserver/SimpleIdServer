using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class RepresentationReferenceAttributeRemovedEventConsumer : IConsumer<RepresentationReferenceAttributeRemovedEvent>
    {
        public Task Consume(ConsumeContext<RepresentationReferenceAttributeRemovedEvent> context)
        {
            Console.WriteLine($"Attribute '{context.Message.AttributeFullPath}={string.Join(",", context.Message.Values)}' has beeen removed from {context.Message.RepresentationAggregateId}");
            return Task.CompletedTask;
        }
    }
}
