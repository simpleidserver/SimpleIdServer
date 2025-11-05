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

			var controllerNameCache = attributeMappingLst
				.GroupBy(m => m.TargetResourceType)
				.ToDictionary(
					g => g.Key,
					g => _resourceTypeResolver.ResolveByResourceType(g.Key).ControllerName
				);

			foreach (var representation in representationLst)
			{
				var referenceAttributeCache = new Dictionary<string, SCIMSchemaAttribute>();
				foreach (var schema in representation.Schemas)
				{
					foreach (var attr in schema.Attributes.Where(a => !string.IsNullOrEmpty(a.ParentId)))
					{
						if (attr.Name == "$ref" && !referenceAttributeCache.ContainsKey(attr.ParentId))
						{
							referenceAttributeCache[attr.ParentId] = attr;
						}
					}
				}

				var childrenByParentId = new Dictionary<string, List<SCIMRepresentationAttribute>>();
				foreach (var attr in representation.FlatAttributes)
				{
					if (!string.IsNullOrEmpty(attr.ParentAttributeId))
					{
						if (!childrenByParentId.ContainsKey(attr.ParentAttributeId))
						{
							childrenByParentId[attr.ParentAttributeId] = new List<SCIMRepresentationAttribute>();
						}
						childrenByParentId[attr.ParentAttributeId].Add(attr);
					}
				}

				foreach (var attributeMapping in attributeMappingLst)
				{
					var controllerName = controllerNameCache[attributeMapping.TargetResourceType];
					var attrs = representation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).ToList();

					foreach (var attr in attrs)
					{
						if (!childrenByParentId.TryGetValue(attr.Id, out var children))
						{
							continue;
						}

						var childrenLookup = children.ToLookup(v => v.SchemaAttribute.Name);
						var value = childrenLookup["value"].FirstOrDefault();
						var type = childrenLookup["type"].FirstOrDefault();
						var reference = childrenLookup["$ref"].FirstOrDefault();
						if (value == null || 
							string.IsNullOrWhiteSpace(value.ValueString) || 
							reference != null || 
							type == null || 
							type.ValueString != attributeMapping.TargetResourceType)
						{
							continue;
						}

						var schema = representation.GetSchemaByAttributeId(attr.SchemaAttributeId);
						if (schema == null)
						{
							continue;
						}

						if (!referenceAttributeCache.TryGetValue(attr.SchemaAttributeId, out var referenceAttribute))
						{
							continue;
						}

						representation.AddAttribute(attr, new SCIMRepresentationAttribute(
							Guid.NewGuid().ToString(), 
							Guid.NewGuid().ToString(), 
							referenceAttribute, 
							referenceAttribute.SchemaId)
						{
							ValueReference = $"{baseUrl}/{controllerName}/{value.ValueString}"
						});
					}
				}
			}
		}
	}
}
