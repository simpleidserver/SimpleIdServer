// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
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

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            var result = _representations.FirstOrDefault(r => r.Id == representationId && r.ResourceType == resourceType);
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((SCIMRepresentation)result.Clone());
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attrSchemaId, string value, string endpoint = null)
        {
            return Task.FromResult(_representations.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueString == value)));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attrSchemaId, int value, string endpoint = null)
        {
            return Task.FromResult(_representations.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueInteger == value)));
        }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByAttributes(string attrSchemaId, IEnumerable<string> values, string endpoint = null)
        {
            return Task.FromResult(_representations.Where(r =>
            {
                return r.GetAttributesByAttrSchemaId(attrSchemaId).Any(a => values.Contains(a.ValueString) && (endpoint == null || endpoint == r.ResourceType));
            }));
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            var queryableRepresentations = _representations.AsQueryable().Where(r => r.ResourceType == parameter.ResourceType);
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            if (parameter.SortBy != null)
            {
                var evaluatedExpression = parameter.SortBy.Evaluate(queryableRepresentations);
                var ordered = (IEnumerable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
                queryableRepresentations = ordered.ToList().AsQueryable();
            }

            int totalResults = queryableRepresentations.Count();
            IEnumerable<SCIMRepresentation> result = new List<SCIMRepresentation>();
            if (parameter.Count > 0)
            {
                result = queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList();
            }

            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            IEnumerable<SCIMRepresentation> representations = _representations.AsQueryable().Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id));
            return Task.FromResult(representations);
        }
    }
}
