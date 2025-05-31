# Integration events

A SCIM server publishes integration events on a messaging bus, which client applications can consume for internal purposes—for example, to duplicate user data in a separate database. This article explores the different integration events, how to configure the event bus with MassTransit, and techniques for handling large event payloads.

## Capturing Integration Events by Default

By default, integration events can be captured by editing the `SCIMEvents` properties available in the SCIM server options. 
To listen to events, edit the `Program.cs` file and configure the SCIM server as follows:

```
AddScim(o =>
{
    o.SCIMEvents = new SCIMHostEvents
    {
        RepresentationAdded = (r) =>
        {
            // Handle the RepresentationAdded event
        }
    };
})
```

In this example, when a new user or group representation is added, the `RepresentationAdded` delegate will be invoked with the representation instance (r) as its parameter. You can similarly assign delegates for other event types, such as RepresentationRemoved, RepresentationUpdated, and the various reference‑attribute events.

## Integration Events Overview

The server emits several integration events when user or group representations are added, updated, or removed, as well as when relationships between representations change. The list of integration events includes:

| Event | Description |
| ----- | ----------- |
| RepresentationAddedEvent | A representation (user or group) has been added |
| RepresentationRemovedEvent | A representation (user or group) has been removed |
| RepresentationUpdatedEvent | A representation (user or group) has been updated |
| RepresentationRefAttributeAddedEvent | A reference has been added, for example, a user has been added to a group |
| RepresentationRefAttributeRemovedEvent | A reference has been removed, for example, a user has been removed from a group |
| RepresentationRefAttributeUpdatedEvent | A reference has been updated, for example, a user has an indirect rather than a direct relation with a group |

## Masstransit

Integration events can be published using the MassTransit library. To enable this functionality, install the NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.MassTransit
```

Once the package is installed, MassTransit will automatically pick up SCIM integration events and forward them to the configured message bus.

The MassTransit library is used to transmit integration events over a messaging bus. 
Any transport technology supported by MassTransit can be configured. The configuration is done by calling the `ConfigureMassTransit` method. For a list of supported bus transports, refer to the [official documentation](https://masstransit.io/documentation/transports).

An example of configuring MassTransit in your `Program.cs` file is as follows:

```csharp title="Program.cs"
ConfigureMassTransit(configurator =>  {
    configurator.AddConsumer<IntegrationEventConsumer>();
    configurator.UsingInMemory((context, cfg) =>
    {
        cfg.UseMessageData(repository);
        cfg.ConfigureEndpoints(context);
    });
})
```

### Handling Large Integration Events

Sometimes the size of integration events may be too large for direct transmission on the bus. 
In these cases, the payload must be stored in a dedicated storage area. 
MassTransit provides an implementation of the `IMessageDataRepository` interface to solve this issue. The following table lists different storage options provided through NuGet packages:

| Nuget package | Description | Code source |
| ------------- | ---- | ----------- |
| Masstransit | In memory | `new InMemoryMessageDataRepository()` |
| MassTransit | In a directory | `new FileSystemMessageDataRepository(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory))` |
| MassTransit.Azure.Storage | Azure blob storage | `new BlobServiceClient(conf.ConnectionString).CreateMessageDataRepository("message-data")` |

Once the repository is instantiated, you can configure MassTransit for large messages with the following code:

```csharp title="Program.cs"
ConfigureMassTransit(configurator =>  {
    IMessageDataRepository repository = ....
    configurator.AddConsumer<BigMessageConsumer>();
    configurator.UsingInMemory((context, cfg) =>
    {
        cfg.UseMessageData(repository);
        cfg.ConfigureEndpoints(context);
    });
}).PublishLargeMessage()
```

When handling large events, a specialized consumer like the `BigMessageConsumer` class is used. 
This consumer is capable of processing large integration events by retrieving the full payload from the storage repository. Below is the source code for the BigMessageConsumer class:

```csharp title="BigMessageConsumer.cs"
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
```

This consumer retrieves the payload from the repository, identifies the event type based on its name, and then deserializes the payload accordingly.