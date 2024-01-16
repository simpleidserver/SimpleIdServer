// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
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
                var filteredRepresentationAttributes = from a in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable().Where(a => parameter.SchemaNames.Contains(a.Namespace) || a.ResourceType == parameter.ResourceType)
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

            
            var filteredRepresentations = _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => r.ResourceType == parameter.ResourceType);
            if(filteredRepresentationIds != null)
            {
                filteredRepresentations = filteredRepresentations.Where(r => filteredRepresentationIds.Contains(r.Id));
            }

            var paginationResult = await OrderByAndPaginate(filteredRepresentations, parameter);
            filteredRepresentations = paginationResult.Query;
            var filteredRepresentationsWithAttributes = from a in filteredRepresentations
                  join b in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable() on a.Id equals b.RepresentationId into Attributes
                  select new
                  {
                      Representation = a,
                      Attributes = Attributes
                  };
            var representationWithAttributes = await filteredRepresentationsWithAttributes
                .ToListAsync();
            if(paginationResult.OrderedRepresentationIds != null)
            {
                representationWithAttributes = representationWithAttributes.Select(r => new
                {
                    Representation = r,
                    Order = paginationResult.OrderedRepresentationIds.IndexOf(r.Representation.Id)
                })
                .OrderBy(r => r.Order)
                .Select(r => r.Representation)
                .ToList();
            }

            var representations = new List<SCIMRepresentation>();
            foreach(var record in representationWithAttributes)
            {
                var representation = record.Representation;
                representation.FlatAttributes = record.Attributes.ToList();
                representations.Add(representation);
            }

            representations.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
            return new SearchSCIMRepresentationsResponse(total, representations);
        }

        private async Task<PaginationResult> OrderByAndPaginate(
            IMongoQueryable<SCIMRepresentationModel> representations, 
            SearchSCIMRepresentationsParameter parameter)
        {
            if (parameter.SortBy == null) return new PaginationResult
            {
                Query = representations
                    .OrderBy(r => r.Id)
                    .Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1)
                    .Take(parameter.Count)
            };
            var sortBy = parameter.SortBy as SCIMAttributeExpression;
            var order = parameter.SortOrder ?? SearchSCIMRepresentationOrders.Ascending;
            var result = OrderByMetadataAndPaginate(sortBy,
                representations,
                order,
                parameter.StartIndex,
                parameter.Count);
            if (result != null) return result;
            return await OrderByPropertyAndPaginate(
                representations,
                sortBy,
                parameter.SchemaNames,
                order,
                parameter.StartIndex,
                parameter.Count);
        }

        private PaginationResult OrderByMetadataAndPaginate(
            SCIMAttributeExpression attrExpression, 
            IMongoQueryable<SCIMRepresentationModel> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count)
        {
            var fullPath = attrExpression.GetFullPath();
            IMongoQueryable<SCIMRepresentationModel> result = null; 
            switch (fullPath)
            {
                case StandardSCIMRepresentationAttributes.Id:
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.Id) : representations.OrderByDescending(r => r.Id);
                    break;
                case StandardSCIMRepresentationAttributes.ExternalId:
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.ExternalId) : representations.OrderByDescending(r => r.ExternalId);
                    break;
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}":
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.ResourceType) : representations.OrderByDescending(r => r.ResourceType);
                    break;
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}":
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.Created) : representations.OrderByDescending(r => r.Created);
                    break;
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}":
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.LastModified) : representations.OrderByDescending(r => r.LastModified);
                    break;
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}":
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.Version) : representations.OrderByDescending(r => r.Version);
                    break;
                case StandardSCIMRepresentationAttributes.DisplayName:
                    result = order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.DisplayName) : representations.OrderByDescending(r => r.DisplayName);
                    break;
                default:
                    return null;
            }

            return new PaginationResult
            {
                Query = result.Skip(startIndex <= 1 ? 0 : startIndex - 1).Take(count)
            };
        }

        private async Task<PaginationResult> OrderByPropertyAndPaginate(
            IMongoQueryable<SCIMRepresentationModel> representations,
            SCIMAttributeExpression attrExpression,
            List<string> schemaNames,
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count)
        {
            var fullPath = attrExpression.GetFullPath();
            var attributes = _scimDbContext.SCIMRepresentationAttributeLst
                .AsQueryable()
                .Where(r => r.FullPath == fullPath && schemaNames.Contains(r.Namespace));
            var lastExpr = attrExpression.GetLastChild();
            switch(lastExpr.SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.STRING:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueString) : attributes.OrderByDescending(a => a.ValueString);
                    break;
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueBoolean) : attributes.OrderByDescending(a => a.ValueBoolean);
                    break;
                case SCIMSchemaAttributeTypes.DECIMAL:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueDecimal) : attributes.OrderByDescending(a => a.ValueDecimal);
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueInteger) : attributes.OrderByDescending(a => a.ValueInteger);
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueDateTime) : attributes.OrderByDescending(a => a.ValueDateTime);
                    break;
                case SCIMSchemaAttributeTypes.BINARY:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueBinary) : attributes.OrderByDescending(a => a.ValueBinary);
                    break;
                case SCIMSchemaAttributeTypes.REFERENCE:
                    attributes = order == SearchSCIMRepresentationOrders.Ascending ? attributes.OrderBy(a => a.ValueReference) : attributes.OrderByDescending(a => a.ValueReference);
                    break;
            }

            var representationIds = (await attributes
                .Select(a => a.RepresentationId)
                .Skip(startIndex <= 1 ? 0 : startIndex - 1)
                .Take(count)
                .ToListAsync())
                .Distinct()
                .ToList();
             return new PaginationResult
             {
                 Query = representations.Where(r => representationIds.Contains(r.Id)),
                 OrderedRepresentationIds = representationIds
             };
        }

        private record PaginationResult
        {
            public IMongoQueryable<SCIMRepresentationModel> Query { get; set; }
            public List<string> OrderedRepresentationIds { get; set; }
        }
    }
}
