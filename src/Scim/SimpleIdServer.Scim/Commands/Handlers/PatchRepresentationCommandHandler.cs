// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
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
    public class PatchRepresentationCommandHandler : IPatchRepresentationCommandHandler
    {
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IDistributedLock _distributedLock;
        private readonly SCIMHostOptions _options;

        public PatchRepresentationCommandHandler(ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMRepresentationCommandRepository scimRepresentationCommandRepository, IDistributedLock distributedLock, IOptions<SCIMHostOptions> options)
        {
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _distributedLock = distributedLock;
            _options = options.Value;
        }

        public async Task<SCIMRepresentation> Handle(PatchRepresentationCommand patchRepresentationCommand)
        {
            CheckParameter(patchRepresentationCommand.PatchRepresentation);
            var lockName = $"representation-{patchRepresentationCommand.Id}";
            await _distributedLock.WaitLock(lockName, CancellationToken.None);
            try
            {
                var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(patchRepresentationCommand.Id);
                if (existingRepresentation == null)
                {
                    throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, patchRepresentationCommand.Id));
                }

                existingRepresentation.ApplyPatches(patchRepresentationCommand.PatchRepresentation.Operations, _options.IgnoreUnsupportedCanonicalValues);
                existingRepresentation.SetUpdated(DateTime.UtcNow);
                using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
                {
                    await _scimRepresentationCommandRepository.Update(existingRepresentation);
                    await transaction.Commit();
                }

                existingRepresentation.ApplyEmptyArray();
                return existingRepresentation;
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
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, SCIMConstants.StandardSCIMRepresentationAttributes.Schemas));
            }

            if (!requestedSchemas.SequenceEqual(new List<string> { SCIMConstants.StandardSchemas.PatchRequestSchemas.Id }))
            {
                throw new SCIMBadSyntaxException(Global.SchemasNotRecognized);
            }

            if (patchRepresentation.Operations == null)
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, SCIMConstants.StandardSCIMRepresentationAttributes.Operations));
            }
        }
    }
}
