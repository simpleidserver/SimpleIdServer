using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class ReplaceRepresentationCommandHandler : IReplaceRepresentationCommandHandler
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationHelper _scimRepresentationHelper;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;

        public ReplaceRepresentationCommandHandler(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMRepresentationHelper scimRepresentationHelper, ISCIMRepresentationCommandRepository scimRepresentationCommandRepository)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationHelper = scimRepresentationHelper;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
        }

        public async Task<SCIMRepresentation> Handle(ReplaceRepresentationCommand replaceRepresentationCommand)
        {
            var requestedSchemas = replaceRepresentationCommand.Representation.GetSchemas();
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadRequestException("invalidRequest", $"{SCIMConstants.StandardSCIMRepresentationAttributes.Schemas} attribute is missing");
            }

            if (!replaceRepresentationCommand.SchemaIds.SequenceEqual(requestedSchemas))
            {
                throw new SCIMBadRequestException("invalidRequest", $"some schemas are not recognized by the endpoint");
            }

            var schemas = await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(requestedSchemas);
            var unsupportedSchemas = requestedSchemas.Where(s => !schemas.Any(sh => sh.Id == s));
            if (unsupportedSchemas.Any())
            {
                throw new SCIMBadRequestException("invalidRequest", $"{string.Join(",", unsupportedSchemas)} schemas are unknown");
            }

            var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(replaceRepresentationCommand.Id);
            if (existingRepresentation == null)
            {
                throw new SCIMNotFoundException("notFound", "Resource does not exist");
            }

            var updatedRepresentation = _scimRepresentationHelper.ExtractSCIMRepresentationFromJSON(replaceRepresentationCommand.Representation, schemas.ToList());
            foreach(var updatedAttribute in updatedRepresentation.Attributes)
            {
                if (updatedAttribute.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.IMMUTABLE)
                {
                    throw new SCIMImmutableAttributeException($"attribute {updatedAttribute.Id} is immutable");
                }

                if (updatedAttribute.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.WRITEONLY || updatedAttribute.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READWRITE)
                {
                    var existingAttribute = existingRepresentation.Attributes.FirstOrDefault(a => a.SchemaAttribute.Id == updatedAttribute.SchemaAttribute.Id);
                    if (existingAttribute == null)
                    {
                        existingRepresentation.AddAttribute(updatedAttribute);
                    }
                    else
                    {
                        existingRepresentation.Attributes.Remove(existingAttribute);
                        existingRepresentation.AddAttribute(updatedAttribute);
                    }
                }
            }

            _scimRepresentationCommandRepository.Update(existingRepresentation);
            await _scimRepresentationCommandRepository.SaveChanges();
            return existingRepresentation;
        }
    }
}
