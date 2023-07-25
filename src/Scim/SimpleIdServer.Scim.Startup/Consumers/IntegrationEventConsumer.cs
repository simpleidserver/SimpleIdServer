using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Startup.Consumers
{
    public class IntegrationEventConsumer : IConsumer<RepresentationRefAttributeAddedEvent>, IConsumer<RepresentationRefAttributeRemovedEvent>, IConsumer<RepresentationRefAttributeUpdatedEvent>
    {
        public Task Consume(ConsumeContext<RepresentationRefAttributeAddedEvent> context)
        {
            Debug.WriteLine(context.Message.Id);
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RepresentationRefAttributeRemovedEvent> context)
        {
            Debug.WriteLine(context.Message.Id);
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RepresentationRefAttributeUpdatedEvent> context)
        {
            Debug.WriteLine(context.Message.Id);
            return Task.CompletedTask;
        }
    }
}
