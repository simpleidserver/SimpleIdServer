// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure.Lock;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class ReplaceRepresentationCommandHandler : BaseCommandHandler, IReplaceRepresentationCommandHandler
    {
        private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;
        private readonly IDistributedLock _distributedLock;

        public ReplaceRepresentationCommandHandler(
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
            ISCIMSchemaQueryRepository scimSchemaQueryRepository,
            ISCIMRepresentationHelper scimRepresentationHelper,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IDistributedLock distributedLock,
            IBusControl busControl) : base(busControl)
        {
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
            _distributedLock = distributedLock;
        }

        public async Task<SCIMRepresentation> Handle(ReplaceRepresentationCommand replaceRepresentationCommand)
        {
            var requestedSchemas = replaceRepresentationCommand.Representation.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Schemas));
            }

            var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(replaceRepresentationCommand.ResourceType);
            var allSchemas = new List<string> { schema.Id };
            allSchemas.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
            var unsupportedSchemas = requestedSchemas.Where(s => !allSchemas.Contains(s));
            if (unsupportedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.SchemasAreUnknown, string.Join(",", unsupportedSchemas)));
            }

            var schemas = await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            var lockName = $"representation-{replaceRepresentationCommand.Id}";
            await _distributedLock.WaitLock(lockName, CancellationToken.None);
            try
            {
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
                var isReferenceProperty = await _representationReferenceSync.IsReferenceProperty(replaceRepresentationCommand.Representation.Attributes.GetKeys());
                var references = await _representationReferenceSync.Sync(updateResult.AttributeMappingLst, replaceRepresentationCommand.ResourceType, oldRepresentation, existingRepresentation, replaceRepresentationCommand.Location, !isReferenceProperty);
                using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
                {
                    await _scimRepresentationCommandRepository.Update(existingRepresentation);
                    foreach (var reference in references.Representations)
                    {
                        await _scimRepresentationCommandRepository.Update(reference);
                    }

                    await transaction.Commit();
                }

                await Notify(references);
                existingRepresentation.ApplyEmptyArray();
                return existingRepresentation;
            }
            finally
            {
                await _distributedLock.ReleaseLock(lockName, CancellationToken.None);
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

            return new UpdateRepresentationResult { AttributeMappingLst = attributeMappings };
        }

        private void RemoveUnusedAttributes(IEnumerable<SCIMAttributeMapping> attributeMappings, SCIMRepresentation updatedRepresentation)
        {
            var hierarchicalUpdatedAttributes = updatedRepresentation.HierarchicalAttributes;
            foreach (var attributeMapping in attributeMappings)
            {
                var attrLstToRemove = hierarchicalUpdatedAttributes.Where(a => a.SchemaAttributeId == attributeMapping.SourceAttributeId)
                    .SelectMany(a => a.Children)
                    .Where(c => 
                    c.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Type || c.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Display);
                updatedRepresentation.RemoveAttributesById(attrLstToRemove.Select(a => a.Id));
            }
        }

        private class UpdateRepresentationResult
        {
            public IEnumerable<SCIMAttributeMapping> AttributeMappingLst { get; set; }
        }
    }
}
