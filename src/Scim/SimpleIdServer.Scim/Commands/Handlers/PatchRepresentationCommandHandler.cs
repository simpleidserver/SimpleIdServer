// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
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
    public class PatchRepresentationCommandHandler : BaseCommandHandler, IPatchRepresentationCommandHandler
    {
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IDistributedLock _distributedLock;
        private readonly IRepresentationReferenceSync _representationReferenceSync;
        private readonly SCIMHostOptions _options;

        public PatchRepresentationCommandHandler(
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository, 
            IDistributedLock distributedLock,
            IRepresentationReferenceSync representationReferenceSync,
            IOptions<SCIMHostOptions> options,
            IBusControl busControl) : base(busControl)
        {
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _distributedLock = distributedLock;
            _representationReferenceSync = representationReferenceSync;
            _options = options.Value;
        }

        public async Task<PatchRepresentationResult> Handle(PatchRepresentationCommand patchRepresentationCommand)
        {
            CheckParameter(patchRepresentationCommand.PatchRepresentation);
            var lockName = $"representation-{patchRepresentationCommand.Id}";
            await _distributedLock.WaitLock(lockName, CancellationToken.None);
            try
            {
                var existingRepresentation = await _scimRepresentationCommandRepository.FindSCIMRepresentationById(patchRepresentationCommand.Id);
                if (existingRepresentation == null)
                {
                    throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, patchRepresentationCommand.Id));
                }

                var oldRepresentation = existingRepresentation.Clone() as SCIMRepresentation;
                var patchResult = existingRepresentation.ApplyPatches(patchRepresentationCommand.PatchRepresentation.Operations, _options.IgnoreUnsupportedCanonicalValues);
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
            finally
            {
                await _distributedLock.ReleaseLock(lockName, CancellationToken.None);
            }
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
