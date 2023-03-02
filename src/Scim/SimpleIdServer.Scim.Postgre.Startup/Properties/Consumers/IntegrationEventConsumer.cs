using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.SqlServer.Startup.Consumers
{
    public class IntegrationEventConsumer : IConsumer<RepresentationRefAttributeAddedEvent>
    {
        public Task Consume(ConsumeContext<RepresentationRefAttributeAddedEvent> context)
        {
            Debug.WriteLine(context.Message.Id);
            return Task.CompletedTask;
        }
    }
}
