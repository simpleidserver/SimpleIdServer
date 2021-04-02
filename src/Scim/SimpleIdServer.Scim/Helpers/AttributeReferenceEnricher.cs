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
		private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;

		public AttributeReferenceEnricher(ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository)
		{
			_scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
			_scimRepresentationQueryRepository = scimRepresentationQueryRepository;
		}

		public async Task Enrich(string resourceType, IEnumerable<SCIMRepresentation> representationLst, string baseUrl)
		{
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			if (!attributeMappingLst.Any())
			{
				return;
			}

			var ids = representationLst.Select(r => r.Id);
			foreach (var attributeMapping in attributeMappingLst)
			{
				var targetRepresentations = await _scimRepresentationQueryRepository.FindSCIMRepresentationByAttributes(attributeMapping.TargetAttributeId, ids, attributeMapping.TargetResourceType);
				foreach (var representation in representationLst)
				{
					var filteredRepresentations = targetRepresentations.Where(a => a.GetAttributesByAttrSchemaId(attributeMapping.TargetAttributeId).Any(attr => attr.ValuesString.Contains(representation.Id)));
					if (!filteredRepresentations.Any())
					{
						representation.AddAttribute(new SCIMRepresentationAttribute
						{
							SchemaAttribute = new SCIMSchemaAttribute(attributeMapping.SourceAttributeSelector)
							{
								Name = attributeMapping.SourceAttributeSelector,
								MultiValued = true,
								Type = SCIMSchemaAttributeTypes.COMPLEX
							},
							Values = new List<SCIMRepresentationAttribute>()
						});
						continue;
					}

					foreach (var filteredRepresentation in filteredRepresentations)
					{
						var refLst = new List<SCIMRepresentationAttribute>();
						var refsAttribute = new SCIMRepresentationAttribute
						{
							SchemaAttribute = new SCIMSchemaAttribute(attributeMapping.SourceAttributeSelector)
							{
								Name = attributeMapping.SourceAttributeSelector,
								MultiValued = true,
								Type = SCIMSchemaAttributeTypes.COMPLEX
							},
							Values = refLst
						};

						refLst.Add(new SCIMRepresentationAttribute
						{
							SchemaAttribute = new SCIMSchemaAttribute("value")
							{
								Name = "value",
								MultiValued = false,
								Type = SCIMSchemaAttributeTypes.STRING
							},
							ValuesString = new List<string>
							{
								filteredRepresentation.Id
							}
						});
						refLst.Add(new SCIMRepresentationAttribute
						{
							SchemaAttribute = new SCIMSchemaAttribute("display")
							{
								Name = "display",
								MultiValued = false,
								Type = SCIMSchemaAttributeTypes.STRING
							},
							ValuesString = filteredRepresentation.Attributes.First(a => a.SchemaAttribute.Name == "displayName").ValuesString
						});
						refLst.Add(new SCIMRepresentationAttribute
						{
							SchemaAttribute = new SCIMSchemaAttribute("$ref")
							{
								Name = "$ref",
								MultiValued = false,
								Type = SCIMSchemaAttributeTypes.STRING
							},
							ValuesString = new List<string>
							{
								$"{baseUrl}/{attributeMapping.TargetResourceType}/{filteredRepresentation.Id}"
							}
						});

						representation.AddAttribute(refsAttribute);
					}
				}
			}
		}
	}
}
