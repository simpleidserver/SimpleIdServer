// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class ReplaceRepresentationCommandHandler : BaseCommandHandler, IReplaceRepresentationCommandHandler
    {
        private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
        private readonly ISCIMSchemaCommandRepository _scimSchemaCommandRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;

        public ReplaceRepresentationCommandHandler(
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
            ISCIMSchemaCommandRepository scimSchemaCommandRepository,
            ISCIMRepresentationHelper scimRepresentationHelper,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IBusControl busControl) : base(busControl)
        {
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimSchemaCommandRepository = scimSchemaCommandRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
        }

        public async virtual Task<GenericResult<SCIMRepresentation>> Handle(ReplaceRepresentationCommand replaceRepresentationCommand)
        {
            var requestedSchemas = replaceRepresentationCommand.Representation.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Schemas));
            }

            var schema = await _scimSchemaCommandRepository.FindRootSCIMSchemaByResourceType(replaceRepresentationCommand.ResourceType);
            if (schema == null) throw new SCIMSchemaNotFoundException();
            var allSchemas = new List<string> { schema.Id };
            allSchemas.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
            var unsupportedSchemas = requestedSchemas.Where(s => !allSchemas.Contains(s));
            if (unsupportedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.SchemasAreUnknown, string.Join(",", unsupportedSchemas)));
            }

            var schemas = await _scimSchemaCommandRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            var existingRepresentation = await _scimRepresentationCommandRepository.Get(replaceRepresentationCommand.Id);
            if (existingRepresentation == null)
            {
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, replaceRepresentationCommand.Id));
            }

            var oldRepresentation = (SCIMRepresentation)existingRepresentation.Clone();
            var mainSchema = schemas.First(s => s.Id == schema.Id);
            var extensionSchemas = schemas.Where(s => s.Id != schema.Id).ToList();
            var updatedRepresentation = _scimRepresentationHelper.ExtractSCIMRepresentationFromJSON(
                replaceRepresentationCommand.Representation.Attributes,
                replaceRepresentationCommand.Representation.ExternalId,
                mainSchema,
                extensionSchemas);
            var updateResult = await UpdateExistingRepresentation(replaceRepresentationCommand.ResourceType, existingRepresentation, updatedRepresentation);
            existingRepresentation.SetDisplayName(updatedRepresentation.DisplayName);
            existingRepresentation.SetExternalId(updatedRepresentation.ExternalId);
            existingRepresentation.SetUpdated(DateTime.UtcNow);
            
            var uniqueServerAttributeIds = existingRepresentation.FlatAttributes.Where(s => s.IsLeaf()).Where(a => a.SchemaAttribute.MultiValued == false && a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.SERVER);
            var uniqueGlobalAttributes = existingRepresentation.FlatAttributes.Where(s => s.IsLeaf()).Where(a => a.SchemaAttribute.MultiValued == false && a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.GLOBAL);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueServerAttributeIds, existingRepresentation.Id, replaceRepresentationCommand.ResourceType);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueGlobalAttributes, existingRepresentation.Id);
            
            var isReferenceProperty = await _representationReferenceSync.IsReferenceProperty(replaceRepresentationCommand.Representation.Attributes.GetKeys());
            var references = _representationReferenceSync.Sync(updateResult.AttributeMappingLst, replaceRepresentationCommand.ResourceType, oldRepresentation, existingRepresentation, replaceRepresentationCommand.Location, schema, !isReferenceProperty);
            using (var transaction = await _scimRepresentationCommandRepository.StartTransaction().ConfigureAwait(false))
            {
                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkInsert(reference.AddedRepresentationAttributes).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkDelete(reference.RemovedRepresentationAttributes).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkUpdate(reference.UpdatedRepresentationAttributes).ConfigureAwait(false);
                    await Notify(reference);
                }

                await _scimRepresentationCommandRepository.Update(existingRepresentation).ConfigureAwait(false);
                await transaction.Commit().ConfigureAwait(false);
                existingRepresentation.ApplyEmptyArray();
                return GenericResult<SCIMRepresentation>.Ok(existingRepresentation);
            }
        }

        private async Task<UpdateRepresentationResult> UpdateExistingRepresentation(string resourceType, SCIMRepresentation existingRepresentation, SCIMRepresentation updatedRepresentation)
        {
            var attributeMappings = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
            RemoveUnusedAttributes(attributeMappings, updatedRepresentation);
            var updatedAttributes = updatedRepresentation.HierarchicalAttributes;
            var allExistingAttributes = existingRepresentation.HierarchicalAttributes;
            existingRepresentation.RemoveAttributesBySchemaAttrId(updatedAttributes.Select(u => u.SchemaAttribute.Id).Distinct());
            foreach (var kvp in updatedAttributes.GroupBy(h => h.FullPath))
            {
                var fullPath = kvp.Key;
                var filteredExistingAttributes = allExistingAttributes.Where(a => a.FullPath == fullPath);
                var invalidAttrs = filteredExistingAttributes.Where(fa => !kvp.Any(a => a.IsMutabilityValid(fa)));
                if (invalidAttrs.Any())
                {
                    throw new SCIMImmutableAttributeException(string.Format(Global.AttributeImmutable, string.Join(",", invalidAttrs.Select(a => a.FullPath))));
                }

                foreach (var rootAttr in kvp)
                {
                    if (rootAttr.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.WRITEONLY || rootAttr.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READWRITE || rootAttr.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.IMMUTABLE)
                    {
                        var flatAttrs = rootAttr.ToFlat();
                        foreach (var attr in flatAttrs)
                        {
                            existingRepresentation.AddAttribute(attr);
                        }
                    }
                }
            }

            var missingSchemas = updatedRepresentation.Schemas.Where(s => !existingRepresentation.Schemas.Any(sh => sh.Id == s.Id));
            foreach (var missingSchema in missingSchemas) existingRepresentation.Schemas.Add(missingSchema);
            return new UpdateRepresentationResult { AttributeMappingLst = attributeMappings };
        }

        private void RemoveUnusedAttributes(IEnumerable<SCIMAttributeMapping> attributeMappings, SCIMRepresentation updatedRepresentation)
        {
            var hierarchicalUpdatedAttributes = updatedRepresentation.HierarchicalAttributes;
            foreach (var attributeMapping in attributeMappings)
            {
                var attrLstToRemove = hierarchicalUpdatedAttributes.Where(a => a.SchemaAttributeId == attributeMapping.SourceAttributeId)
                    .SelectMany(a => a.CachedChildren)
                    .Where(c => 
                    c.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Type || c.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Display);
                updatedRepresentation.RemoveAttributesById(attrLstToRemove.Select(a => a.Id));
            }
        }

        private async Task CheckSCIMRepresentationExistsForGivenUniqueAttributes(IEnumerable<SCIMRepresentationAttribute> attributes, string currentId, string endpoint = null)
        {
            foreach (var attribute in attributes)
            {
                SCIMRepresentation record = null;
                switch (attribute.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        record = await _scimRepresentationCommandRepository.FindSCIMRepresentationByAttribute(attribute.SchemaAttribute.Id, attribute.ValueString, endpoint);
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        if (attribute.ValueInteger != null)
                        {
                            record = await _scimRepresentationCommandRepository.FindSCIMRepresentationByAttribute(attribute.SchemaAttribute.Id, attribute.ValueInteger.Value, endpoint);
                        }

                        break;
                }

                if (record != null && record.Id != currentId)
                {
                    throw new SCIMUniquenessAttributeException(string.Format(Global.AttributeMustBeUnique, attribute.SchemaAttribute.Name));
                }
            }
        }

        private class UpdateRepresentationResult
        {
            public IEnumerable<SCIMAttributeMapping> AttributeMappingLst { get; set; }
        }
    }
}
