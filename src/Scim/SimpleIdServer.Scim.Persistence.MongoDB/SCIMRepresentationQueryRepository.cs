// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly SCIMDbContext _scimDbContext;
        private readonly MongoDbOptions _options;

        public SCIMRepresentationQueryRepository(SCIMDbContext scimDbContext, IOptions<MongoDbOptions> options)
        {
            _scimDbContext = scimDbContext;
            _options = options.Value;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, CancellationToken cancellationToken)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType, CancellationToken cancellationToken)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId && a.ResourceType == resourceType).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType, GetSCIMResourceParameter parameter, CancellationToken cancellationToken)
        {
            return FindSCIMRepresentationById(representationId, resourceType, cancellationToken);
        }

        public async Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter, CancellationToken cancellationToken)
        {
            List<string> filteredRepresentationIds = null;
            int total = 0;
            if (parameter.Filter != null)
            {
                IQueryable<EnrichedAttribute> filteredRepresentationAttributes = from a in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable().Where(a => parameter.SchemaNames.Contains(a.Namespace))
                    join b in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable() on a.ParentAttributeId equals b.Id into Parents
                    select new EnrichedAttribute
                    {
                        Attribute = a,
                        Parent = Parents.First(),
                        Children = new List<SCIMRepresentationAttribute>()
                    };
                filteredRepresentationIds = (await parameter.Filter
                    .EvaluateMongoDbAttributes(filteredRepresentationAttributes)
                    .Select(a => a.Attribute.RepresentationId)
                    .ToMongoListAsync())
                    .Distinct()
                    .ToList();
                total = filteredRepresentationIds.Count();
            }
            else
            {
                total = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                    .Where(s => s.ResourceType == parameter.ResourceType)
                    .CountAsync();
            }


            var filteredRepresentations = _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(s => s.ResourceType == parameter.ResourceType);
            if(filteredRepresentationIds != null)
                filteredRepresentations = filteredRepresentations.Where(r => filteredRepresentationIds.Contains(r.Id));

            if (parameter.SortBy == null)
                filteredRepresentations = filteredRepresentations.OrderBy(s => s.Id);
            else
            {
                filteredRepresentations = parameter.SortBy.EvaluateMongoDbOrderBy(
                    filteredRepresentations,
                    parameter.SortOrder ?? SearchSCIMRepresentationOrders.Descending);
            }

            var representations = await filteredRepresentations
                .Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1)
                .Take(parameter.Count)
                .ToListAsync();
            var representationIds = representations.Select(r => r.Id);
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => representationIds.Contains(a.RepresentationId))
                .ToListAsync();
            foreach (var representation in representations) 
                representation.FlatAttributes = representationAttributes.Where(a => a.RepresentationId == representation.Id).ToList();
            representations.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
            return new SearchSCIMRepresentationsResponse(total, representations);
        }
    }
}
