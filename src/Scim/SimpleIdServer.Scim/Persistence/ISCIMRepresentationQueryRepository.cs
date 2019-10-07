using SimpleIdServer.Scim.Domain;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMRepresentationQueryRepository
    {
        Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType);
        Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, string value, string endpoint = null);
        Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, int value, string endpoint = null);
    }
}