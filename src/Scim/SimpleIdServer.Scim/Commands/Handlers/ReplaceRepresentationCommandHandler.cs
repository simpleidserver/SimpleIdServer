// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;
        private readonly IRepresentationHelper _representationHelper;

        public ReplaceRepresentationCommandHandler(
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
            ISCIMSchemaCommandRepository scimSchemaCommandRepository,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IRepresentationHelper representationHelper,
            IBusHelper busControl) : base(busControl)
        {
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimSchemaCommandRepository = scimSchemaCommandRepository;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
            _representationHelper = representationHelper;
        }

        public async virtual Task<GenericResult<ReplaceRepresentationResult>> Handle(ReplaceRepresentationCommand replaceRepresentationCommand)
        {
            var attributeMappings = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(replaceRepresentationCommand.ResourceType);
            var kvp = await Validate(replaceRepresentationCommand, attributeMappings);
            var existingRepresentation = kvp.Item1;
            var schema = kvp.Item2;
            var oldDisplayName = existingRepresentation.DisplayName;
            var patchParameters = new List<PatchOperationParameter>
            {
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Value = replaceRepresentationCommand.Representation.Attributes
                }
            };
            var patchResult = await _representationHelper.Apply(existingRepresentation, patchParameters, attributeMappings, true, CancellationToken.None);
            if (!patchResult.Patches.Any()) return GenericResult<ReplaceRepresentationResult>.Ok(ReplaceRepresentationResult.NoReplacement());
            var patchOperations = patchResult.Patches.Where(p => p.Attr != null).ToList();
            var displayNameDifferent = existingRepresentation.DisplayName != oldDisplayName;
            var modifiedAttributes = patchOperations.Where(p => p.Operation != SCIMPatchOperations.REMOVE && p.Attr != null && !p.Attr.IsLeaf() && p.Attr.SchemaAttribute.MultiValued == false).Select(p => p.Attr);
            await _representationHelper.CheckUniqueness(replaceRepresentationCommand.Realm, modifiedAttributes);
            _representationHelper.CheckMutability(patchOperations);
            var references = await _representationReferenceSync.Sync(existingRepresentation.ResourceType, existingRepresentation, patchOperations, replaceRepresentationCommand.Location, schema, displayNameDifferent);
            await using (var transaction = await _scimRepresentationCommandRepository.StartTransaction().ConfigureAwait(false))
            {
                await _scimRepresentationCommandRepository.BulkDelete(patchOperations.Where(p => p.Operation == SCIMPatchOperations.REMOVE).Select(p => p.Attr), existingRepresentation.Id).ConfigureAwait(false);
                await _scimRepresentationCommandRepository.BulkInsert(patchOperations.Where(p => p.Operation == SCIMPatchOperations.ADD).Select(p => p.Attr), existingRepresentation.Id).ConfigureAwait(false);
                await _scimRepresentationCommandRepository.BulkUpdate(patchOperations.Where(p => p.Operation == SCIMPatchOperations.REPLACE).Select(p => p.Attr)).ConfigureAwait(false);

                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkInsert(reference.AddedRepresentationAttributes, existingRepresentation.Id, true).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkDelete(reference.RemovedRepresentationAttributes, existingRepresentation.Id, true).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkUpdate(reference.UpdatedRepresentationAttributes, true).ConfigureAwait(false);
                }

                await _scimRepresentationCommandRepository.Update(existingRepresentation).ConfigureAwait(false);
                await transaction.Commit().ConfigureAwait(false);
                await NotifyAllReferences(references).ConfigureAwait(false);
            }

            existingRepresentation.Apply(references, patchOperations);
            return GenericResult<ReplaceRepresentationResult>.Ok(ReplaceRepresentationResult.Ok(existingRepresentation, patchOperations));
        }

        private async Task<(SCIMRepresentation, SCIMSchema)> Validate(ReplaceRepresentationCommand replaceRepresentationCommand, IEnumerable<SCIMAttributeMapping> attributeMappings)
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

            var existingRepresentation = await _scimRepresentationCommandRepository.Get(replaceRepresentationCommand.Realm, replaceRepresentationCommand.Id);
            if (existingRepresentation == null)
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, replaceRepresentationCommand.Id));

            var schemas = await _scimSchemaCommandRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            _representationHelper.ExtractSCIMRepresentationFromJSON(replaceRepresentationCommand.Representation.Attributes, replaceRepresentationCommand.Representation.ExternalId, schema, schemas.Where(s => s.Id != schema.Id).ToList(), attributeMappings);
            
            return (existingRepresentation, schema);
        }
    }
}
