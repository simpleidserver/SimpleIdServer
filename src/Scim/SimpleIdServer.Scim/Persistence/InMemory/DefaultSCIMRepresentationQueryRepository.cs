using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly List<SCIMRepresentation> _representations;

        public DefaultSCIMRepresentationQueryRepository(List<SCIMRepresentation> representations)
        {
            _representations = representations;
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            return Task.FromResult(_representations.FirstOrDefault(r => r.Id == representationId));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, string value, string endpoint = null)
        {
            return Task.FromResult(_representations.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attributeId && a.ValuesString.Contains(value))));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, int value, string endpoint = null)
        {
            return Task.FromResult(_representations.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attributeId && a.ValuesInteger.Contains(value))));
        }
    }
}
