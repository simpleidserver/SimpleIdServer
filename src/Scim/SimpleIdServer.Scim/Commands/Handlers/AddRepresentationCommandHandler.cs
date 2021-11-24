// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.Domain;
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
    public class AddRepresentationCommandHandler : BaseCommandHandler, IAddRepresentationCommandHandler
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;

        public AddRepresentationCommandHandler(
            ISCIMSchemaQueryRepository scimSchemaQueryRepository, 
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, 
            ISCIMRepresentationHelper scimRepresentationHelper, 
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IBusControl busControl) : base(busControl)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
        }

        public async Task<SCIMRepresentation> Handle(AddRepresentationCommand addRepresentationCommand)
        {
            var requestedSchemas = addRepresentationCommand.Representation.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, SCIMConstants.StandardSCIMRepresentationAttributes.Schemas));
            }

            var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(addRepresentationCommand.ResourceType);
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

            var schemas = await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
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
            var references = await _representationReferenceSync.Sync(addRepresentationCommand.ResourceType, new SCIMRepresentation(), scimRepresentation);
            using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
            {
                await _scimRepresentationCommandRepository.Add(scimRepresentation);
                foreach(var reference in references.Representations)
                {
                    await _scimRepresentationCommandRepository.Update(reference);
                }

                await transaction.Commit();
            }

            await Notify(references);
            scimRepresentation.ApplyEmptyArray();
            return scimRepresentation;
        }

        private async Task CheckSCIMRepresentationExistsForGivenUniqueAttributes(IEnumerable<SCIMRepresentationAttribute> attributes, string endpoint = null)
        {
            foreach(var attribute in attributes)
            {
                SCIMRepresentation record = null;
                switch (attribute.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        record = await _scimRepresentationQueryRepository.FindSCIMRepresentationByAttribute(attribute.SchemaAttribute.Id, attribute.ValueString, endpoint);
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        if (attribute.ValueInteger != null)
                        {
                            record = await _scimRepresentationQueryRepository.FindSCIMRepresentationByAttribute(attribute.SchemaAttribute.Id, attribute.ValueInteger.Value, endpoint);
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