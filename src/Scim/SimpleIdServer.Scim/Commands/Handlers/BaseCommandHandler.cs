// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Helpers;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class BaseCommandHandler
    {
        private readonly IBusControl _busControl;

        public BaseCommandHandler(IBusControl busControl)
        {
            _busControl = busControl;
        }

        protected async Task Notify(RepresentationSyncResult result)
        {
            foreach(var removeAttr in result.RemovedRepresentationAttributes) await _busControl.Publish(new RepresentationRefAttributeRemovedEvent(removeAttr));
            foreach(var addAttr in result.AddedRepresentationAttributes) await _busControl.Publish(new RepresentationRefAttributeAddedEvent(addAttr));
            foreach (var updateAttr in result.UpdatedRepresentationAttributes) await _busControl.Publish(new RepresentationRefAttributeRemovedEvent(updateAttr));
        }
    }
}
