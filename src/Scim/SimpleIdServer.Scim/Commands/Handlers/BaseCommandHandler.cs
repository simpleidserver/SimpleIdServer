// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Helpers;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class BaseCommandHandler
    {
        private readonly IBusHelper _busControl;

        public BaseCommandHandler(IBusHelper busControl)
        {
            _busControl = busControl;
        }
        
        protected async Task NotifyAllReferences(List<RepresentationSyncResult> references) {
            var tasks = references.Select(Notify).ToList();
            await Task.WhenAll(tasks);
        }

        private async Task Notify(RepresentationSyncResult result)
        {
            foreach(var removeAttr in result.RemovedRepresentationAttributes) await _busControl.Publish(new RepresentationRefAttributeRemovedEvent(removeAttr));
            foreach(var addAttr in result.AddedRepresentationAttributes) await _busControl.Publish(new RepresentationRefAttributeAddedEvent(addAttr));
            foreach (var updateAttr in result.UpdatedRepresentationAttributes) await _busControl.Publish(new RepresentationRefAttributeUpdatedEvent(updateAttr));
        }
    }
}