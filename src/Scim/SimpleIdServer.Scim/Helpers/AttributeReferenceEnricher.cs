// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public class AttributeReferenceEnricher : IAttributeReferenceEnricher
	{
		private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
		private readonly IResourceTypeResolver _resourceTypeResolver;

		public AttributeReferenceEnricher(
			ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
			IResourceTypeResolver resourceTypeResolver)
		{
			_scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
			_resourceTypeResolver = resourceTypeResolver;
		}

		public async Task Enrich(string resourceType, IEnumerable<SCIMRepresentation> representationLst, string baseUrl)
		{
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			if (!attributeMappingLst.Any())
			{
				return;
			}

			foreach (var attributeMapping in attributeMappingLst)
			{
				foreach(var representation in representationLst)
                {
					var attrs = representation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).ToList();
					foreach(var attr in attrs)
                    {
						var values = representation.GetChildren(attr);
						var value = values.FirstOrDefault(v => v.SchemaAttribute.Name == "value");
						var type = values.FirstOrDefault(v => v.SchemaAttribute.Name == "type");
						var reference = values.FirstOrDefault(v => v.SchemaAttribute.Name == "$ref");
						var schema = representation.GetSchema(attr);
						var referenceAttribute = representation.GetSchema(attr).GetChildren(attr.SchemaAttribute).FirstOrDefault(v => v.Name == "$ref");
						if (value == null || string.IsNullOrWhiteSpace(value.ValueString) || reference != null || referenceAttribute == null || type == null || type.ValueString != attributeMapping.TargetResourceType)
                        {
							continue;
                        }

						var controllerName = _resourceTypeResolver.ResolveByResourceType(attributeMapping.TargetResourceType).ControllerName;
						representation.AddAttribute(attr, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), referenceAttribute, referenceAttribute.SchemaId)
						{
							ValueReference = $"{baseUrl}/{controllerName}/{value.ValueString}"
						});
                    }
                }
			}
		}
	}
}
