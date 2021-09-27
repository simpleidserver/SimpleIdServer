// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
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
            foreach(var removeAttr in result.RemoveAttrEvts)
            {
                await _busControl.Publish(removeAttr);
            }

            foreach(var addAttr in result.AddAttrEvts)
            {
                await _busControl.Publish(addAttr);
            }
        }
    }
}
