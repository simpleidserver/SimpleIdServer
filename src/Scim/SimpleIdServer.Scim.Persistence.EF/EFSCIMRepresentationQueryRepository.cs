using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMRepresentationQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, string value, string endpoint = null)
        {
            return _scimDbContext.SCIMRepresentationLst.FirstOrDefaultAsync(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attributeId && a.ValuesString.Contains(value)));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, int value, string endpoint = null)
        {
            return _scimDbContext.SCIMRepresentationLst.FirstOrDefaultAsync(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attributeId && a.ValuesInteger.Contains(value)));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            return _scimDbContext.SCIMRepresentationLst.FirstOrDefaultAsync(r => r.Id == representationId);
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            return _scimDbContext.SCIMRepresentationLst.FirstOrDefaultAsync(r => r.Id == representationId && r.ResourceType == resourceType);
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            IQueryable<SCIMRepresentation> queryableRepresentations = _scimDbContext.SCIMRepresentationLst;
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            int totalResults = queryableRepresentations.Count();
            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList()));
        }
    }
}
