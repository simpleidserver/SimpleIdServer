// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IRepresentationHelper _representationHelper;

        public ReplaceRepresentationCommandHandler(
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
            ISCIMSchemaCommandRepository scimSchemaCommandRepository,
            ISCIMRepresentationHelper scimRepresentationHelper,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IRepresentationHelper representationHelper,
            IBusControl busControl) : base(busControl)
        {
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimSchemaCommandRepository = scimSchemaCommandRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
            _representationHelper = representationHelper;
        }

        public async virtual Task<GenericResult<SCIMRepresentation>> Handle(ReplaceRepresentationCommand replaceRepresentationCommand)
        {
            var requestedSchemas = replaceRepresentationCommand.Representation.Schemas;
            if (!requestedSchemas.Any())
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Schemas));

            var schema = await _scimSchemaCommandRepository.FindRootSCIMSchemaByResourceType(replaceRepresentationCommand.ResourceType);
            if (schema == null) throw new SCIMSchemaNotFoundException();
            var allSchemas = new List<string> { schema.Id };
            allSchemas.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
            var unsupportedSchemas = requestedSchemas.Where(s => !allSchemas.Contains(s));
            if (unsupportedSchemas.Any())
                throw new SCIMBadSyntaxException(string.Format(Global.SchemasAreUnknown, string.Join(",", unsupportedSchemas)));

            var schemas = await _scimSchemaCommandRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            var existingRepresentation = await _scimRepresentationCommandRepository.Get(replaceRepresentationCommand.Id);
            if (existingRepresentation == null)
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, replaceRepresentationCommand.Id));

            var oldDisplayName = existingRepresentation.DisplayName;
            var patchParameters = new List<PatchOperationParameter>
            {
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Value = replaceRepresentationCommand.Representation.Attributes
                }
            };
            var attributeMappings = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(replaceRepresentationCommand.ResourceType);
            var patchResult = await _representationHelper.Apply(existingRepresentation, patchParameters, attributeMappings, true, CancellationToken.None);
            var patchOperations = patchResult.Patches;
            var displayNameDifferent = existingRepresentation.DisplayName != oldDisplayName;
            var modifiedAttributes = patchResult.Patches.Where(p => p.Operation != SCIMPatchOperations.REMOVE && !p.Attr.IsLeaf() && p.Attr.SchemaAttribute.MultiValued == false).Select(p => p.Attr);
            var uniqueServerAttributeIds = modifiedAttributes.Where(a => a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.SERVER);
            var uniqueGlobalAttributes = modifiedAttributes.Where(a => a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.GLOBAL);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueServerAttributeIds, existingRepresentation.Id, replaceRepresentationCommand.ResourceType);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueGlobalAttributes, existingRepresentation.Id);
            var references = await _representationReferenceSync.Sync(existingRepresentation.ResourceType, existingRepresentation, patchResult.Patches, replaceRepresentationCommand.Location, schema, displayNameDifferent);
            await using (var transaction = await _scimRepresentationCommandRepository.StartTransaction().ConfigureAwait(false))
            {
                await _scimRepresentationCommandRepository.BulkDelete(patchOperations.Where(p => p.Operation == SCIMPatchOperations.REMOVE).Select(p => p.Attr)).ConfigureAwait(false);
                await _scimRepresentationCommandRepository.BulkInsert(patchOperations.Where(p => p.Operation == SCIMPatchOperations.ADD).Select(p => p.Attr)).ConfigureAwait(false);
                await _scimRepresentationCommandRepository.BulkUpdate(patchOperations.Where(p => p.Operation == SCIMPatchOperations.REPLACE).Select(p => p.Attr)).ConfigureAwait(false);

                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkInsert(reference.AddedRepresentationAttributes).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkDelete(reference.RemovedRepresentationAttributes).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkUpdate(reference.UpdatedRepresentationAttributes).ConfigureAwait(false);
                }

                await _scimRepresentationCommandRepository.Update(existingRepresentation).ConfigureAwait(false);
                await transaction.Commit().ConfigureAwait(false);
                await NotifyAllReferences(references).ConfigureAwait(false);
            }

            existingRepresentation.ApplyEmptyArray();
            return GenericResult<SCIMRepresentation>.Ok(existingRepresentation);
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
