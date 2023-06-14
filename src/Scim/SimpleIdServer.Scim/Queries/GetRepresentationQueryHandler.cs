// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.Scim.Domain;
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
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Queries
{
    public class GetRepresentationQueryHandler : IGetRepresentationQueryHandler
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ILogger<GetRepresentationQueryHandler> _logger;

        public GetRepresentationQueryHandler(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ILogger<GetRepresentationQueryHandler> logger)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _logger = logger;
        }

        public async virtual Task<GenericResult<SCIMRepresentation>> Handle(string id, GetSCIMResourceRequest parameter, string resourceType)
        {
            var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(resourceType);
            if (schema == null) throw new SCIMNotFoundException();
            var schemaIds = new List<string> { schema.Id };
            schemaIds.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
            var schemas = (await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(schemaIds)).ToList();
            var standardSchemas = new List<SCIMSchema>
                {
                    StandardSchemas.StandardResponseSchemas
                };
            standardSchemas.AddRange(schemas);
            var includedAttributes = parameter.Attributes == null ? new List<SCIMAttributeExpression>() : parameter.Attributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
            var excludedAttributes = parameter.ExcludedAttributes == null ? new List<SCIMAttributeExpression>() : parameter.ExcludedAttributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
            var representation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(id, resourceType, new GetSCIMResourceParameter { ExcludedAttributes = excludedAttributes, IncludedAttributes = includedAttributes });
            if (representation == null)
            {
                _logger.LogError(string.Format(Global.ResourceNotFound, id));
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, id));
            }

            if (!includedAttributes.Any() && !excludedAttributes.Any())
                representation.ApplyEmptyArray();
            return GenericResult<SCIMRepresentation>.Ok(representation);
        }
    }
}
