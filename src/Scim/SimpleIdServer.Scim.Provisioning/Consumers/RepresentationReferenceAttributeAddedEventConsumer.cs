using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class RepresentationReferenceAttributeAddedEventConsumer : IConsumer<RepresentationReferenceAttributeAddedEvent>
    {
        public Task Consume(ConsumeContext<RepresentationReferenceAttributeAddedEvent> context)
        {
            Console.WriteLine($"Attribute '{context.Message.AttributeFullPath}={string.Join(",", context.Message.Values)}' has beeen added in {context.Message.RepresentationAggregateId}");
            return Task.CompletedTask;
        }
    }
}
