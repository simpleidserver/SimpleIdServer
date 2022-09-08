using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.SqlServer.Startup.Consumers
{
    public class IntegrationEventConsumer : IConsumer<RepresentationReferenceAttributeAddedEvent>
    {
        public Task Consume(ConsumeContext<RepresentationReferenceAttributeAddedEvent> context)
        {
            Debug.WriteLine(context.Message.RepresentationSerialized);
            return Task.CompletedTask;
        }
    }
}
