// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public class AttributeReferenceEnricher : IAttributeReferenceEnricher
	{
		private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;

		public AttributeReferenceEnricher(ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository)
		{
			_scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
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
					var attrs = representation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId);
					foreach(var attr in attrs)
                    {
						var values = representation.GetChildren(attr);
						var value = values.FirstOrDefault(v => v.SchemaAttribute.Name == "value");
						var reference = values.FirstOrDefault(v => v.SchemaAttribute.Name == "$ref");
						if (value == null || string.IsNullOrWhiteSpace(value.ValueString) || reference == null)
                        {
							continue;
                        }

						representation.AddAttribute(attr, new SCIMRepresentationAttribute
						{
							SchemaAttribute = new SCIMSchemaAttribute(reference.Id)
							{
								Name = "$ref",
								MultiValued = false,
								Type = SCIMSchemaAttributeTypes.STRING
							},
							ValueString = $"{baseUrl}/{attributeMapping.TargetResourceType}/{value.ValueString}"
						});
                    }
                }
			}
		}
	}
}
