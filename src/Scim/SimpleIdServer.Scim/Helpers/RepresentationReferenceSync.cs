// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationReferenceSync : IRepresentationReferenceSync
	{
		private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
		private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;

		public RepresentationReferenceSync(
			ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
			ISCIMRepresentationQueryRepository scimRepresentationQueryRepository)
        {
			_scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
			_scimRepresentationQueryRepository = scimRepresentationQueryRepository;
		}

		public async virtual Task<ICollection<SCIMRepresentation>> Sync(string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, bool isScimRepresentationRemoved = false)
		{
			var result = new List<SCIMRepresentation>();
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			if (!attributeMappingLst.Any())
			{
				return result;
			}

			foreach (var attributeMapping in attributeMappingLst)
			{
				var newIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => a.Values.Where(v => v.SchemaAttribute.Name == "value").SelectMany(v => v.ValuesString));
				if (isScimRepresentationRemoved)
				{
					await RemoveReferenceAttributes(newIds, attributeMapping, newSourceScimRepresentation);
				}
				else
				{
					var oldIds = oldScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => a.Values.Where(v => v.SchemaAttribute.Name == "value").SelectMany(v => v.ValuesString));
					var idsToBeRemoved = oldIds.Where(i => !newIds.Contains(i));
					result.AddRange(await RemoveReferenceAttributes(idsToBeRemoved, attributeMapping, newSourceScimRepresentation));
					result.AddRange(await UpdateReferenceAttributes(newIds, attributeMapping, newSourceScimRepresentation));
				}
			}

			return result;
		}

		protected virtual async Task<List<SCIMRepresentation>> RemoveReferenceAttributes(IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation)
		{
			var result = new List<SCIMRepresentation>();
			var targetRepresentations = await _scimRepresentationQueryRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			if (targetRepresentations.Any())
            {
				var firstTargetRepresentation = targetRepresentations.First();
				var targetSchemaAttribute = firstTargetRepresentation.GetRootSchema().GetAttributeById(attributeMapping.TargetAttributeId);
				foreach(var targetRepresentation in targetRepresentations)
				{
					var attr = targetRepresentation.Attributes.FirstOrDefault(s => s.SchemaAttribute.Id == targetSchemaAttribute.Id && s.Values != null && s.Values.Any(v => v.ValuesString != null && v.ValuesString.Contains(sourceScimRepresentation.Id)));
					if (attr != null)
					{
						targetRepresentation.RemoveAttribute(attr);
					}

					result.Add(targetRepresentation);
				}
			}

			return result;
		}

		protected virtual async Task<List<SCIMRepresentation>> UpdateReferenceAttributes(IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation)
		{
			var result = new List<SCIMRepresentation>();
			var targetRepresentations = await _scimRepresentationQueryRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			var missingIds = ids.Where(i => !targetRepresentations.Any(r => r.Id == i));
			if (missingIds.Any())
            {
				throw new SCIMNotFoundException(string.Format(Global.ReferencesDontExist, string.Join(",", missingIds)));
            }

			if (targetRepresentations.Any())
			{
				foreach (var targetRepresentation in targetRepresentations)
				{
					UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType);
					UpdateScimRepresentation(sourceScimRepresentation, targetRepresentation, attributeMapping.SourceAttributeId, attributeMapping.TargetResourceType);
					result.Add(targetRepresentation);
				}
			}

			return result;
		}

		protected virtual void UpdateScimRepresentation(SCIMRepresentation scimRepresentation, SCIMRepresentation sourceRepresentation, string attributeId, string resourceType)
		{
			var attr = scimRepresentation.Attributes.FirstOrDefault(s => s.SchemaAttribute.Id == attributeId && s.Values != null && s.Values.Any(v => v.ValuesString != null && v.ValuesString.Contains(sourceRepresentation.Id)));
			if (attr != null)
			{
				scimRepresentation.RemoveAttribute(attr);
			}

			scimRepresentation.AddAttribute(BuildScimRepresentationAttribute(attributeId, scimRepresentation, sourceRepresentation, resourceType));

		}

		protected virtual SCIMRepresentationAttribute BuildScimRepresentationAttribute(string attributeId, SCIMRepresentation targetRepresentation, SCIMRepresentation sourceRepresentation, string sourceResourceType)
		{
			var attributes = new List<SCIMRepresentationAttribute>();
			var targetSchemaAttribute = targetRepresentation.GetRootSchema().GetAttributeById(attributeId);
			var value = targetSchemaAttribute.SubAttributes.FirstOrDefault(s => s.Name == "value");
			var display = targetSchemaAttribute.SubAttributes.FirstOrDefault(s => s.Name == "display");
			var type = targetSchemaAttribute.SubAttributes.FirstOrDefault(s => s.Name == "type");
			if (value != null)
            {
				attributes.Add(new SCIMRepresentationAttribute
				{
					Id = Guid.NewGuid().ToString(),
					SchemaAttribute = new SCIMSchemaAttribute(value.Id)
					{
						Name = "value",
						MultiValued = false,
						Type = SCIMSchemaAttributeTypes.STRING
					},
					ValuesString = new List<string>
					{
						sourceRepresentation.Id
					}
				});
			}

			if (display != null)
			{
				attributes.Add(new SCIMRepresentationAttribute
				{
					Id = Guid.NewGuid().ToString(),
					SchemaAttribute = new SCIMSchemaAttribute(display.Id)
					{
						Name = "display",
						MultiValued = false,
						Type = SCIMSchemaAttributeTypes.STRING
					},
					ValuesString = new List<string>
					{
						sourceRepresentation.DisplayName
					}
				});
			}

			if (type != null)
			{
				attributes.Add(new SCIMRepresentationAttribute
				{
					Id = Guid.NewGuid().ToString(),
					SchemaAttribute = new SCIMSchemaAttribute(type.Id)
					{
						Name = "type",
						MultiValued = false,
						Type = SCIMSchemaAttributeTypes.STRING
					},
					ValuesString = new List<string>
					{
						sourceResourceType
					}
				});
			}

			return new SCIMRepresentationAttribute
			{
				Id = Guid.NewGuid().ToString(),
				SchemaAttribute = targetSchemaAttribute,
				Values = attributes
			};
        }
	}
}
