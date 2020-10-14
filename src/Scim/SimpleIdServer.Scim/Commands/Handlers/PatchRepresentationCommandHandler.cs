// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
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
            var patches = ExtractPatchOperationsFromRequest(patchRepresentationCommand.Content);
            var lockName = $"representation-{patchRepresentationCommand.Id}";
            await _distributedLock.WaitLock(lockName, CancellationToken.None);
            try
            {
                var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(patchRepresentationCommand.Id);
                if (existingRepresentation == null)
                {
                    throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, patchRepresentationCommand.Id));
                }

                existingRepresentation.ApplyPatches(patches, _options.IgnoreUnsupportedCanonicalValues);
                existingRepresentation.SetUpdated(DateTime.UtcNow);
                using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
                {
                    await _scimRepresentationCommandRepository.Update(existingRepresentation);
                    await transaction.Commit();
                }

                return existingRepresentation;
            }
            finally
            {
                await _distributedLock.ReleaseLock(lockName, CancellationToken.None);
            }
        }

        private ICollection<SCIMPatchOperationRequest> ExtractPatchOperationsFromRequest(JObject content)
        {
            var requestedSchemas = content.GetSchemas();
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, SCIMConstants.StandardSCIMRepresentationAttributes.Schemas));
            }

            if (!requestedSchemas.SequenceEqual(new List<string> { SCIMConstants.StandardSchemas.PatchRequestSchemas.Id }))
            {
                throw new SCIMBadSyntaxException(Global.SchemasNotRecognized);
            }

            var operationsToken = content.SelectToken("Operations") as JArray;
            if (operationsToken == null)
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, SCIMConstants.StandardSCIMRepresentationAttributes.Operations));
            }

            var result = new List<SCIMPatchOperationRequest>();
            foreach(JObject record in operationsToken)
            {
                SCIMPatchOperations op;
                string path;
                if (record.TryGetEnum<SCIMPatchOperations>("op", out op) && record.TryGetString("path", out path))
                {
                    result.Add(new SCIMPatchOperationRequest(op, path, record.SelectToken("value")));
                }
            }

            return result;
        }
    }
}
