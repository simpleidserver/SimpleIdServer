// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.ExternalEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationSyncResult
    {
        public RepresentationSyncResult()
        {
            Representations = new List<SCIMRepresentation>();
            RemoveAttrEvts = new List<RepresentationReferenceAttributeRemovedEvent>();
            AddAttrEvts = new List<RepresentationReferenceAttributeAddedEvent>();
        }

        public ICollection<SCIMRepresentation> Representations { get; set; }
        public ICollection<RepresentationReferenceAttributeRemovedEvent> RemoveAttrEvts { get; set; }
        public ICollection<RepresentationReferenceAttributeAddedEvent> AddAttrEvts { get; set; }

        public void AddRepresentation(SCIMRepresentation representation)
        {
            Representations.Add(representation);
        }

        public void AddReferenceAttr(SCIMRepresentation representation, string schemaAttributeId, string fullPath, string value)
        {
            var newEvt = new RepresentationReferenceAttributeAddedEvent(Guid.NewGuid().ToString(), representation.Version, representation.ResourceType, representation.Id, schemaAttributeId, fullPath);
            newEvt.Values.Add(value);
            ProcessReferenceAttr(AddAttrEvts, representation, schemaAttributeId, newEvt, value);
        }

        public void RemoveReferenceAttr(SCIMRepresentation representation, string schemaAttributeId, string fullPath, string value)
        {
            var newEvt = new RepresentationReferenceAttributeRemovedEvent(Guid.NewGuid().ToString(), representation.Version, representation.ResourceType, representation.Id, schemaAttributeId, fullPath);
            newEvt.Values.Add(value);
            ProcessReferenceAttr(RemoveAttrEvts, representation, schemaAttributeId, newEvt, value);
        }

        private void ProcessReferenceAttr<T>(ICollection<T> evts, SCIMRepresentation representation, string schemaAttributeId, T newEvt, string value) where T : BaseReferenceAttributeEvent
        {
            var evt = evts.FirstOrDefault(e => e.RepresentationAggregateId == representation.Id && e.SchemaAttributeId == schemaAttributeId);
            if (evt != null)
            {
                evt.Values.Add(value);
                return;
            }

            evts.Add(newEvt);
        }
    }
}
