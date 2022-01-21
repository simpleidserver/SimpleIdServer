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
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;
        private readonly IDistributedLock _distributedLock;

        public ReplaceRepresentationCommandHandler(
            ISCIMSchemaQueryRepository scimSchemaQueryRepository,
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository,
            ISCIMRepresentationHelper scimRepresentationHelper,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IDistributedLock distributedLock,
            IBusControl busControl) : base(busControl)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
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
                var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(replaceRepresentationCommand.Id);
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
                existingRepresentation.RemoveAttributesBySchemaAttrId(updatedRepresentation.FlatAttributes.Select(_ => _.SchemaAttribute.Id));
                foreach (var updatedAttribute in updatedRepresentation.FlatAttributes)
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

                existingRepresentation.SetDisplayName(updatedRepresentation.DisplayName);
                existingRepresentation.SetExternalId(updatedRepresentation.ExternalId);
                existingRepresentation.SetUpdated(DateTime.UtcNow);
                var isReferenceProperty = await _representationReferenceSync.IsReferenceProperty(replaceRepresentationCommand.Representation.Attributes.GetKeys());
                var references = await _representationReferenceSync.Sync(replaceRepresentationCommand.ResourceType, oldRepresentation, existingRepresentation, !isReferenceProperty);
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
    }
}
