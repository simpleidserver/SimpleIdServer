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
				await EnrichWithOuterKeys(attributeMapping, representationLst, baseUrl, ids);
				await EnrichWithInnerKeys(attributeMapping, representationLst, baseUrl);
			}
		}

		private async Task EnrichWithOuterKeys(
			SCIMAttributeMapping attributeMapping, 
			IEnumerable<SCIMRepresentation> representationLst, 
			string baseUrl,
			IEnumerable<string> ids)
        {
			var values = representationLst.SelectMany(r => r.GetAttributesByAttrSchemaId(attributeMapping.SourceValueAttributeId)).SelectMany(a => a.ValuesString);
			var targetRepresentations = await _scimRepresentationQueryRepository.FindSCIMRepresentationByAttributes(attributeMapping.TargetAttributeId, ids, attributeMapping.TargetResourceType);
			if (!targetRepresentations.Any())
			{
				targetRepresentations = await _scimRepresentationQueryRepository.FindSCIMRepresentationByIds(values, attributeMapping.TargetResourceType);
			}

			foreach (var representation in representationLst)
			{
				var filteredRepresentations = targetRepresentations.Where(a => a.GetAttributesByAttrSchemaId(attributeMapping.TargetAttributeId).Any(attr => attr.ValuesString.Contains(representation.Id)));
				if (!filteredRepresentations.Any())
				{
					continue;
				}

				foreach (var filteredRepresentation in filteredRepresentations)
				{
					var refLst = BuildAttributes(filteredRepresentation, attributeMapping, baseUrl);
					var refsAttribute = new SCIMRepresentationAttribute
					{
						SchemaAttribute = new SCIMSchemaAttribute(attributeMapping.SourceAttributeSelector)
						{
							Id = attributeMapping.SourceAttributeId,
							Name = attributeMapping.SourceAttributeSelector,
							MultiValued = true,
							Type = SCIMSchemaAttributeTypes.COMPLEX
						},
						Values = refLst
					};
					representation.AddAttribute(refsAttribute);
				}
			}
		}

		private async Task EnrichWithInnerKeys(
			SCIMAttributeMapping attributeMapping,
			IEnumerable<SCIMRepresentation> representationLst,
			string baseUrl
			)
		{
			var values = representationLst.SelectMany(r => r.GetAttributesByAttrSchemaId(attributeMapping.SourceValueAttributeId))
				.SelectMany(a => a.ValuesString)
				.ToList();
			var targetRepresentations = await _scimRepresentationQueryRepository.FindSCIMRepresentationByIds(values, attributeMapping.TargetResourceType);
			if (!targetRepresentations.Any())
            {
				return;
            }

			foreach(var representation in representationLst)
            {
				foreach(var targetRepresentation in targetRepresentations)
				{
					var attr = representation.GetAttributesByAttrSchemaId(attributeMapping.SourceValueAttributeId).FirstOrDefault(a => a.ValuesString.Contains(targetRepresentation.Id));
					if (attr == null)
                    {
						continue;
                    }

					var parentAttr = attr.Parent;
					parentAttr.Values.Clear();
					var newAttributes = BuildAttributes(targetRepresentation, attributeMapping, baseUrl);
					foreach(var newAttribute in newAttributes)
                    {
						parentAttr.Values.Add(newAttribute);
                    }
				}
            }
		}

		private static List<SCIMRepresentationAttribute> BuildAttributes(SCIMRepresentation representation, SCIMAttributeMapping attributeMapping, string baseUrl)
        {
			var refLst = new List<SCIMRepresentationAttribute>();
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
					representation.Id
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
				ValuesString = new List<string> { representation.DisplayName }
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
								$"{baseUrl}/{attributeMapping.TargetResourceType}/{representation.Id}"
							}
			});

			return refLst;
		}
	}
}
