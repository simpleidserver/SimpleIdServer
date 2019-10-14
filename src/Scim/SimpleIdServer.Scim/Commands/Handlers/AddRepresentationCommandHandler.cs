using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class AddRepresentationCommandHandler : IAddRepresentationCommandHandler
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;

        public AddRepresentationCommandHandler(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMRepresentationHelper scimRepresentationHelper, ISCIMRepresentationCommandRepository scimRepresentationCommandRepository)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
        }

        public async Task<SCIMRepresentation> Handle(AddRepresentationCommand addRepresentationCommand)
        {
            var requestedSchemas = addRepresentationCommand.Representation.GetSchemas();
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadRequestException("invalidRequest", $"{SCIMConstants.StandardSCIMRepresentationAttributes.Schemas} attribute is missing");
            }

            if (!addRepresentationCommand.SchemaIds.SequenceEqual(requestedSchemas))
            {
                throw new SCIMBadRequestException("invalidRequest", $"some schemas are not recognized by the endpoint");
            }

            var schemas = await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            var unsupportedSchemas = requestedSchemas.Where(s => !schemas.Any(sh => sh.Id == s));
            if (unsupportedSchemas.Any())
            {
                throw new SCIMBadRequestException("invalidRequest", $"{string.Join(",", unsupportedSchemas)} schemas are unknown");
            }

            var scimRepresentation = _scimRepresentationHelper.ExtractSCIMRepresentationFromJSON(addRepresentationCommand.Representation, schemas.ToList());
            var uniqueServerAttributeIds = scimRepresentation.Attributes.Where(a => a.SchemaAttribute.MultiValued == false && a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.SERVER);
            var uniqueGlobalAttributes = scimRepresentation.Attributes.Where(a => a.SchemaAttribute.MultiValued == false && a.SchemaAttribute.Uniqueness == SCIMSchemaAttributeUniqueness.GLOBAL);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueServerAttributeIds, addRepresentationCommand.ResourceType);
            await CheckSCIMRepresentationExistsForGivenUniqueAttributes(uniqueGlobalAttributes);
            scimRepresentation.Id = Guid.NewGuid().ToString();
            scimRepresentation.Version = Guid.NewGuid().ToString();
            scimRepresentation.Created = DateTime.UtcNow;
            scimRepresentation.LastModified = DateTime.UtcNow;
            scimRepresentation.ResourceType = addRepresentationCommand.ResourceType;
            _scimRepresentationCommandRepository.Add(scimRepresentation);
            await _scimRepresentationCommandRepository.SaveChanges();
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
                        record = await _scimRepresentationQueryRepository.FindSCIMRepresentationByAttribute(attribute.SchemaAttribute.Id, attribute.ValuesString.First(), endpoint);
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        record = await _scimRepresentationQueryRepository.FindSCIMRepresentationByAttribute(attribute.SchemaAttribute.Id, attribute.ValuesInteger.First(), endpoint);
                        break;
                }

                if (record != null)
                {
                    throw new SCIMUniquenessAttributeException("uniqueness", $"attribute {attribute.SchemaAttribute.Name} must be unique");
                }
            }
        }
    }
}