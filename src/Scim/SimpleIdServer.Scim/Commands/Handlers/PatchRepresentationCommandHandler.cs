// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class PatchRepresentationCommandHandler : BaseCommandHandler, IPatchRepresentationCommandHandler
    {
        private readonly ISCIMSchemaCommandRepository _scimSchemaCommandRepository;
        private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;
        private readonly IRepresentationHelper _representationHelper;
        private readonly SCIMHostOptions _options;

        public PatchRepresentationCommandHandler(
            ISCIMSchemaCommandRepository scimSchemaCommandRepository,
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IRepresentationHelper representationHelper,
            IOptions<SCIMHostOptions> options,
            IBusHelper busControl) : base(busControl)
        {
            _scimSchemaCommandRepository = scimSchemaCommandRepository;
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
            _representationHelper = representationHelper;
            _options = options.Value;
        }

        public async virtual Task<GenericResult<PatchRepresentationResult>> Handle(PatchRepresentationCommand patchRepresentationCommand)
        {
            var schema = await _scimSchemaCommandRepository.FindRootSCIMSchemaByResourceType(patchRepresentationCommand.ResourceType);
            if (schema == null) throw new SCIMSchemaNotFoundException();
            CheckParameter(patchRepresentationCommand.PatchRepresentation);
            var existingRepresentation = await _scimRepresentationCommandRepository.Get(patchRepresentationCommand.Realm, patchRepresentationCommand.Id);
            if (existingRepresentation == null) throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, patchRepresentationCommand.Id));
            return await UpdateRepresentation(existingRepresentation, patchRepresentationCommand, schema);
        }

        private async Task<GenericResult<PatchRepresentationResult>> UpdateRepresentation(SCIMRepresentation existingRepresentation, PatchRepresentationCommand patchRepresentationCommand, SCIMSchema schema)
        {
            var attributeMappings = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(existingRepresentation.ResourceType);
            var oldDisplayName = existingRepresentation.DisplayName;
            var patchResult = await _representationHelper.Apply(existingRepresentation, patchRepresentationCommand.PatchRepresentation.Operations, attributeMappings, _options.IgnoreUnsupportedCanonicalValues, CancellationToken.None);
            var patchResultLst = patchResult.Patches.Where(p => p.Attr != null).ToList();
            var displayNameDifferent = existingRepresentation.DisplayName != oldDisplayName;
            if (!patchResult.Patches.Any()) return GenericResult<PatchRepresentationResult>.Ok(PatchRepresentationResult.NoPatch());
            existingRepresentation.SetUpdated(DateTime.UtcNow);
            var references = await _representationReferenceSync.Sync(patchRepresentationCommand.ResourceType, existingRepresentation, patchResultLst, patchRepresentationCommand.Location, schema, displayNameDifferent);
            await using (var transaction = await _scimRepresentationCommandRepository.StartTransaction().ConfigureAwait(false))
            {
                await _scimRepresentationCommandRepository.BulkDelete(patchResultLst.Where(p => p.Operation == SCIMPatchOperations.REMOVE && p.Attr != null).Select(p => p.Attr), existingRepresentation.Id).ConfigureAwait(false);
                await _scimRepresentationCommandRepository.BulkInsert(patchResultLst.Where(p => p.Operation == SCIMPatchOperations.ADD && p.Attr != null).Select(p => p.Attr), existingRepresentation.Id).ConfigureAwait(false);
                await _scimRepresentationCommandRepository.BulkUpdate(patchResultLst.Where(p => p.Operation == SCIMPatchOperations.REPLACE && p.Attr != null).Select(p => p.Attr)).ConfigureAwait(false);
                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkInsert(reference.AddedRepresentationAttributes, existingRepresentation.Id,  true).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkUpdate(reference.UpdatedRepresentationAttributes, true).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkDelete(reference.RemovedRepresentationAttributes, existingRepresentation.Id, true).ConfigureAwait(false);
                }

                await _scimRepresentationCommandRepository.Update(existingRepresentation).ConfigureAwait(false);
                await transaction.Commit().ConfigureAwait(false);
                await NotifyAllReferences(references).ConfigureAwait(false);
            }

            existingRepresentation.Apply(references, patchResultLst);
            return GenericResult<PatchRepresentationResult>.Ok(PatchRepresentationResult.Ok(existingRepresentation, patchResultLst));
        }

        private void CheckParameter(PatchRepresentationParameter patchRepresentation)
        {
            if (patchRepresentation == null || (patchRepresentation.Operations != null && patchRepresentation.Operations.Any(o => o.Operation == null)))
            {
                throw new SCIMBadSyntaxException(string.Format(Global.RequestIsNotWellFormatted, "PATCH"));
            }

            var requestedSchemas = patchRepresentation.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Schemas));
            }

            if (!requestedSchemas.SequenceEqual(new List<string> { StandardSchemas.PatchRequestSchemas.Id }))
            {
                throw new SCIMBadSyntaxException(Global.SchemasNotRecognized);
            }

            if (patchRepresentation.Operations == null)
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Operations));
            }

            if(!patchRepresentation.Operations.Any())
            {
                throw new SCIMBadSyntaxException(Global.OperationsRequired);
            }
        }
    }
}
