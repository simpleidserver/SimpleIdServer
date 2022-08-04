// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class PatchRepresentationCommandHandler : BaseCommandHandler, IPatchRepresentationCommandHandler
    {
        private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;
        private readonly SCIMHostOptions _options;

        public PatchRepresentationCommandHandler(
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IOptions<SCIMHostOptions> options,
            IBusControl busControl) : base(busControl)
        {
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
            _options = options.Value;
        }

        public async Task<PatchRepresentationResult> Handle(PatchRepresentationCommand patchRepresentationCommand)
        {
            CheckParameter(patchRepresentationCommand.PatchRepresentation);
            var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(patchRepresentationCommand.Id);
            if (existingRepresentation == null) throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, patchRepresentationCommand.Id));
            return await UpdateRepresentation(existingRepresentation, patchRepresentationCommand);
        }

        private async Task<PatchRepresentationResult> UpdateRepresentation(SCIMRepresentation existingRepresentation, PatchRepresentationCommand patchRepresentationCommand)
        {
            var attributeMappings = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(existingRepresentation.ResourceType);
            var patchResult = existingRepresentation.ApplyPatches(patchRepresentationCommand.PatchRepresentation.Operations, attributeMappings, _options.IgnoreUnsupportedCanonicalValues);
            if (!patchResult.Any()) return PatchRepresentationResult.NoPatch();
            existingRepresentation.SetUpdated(DateTime.UtcNow);
            var references = await _representationReferenceSync.Sync(patchRepresentationCommand.ResourceType, existingRepresentation, patchResult, patchRepresentationCommand.Location);
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
            return PatchRepresentationResult.Ok(existingRepresentation);
        }

        private void CheckParameter(PatchRepresentationParameter patchRepresentation)
        {
            if (patchRepresentation == null)
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
        }
    }
}
