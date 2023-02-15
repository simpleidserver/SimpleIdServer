// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.ExternalEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationSyncResult
    {
        private readonly IResourceTypeResolver _resourceTypeResolver;

        public RepresentationSyncResult(IResourceTypeResolver resourceTypeResolver)
        {
            _resourceTypeResolver = resourceTypeResolver;
        }

        public ICollection<SCIMRepresentation> Representations { get; set; } = new List<SCIMRepresentation>();
        public ICollection<RepresentationReferenceAttributeRemovedEvent> RemoveAttrEvts { get; set; } = new List<RepresentationReferenceAttributeRemovedEvent>();
        public ICollection<RepresentationReferenceAttributeAddedEvent> AddAttrEvts { get; set; } = new List<RepresentationReferenceAttributeAddedEvent>();
        public ICollection<RepresentationReferenceAttributeUpdatedEvent> UpdateAttrEvts { get; set; } = new List<RepresentationReferenceAttributeUpdatedEvent>();
        public List<SCIMRepresentationAttribute> AddedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();
        public List<SCIMRepresentationAttribute> RemovedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();

        public void AddReferenceAttr(SCIMRepresentation representation, string schemaAttributeId, string fullPath, string value, string location)
        {
            location = $"{location}/{_resourceTypeResolver.ResolveByResourceType(representation.ResourceType).ControllerName}/{representation.Id}";
            var obj = representation.Duplicate().ToResponse(location, false, addEmptyArray: true);
            var newEvt = new RepresentationReferenceAttributeAddedEvent(Guid.NewGuid().ToString(), representation.Version, representation.ResourceType, representation.Id, schemaAttributeId, fullPath, obj);
            newEvt.Values.Add(value);
            ProcessReferenceAttr(AddAttrEvts, representation, schemaAttributeId, newEvt, value);
            if (Representations.Any(r => r.Id == representation.Id)) Representations.Add(representation);
        }

        public void AddReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs) => AddedRepresentationAttributes.AddRange(attrs);

        public void RemoveReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs) => RemovedRepresentationAttributes.AddRange(attrs);

        public void RemoveReferenceAttr(SCIMRepresentation representation, string schemaAttributeId, string fullPath, string value, string location)
        {
            location = $"{location}/{_resourceTypeResolver.ResolveByResourceType(representation.ResourceType).ControllerName}/{representation.Id}";
            var obj = representation.Duplicate().ToResponse(location, false, addEmptyArray: true);
            var newEvt = new RepresentationReferenceAttributeRemovedEvent(Guid.NewGuid().ToString(), representation.Version, representation.ResourceType, representation.Id, schemaAttributeId, fullPath, obj);
            newEvt.Values.Add(value);
            ProcessReferenceAttr(RemoveAttrEvts, representation, schemaAttributeId, newEvt, value);
        }

        public void UpdateReferenceAttr(SCIMRepresentation representation, string schemaAttributeId, string fullPath, string value, string location)
        {
            location = $"{location}/{_resourceTypeResolver.ResolveByResourceType(representation.ResourceType).ControllerName}/{representation.Id}";
            var obj = representation.Duplicate().ToResponse(location, false, addEmptyArray: true);
            var newEvt = new RepresentationReferenceAttributeUpdatedEvent(Guid.NewGuid().ToString(), representation.Version, representation.ResourceType, representation.Id, schemaAttributeId, fullPath, obj);
            newEvt.Values.Add(value);
            ProcessReferenceAttr(UpdateAttrEvts, representation, schemaAttributeId, newEvt, value);
        }

        private void ProcessReferenceAttr<T>(ICollection<T> evts, SCIMRepresentation representation, string schemaAttributeId, T newEvt, string value) where T : BaseReferenceAttributeEvent
        {
            var evt = evts.FirstOrDefault(e => e.RepresentationAggregateId == representation.Id && e.SchemaAttributeId == schemaAttributeId);
            if (evt != null)
            {
                if (!evt.Values.Contains(value))
                    evt.Values.Add(value);
                return;
            }

            evts.Add(newEvt);
        }
    }
}
