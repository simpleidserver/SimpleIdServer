// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.ExternalEvents;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers;

public interface IBusHelper
{
    Task Publish<T>(T evt, CancellationToken cancellationToken = default) where T : IntegrationEvent;
}

public class BusHelper : IBusHelper
{
    private readonly ScimHostOptions _options;

    public BusHelper(IOptions<ScimHostOptions> options)
    {
        _options = options.Value;
    }

    public Task Publish<T>(T evt, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        if (evt is RepresentationAddedEvent addedEvent)
        {
            _options.SCIMEvents.RepresentationAdded?.Invoke(addedEvent);
        }
        else if (evt is RepresentationRefAttributeAddedEvent refAttributeAddedEvent)
        {
            _options.SCIMEvents.RepresentationRefAttributeAdded?.Invoke(refAttributeAddedEvent);
        }
        else if (evt is RepresentationRefAttributeRemovedEvent refAttributeRemovedEvent)
        {
            _options.SCIMEvents.RepresentationRefAttributeRemoved?.Invoke(refAttributeRemovedEvent);
        }
        else if (evt is RepresentationRemovedEvent removedEvent)
        {
            _options.SCIMEvents.RepresentationRemoved?.Invoke(removedEvent);
        }
        else if (evt is RepresentationUpdatedEvent updatedEvent)
        {
            _options.SCIMEvents.RepresentationUpdated?.Invoke(updatedEvent);
        }

        return Task.CompletedTask;
    }
}
