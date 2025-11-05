// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Helpers;
using System.Collections.Generic;
using System.Linq;
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
        
        protected async Task NotifyAllReferences(
            string fromRepresentationId, 
            string fromRepresentationType, 
            string? realm, 
            List<RepresentationSyncResult> references,
            List<SCIMAttributeMapping> mappings) 
        {
            var notificationTasks = references.Select(reference => 
                Notify(fromRepresentationId, fromRepresentationType, realm, reference, mappings));
            
            await Task.WhenAll(notificationTasks);
        }

        private async Task Notify(
            string fromRepresentationId, 
            string fromRepresentationType, 
            string? realm,
            RepresentationSyncResult result,
            List<SCIMAttributeMapping> mappings)
        {
            await NotifyRemovedAttributes(fromRepresentationId, fromRepresentationType, realm, result, mappings);
            await NotifyAddedAttributes(fromRepresentationId, fromRepresentationType, realm, result, mappings);
        }

        private async Task NotifyRemovedAttributes(
            string fromRepresentationId,
            string fromRepresentationType,
            string? realm,
            RepresentationSyncResult result,
            List<SCIMAttributeMapping> mappings)
        {
            var groupedAttributes = result.RemovedRepresentationAttributes.GroupBy(a => a.RepresentationId);            
            foreach (var group in groupedAttributes)
            {
                var mapping = FindMapping(group, fromRepresentationType, mappings);
                if (mapping != null)
                {
                    var evt = new RepresentationRefAttributeRemovedEvent(
                        fromRepresentationId, 
                        fromRepresentationType, 
                        realm, 
                        group.Key, 
                        mapping.TargetResourceType);
                    
                    await _busControl.Publish(evt);
                }
            }
        }

        private async Task NotifyAddedAttributes(
            string fromRepresentationId,
            string fromRepresentationType,
            string? realm,
            RepresentationSyncResult result,
            List<SCIMAttributeMapping> mappings)
        {
            var groupedAttributes = result.AddedRepresentationAttributes.GroupBy(a => a.RepresentationId);
            foreach (var group in groupedAttributes)
            {
                var mapping = FindMapping(group, fromRepresentationType, mappings);
                if (mapping != null)
                {
                    var evt = new RepresentationRefAttributeAddedEvent(
                        fromRepresentationId, 
                        fromRepresentationType, 
                        realm, 
                        group.Key, 
                        mapping.TargetResourceType);
                    
                    await _busControl.Publish(evt);
                }
            }
        }

        private SCIMAttributeMapping FindMapping(
            IGrouping<string, SCIMRepresentationAttribute> attributeGroup,
            string fromRepresentationType,
            List<SCIMAttributeMapping> mappings)
        {
            var sourceAttributeIds = attributeGroup.Select(a => a.SchemaAttributeId).ToList();
            
            return mappings.FirstOrDefault(m => 
                m.SourceResourceType == fromRepresentationType && 
                sourceAttributeIds.Contains(m.TargetAttributeId));
        }
    }
}