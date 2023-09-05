// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Queries
{
    public class SearchRepresentationsQueryHandler : ISearchRepresentationsQueryHandler
    {
        private readonly SCIMHostOptions _options;
        private readonly ILogger<SearchRepresentationsQueryHandler> _logger;
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;

        public SearchRepresentationsQueryHandler(ILogger<SearchRepresentationsQueryHandler> logger, IOptionsMonitor<SCIMHostOptions> options, ISCIMSchemaQueryRepository scimSchemaQueryRepository, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository)
        {
            _options = options.CurrentValue;
            _logger = logger;
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
        }

        public virtual async Task<GenericResult<SearchSCIMRepresentationsResponse>> Handle(SearchSCIMResourceParameter searchRequest, string resourceType, CancellationToken cancellationToken)
        {
            if (searchRequest == null) throw new SCIMBadSyntaxException(Global.HttpPostNotWellFormatted);
            if (searchRequest.Count > _options.MaxResults || searchRequest.Count == null) searchRequest.Count = _options.MaxResults;
            var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(resourceType);
            if (schema == null) throw new SCIMNotFoundException();
            var schemaIds = new List<string> { schema.Id };
            schemaIds.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
            var schemas = (await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(schemaIds)).ToList();
            var sortByFilter = SCIMFilterParser.Parse(searchRequest.SortBy, schemas);
            if (searchRequest.StartIndex <= 0)
            {
                _logger.LogError(Global.StartIndexMustBeSuperiorOrEqualTo1);
                throw new SCIMBadSyntaxException(Global.StartIndexMustBeSuperiorOrEqualTo1);
            }

            var standardSchemas = new List<SCIMSchema>
            {
                StandardSchemas.StandardResponseSchemas
            };
            standardSchemas.AddRange(schemas);
            var includedAttributes = searchRequest.Attributes == null ? new List<SCIMAttributeExpression>() : searchRequest.Attributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
            var excludedAttributes = searchRequest.ExcludedAttributes == null ? new List<SCIMAttributeExpression>() : searchRequest.ExcludedAttributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
            var result = await _scimRepresentationQueryRepository.FindSCIMRepresentations(new SearchSCIMRepresentationsParameter(resourceType, searchRequest.StartIndex, searchRequest.Count.Value, sortByFilter, searchRequest.SortOrder, SCIMFilterParser.Parse(searchRequest.Filter, schemas), includedAttributes, excludedAttributes), cancellationToken);
            var representations = result.Content.ToList();
            foreach (var representation in representations) representation.Schemas = schemas;
            return GenericResult<SearchSCIMRepresentationsResponse>.Ok(result);
        }
    }
}
