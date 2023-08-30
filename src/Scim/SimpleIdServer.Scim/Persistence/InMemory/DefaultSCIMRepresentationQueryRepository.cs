// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly List<SCIMRepresentation> _representations;
        private readonly List<SCIMRepresentationAttribute> _attributes;

        public DefaultSCIMRepresentationQueryRepository(List<SCIMRepresentation> representations, List<SCIMRepresentationAttribute> attributes)
        {
            _representations = representations;
            _attributes = attributes;
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var result = _representations.FirstOrDefault(r => r.Id == representationId);
            if (result == null)
            {
                return Task.FromResult((SCIMRepresentation)null);
            }

            return Task.FromResult(Enrich(result));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            var result = _representations.FirstOrDefault(r => r.Id == representationId && r.ResourceType == resourceType);
            if (result == null)
            {
                return Task.FromResult((SCIMRepresentation)null);
            }

            var clone = (SCIMRepresentation)result.Clone();
            return Task.FromResult(Enrich(result));
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType, GetSCIMResourceParameter parameter)
        {
            var result = _representations.FirstOrDefault(r => r.Id == representationId && r.ResourceType == resourceType);
            if (result == null)
            {
                return Task.FromResult(result);
            }

            var clone = Enrich(result);
            clone.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
            return Task.FromResult(clone);
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            var queryableRepresentations = _representations.Select(r => Enrich(r)).AsQueryable().Where(r => r.ResourceType == parameter.ResourceType);
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
            result = queryableRepresentations.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count).Select(r => (SCIMRepresentation)r.Clone()).ToList();
            result.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            IEnumerable<SCIMRepresentation> representations = _representations.AsQueryable().Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id));
            return Task.FromResult(representations);
        }

        private SCIMRepresentation Enrich(SCIMRepresentation representation)
        {
            var clone = (SCIMRepresentation)representation.Clone();
            clone.FlatAttributes = _attributes.Where(a => a.RepresentationId == representation.Id).ToList();
            return clone;
        }
    }
}
