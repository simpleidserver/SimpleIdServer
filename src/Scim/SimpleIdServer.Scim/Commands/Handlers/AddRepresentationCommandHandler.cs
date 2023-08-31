// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class AddRepresentationCommandHandler : BaseCommandHandler, IAddRepresentationCommandHandler
    {
        private readonly ISCIMSchemaCommandRepository _scimSchemaCommandRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;

        public AddRepresentationCommandHandler(
            ISCIMSchemaCommandRepository scimSchemaCommandRepository,
            ISCIMRepresentationHelper scimRepresentationHelper,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IBusControl busControl) : base(busControl)
        {
            _scimSchemaCommandRepository = scimSchemaCommandRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
        }

        public async virtual Task<GenericResult<string>> Handle(AddRepresentationCommand addRepresentationCommand)
        {
            var requestedSchemas = addRepresentationCommand.Representation.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Schemas));
            }

            var schema = await _scimSchemaCommandRepository.FindRootSCIMSchemaByResourceType(addRepresentationCommand.ResourceType);
            if (schema == null) throw new SCIMSchemaNotFoundException();
            var allSchemas = new List<string> { schema.Id };
            var requiredSchemas = new List<string> { schema.Id };
            allSchemas.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
            requiredSchemas.AddRange(schema.SchemaExtensions.Where(s => s.Required).Select(s => s.Schema));
            var missingRequiredSchemas = requiredSchemas.Where(s => !requestedSchemas.Contains(s));
            if (missingRequiredSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.RequiredSchemasAreMissing, string.Join(",", missingRequiredSchemas)));
            }

            var unsupportedSchemas = requestedSchemas.Where(s => !allSchemas.Contains(s));
            if (unsupportedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.SchemasAreUnknown, string.Join(",", unsupportedSchemas)));
            }

            var schemas = await _scimSchemaCommandRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            var scimRepresentation = _scimRepresentationHelper.ExtractSCIMRepresentationFromJSON(addRepresentationCommand.Representation.Attributes, addRepresentationCommand.Representation.ExternalId, schema, schemas.Where(s => s.Id != schema.Id).ToList());
            scimRepresentation.Id = Guid.NewGuid().ToString();
            scimRepresentation.SetCreated(DateTime.UtcNow);
            scimRepresentation.SetUpdated(DateTime.UtcNow);
            scimRepresentation.SetVersion(0);
            scimRepresentation.SetResourceType(addRepresentationCommand.ResourceType);
            var uniqueServerAttributeIds = scimRepresentation.FlatAttributes.Where(s => s.IsLeaf()).Where(a => a.SchemaAttribute.MultiValued == false && a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.SERVER);
            var uniqueGlobalAttributes = scimRepresentation.FlatAttributes.Where(s => s.IsLeaf()).Where(a => a.SchemaAttribute.MultiValued == false && a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.GLOBAL);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueServerAttributeIds, addRepresentationCommand.ResourceType);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueGlobalAttributes);
            var references = await _representationReferenceSync.Sync(addRepresentationCommand.ResourceType, new SCIMRepresentation(), scimRepresentation, addRepresentationCommand.Location, schema);
            await using (var transaction = await _scimRepresentationCommandRepository.StartTransaction().ConfigureAwait(false))
            {
                await _scimRepresentationCommandRepository.Add(scimRepresentation).ConfigureAwait(false);
                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkInsert(reference.AddedRepresentationAttributes).ConfigureAwait(false);
                }

                await transaction.Commit().ConfigureAwait(false);
                await NotifyAllReferences(references).ConfigureAwait(false);
            }

            return GenericResult<string>.Ok(scimRepresentation.Id);
        }

        private async Task CheckSCIMRepresentationExistsForGivenUniqueAttributes(IEnumerable<SCIMRepresentationAttribute> attributes, string endpoint = null)
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

                if (record != null)
                {
                    throw new SCIMUniquenessAttributeException(string.Format(Global.AttributeMustBeUnique, attribute.SchemaAttribute.Name));
                }
            }
        }
    }
}