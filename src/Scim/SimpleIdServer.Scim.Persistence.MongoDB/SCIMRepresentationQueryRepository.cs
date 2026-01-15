// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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

        public SCIMRepresentationQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter, CancellationToken cancellationToken)
        {
            List<string> filteredRepresentationIds = null;
            int total = 0;
            if (parameter.Filter != null)
            {
                var filteredRepresentationAttributes = _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable();
                filteredRepresentationAttributes = parameter.Filter.EvaluateMongoDbAttributesDirect(filteredRepresentationAttributes);
                if (filteredRepresentationAttributes != null)
                {
                    filteredRepresentationIds = (await
                        filteredRepresentationAttributes
                        .Select(a => a.RepresentationId)
                        .ToMongoListAsync())
                        .Distinct()
                        .ToList();
                    total = filteredRepresentationIds.Count;
                }
            }
            else
            {
                total = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                    .Where(s => s.ResourceType == parameter.ResourceType)
                    .CountAsync(cancellationToken);
            }

            if (parameter.Count == 0)
                return new SearchSCIMRepresentationsResponse(total, []);

            var filteredRepresentations = _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => r.ResourceType == parameter.ResourceType);
            if (!string.IsNullOrWhiteSpace(parameter.Realm))
                filteredRepresentations = filteredRepresentations.Where(r => r.RealmName == parameter.Realm);
            if (parameter.Filter != null)
            {
                var tmp = parameter.Filter.EvaluateMongoDbRepresentations(filteredRepresentations);
                if (tmp != null)
                {
                    var tmpIds = await tmp.Select(r => r.Id).ToListAsync(cancellationToken);
                    if (filteredRepresentationIds == null) filteredRepresentationIds = [.. tmpIds];
                    else
                    {
                        if (parameter.Filter is SCIMLogicalExpression logical)
                        {
                            if (logical.LogicalOperator == Parser.Operators.SCIMLogicalOperators.AND)
                                filteredRepresentationIds = [.. filteredRepresentationIds.Intersect(tmpIds)];
                            else
                                filteredRepresentationIds = [.. filteredRepresentationIds.Union(tmpIds)];
                        }
                    }

                    total = filteredRepresentationIds.Count;
                }
            }

            if (filteredRepresentationIds != null)
            {
                filteredRepresentations = filteredRepresentations.Where(r => filteredRepresentationIds.Contains(r.Id));
            }

            var paginationResult = await OrderByAndPaginate(filteredRepresentations, parameter, filteredRepresentationIds);
            filteredRepresentations = paginationResult.Query;
            
            var representationsList = await filteredRepresentations.ToListAsync(cancellationToken);
            var representationIds = representationsList.Select(r => r.Id).ToList();
            
            var attributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => representationIds.Contains(a.RepresentationId))
                .ToListAsync(cancellationToken);
            
            var attributesByRepId = attributes.GroupBy(a => a.RepresentationId)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            var representationWithAttributes = representationsList.Select(rep => new
            {
                Representation = rep,
                Attributes = attributesByRepId.ContainsKey(rep.Id) ? attributesByRepId[rep.Id] : new List<SCIMRepresentationAttribute>()
            }).ToList();
            
            if (paginationResult.OrderedRepresentationIds != null)
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
            foreach (var record in representationWithAttributes)
            {
                var representation = record.Representation;
                representation.FlatAttributes = [.. record.Attributes];
                representations.Add(representation);
            }

            representations.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
            return new SearchSCIMRepresentationsResponse(total, representations);
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string realm, string representationId, string resourceType, GetSCIMResourceParameter parameter, CancellationToken cancellationToken)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            SCIMRepresentationModel result = null;
            if(!string.IsNullOrWhiteSpace(realm))
                result = await collection.AsQueryable().Where(a => a.Id == representationId && a.ResourceType == resourceType && a.RealmName == realm).ToMongoFirstAsync();
            else
                result = await collection.AsQueryable().Where(a => a.Id == representationId && a.ResourceType == resourceType).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            await result.IncludeAll(_scimDbContext);
            return result;
        }

        private async Task<PaginationResult> OrderByAndPaginate(
            IMongoQueryable<SCIMRepresentationModel> representations, 
            SearchSCIMRepresentationsParameter parameter,
            List<string> representationIds)
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
                parameter.Count,
                representationIds);
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
            int count,
            List<string> ids)
        {
            var fullPath = attrExpression.GetFullPath();
            var attributes = _scimDbContext.SCIMRepresentationAttributeLst
                .AsQueryable()
                .Where(r => r.FullPath == fullPath && schemaNames.Contains(r.Namespace));
            if (ids != null)
                attributes = attributes.Where(a => ids.Contains(a.RepresentationId));

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
