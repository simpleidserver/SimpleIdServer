// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Scim.ExternalEvents;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Startup.Consumers
{
    public class IntegrationEventConsumer : IConsumer<RepresentationAddedEvent>,
        IConsumer<RepresentationRemovedEvent>,
        IConsumer<RepresentationUpdatedEvent>,
        IConsumer<RepresentationReferenceAttributeUpdatedEvent>
    {
        private readonly ILogger<IntegrationEventConsumer> _logger;

        public IntegrationEventConsumer(ILogger<IntegrationEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RepresentationAddedEvent> context)
        {
            _logger.LogInformation($"Event received : Representation '{context.Message.ResourceType}' has been added");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RepresentationRemovedEvent> context)
        {
            _logger.LogInformation($"Event received : Representation '{context.Message.ResourceType}' has been removed");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RepresentationUpdatedEvent> context)
        {
            _logger.LogInformation($"Event received : Representation '{context.Message.ResourceType}' has been updated");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RepresentationReferenceAttributeUpdatedEvent> context)
        {
            _logger.LogInformation($"Event received : Representation '{context.Message.ResourceType}' has been updated");
            return Task.CompletedTask;
        }
    }
}
