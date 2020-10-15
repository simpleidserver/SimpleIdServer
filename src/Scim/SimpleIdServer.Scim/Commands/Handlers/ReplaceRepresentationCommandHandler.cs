// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
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
    public class ReplaceRepresentationCommandHandler : IReplaceRepresentationCommandHandler
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IDistributedLock _distributedLock;

        public ReplaceRepresentationCommandHandler(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMRepresentationHelper scimRepresentationHelper, ISCIMRepresentationCommandRepository scimRepresentationCommandRepository, IDistributedLock distributedLock)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _distributedLock = distributedLock;
        }

        public async Task<SCIMRepresentation> Handle(ReplaceRepresentationCommand replaceRepresentationCommand)
        {
            var requestedSchemas = replaceRepresentationCommand.Representation.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, SCIMConstants.StandardSCIMRepresentationAttributes.Schemas));
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
                var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(replaceRepresentationCommand.Id);
                if (existingRepresentation == null)
                {
                    throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, replaceRepresentationCommand.Id));
                }

                var updatedRepresentation = _scimRepresentationHelper.ExtractSCIMRepresentationFromJSON(replaceRepresentationCommand.Representation.Attributes, replaceRepresentationCommand.Representation.ExternalId, schemas.ToList());
                existingRepresentation.RemoveAttributes(updatedRepresentation.Attributes.Select(_ => _.SchemaAttribute.Id));
                foreach (var updatedAttribute in updatedRepresentation.Attributes)
                {
                    if (updatedAttribute.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.IMMUTABLE)
                    {
                        throw new SCIMImmutableAttributeException(string.Format(Global.AttributeImmutable, updatedAttribute.Id));
                    }

                    if (updatedAttribute.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.WRITEONLY || updatedAttribute.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READWRITE)
                    {
                        existingRepresentation.AddAttribute(updatedAttribute);
                    }
                }

                existingRepresentation.SetExternalId(updatedRepresentation.ExternalId);
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
    }
}
