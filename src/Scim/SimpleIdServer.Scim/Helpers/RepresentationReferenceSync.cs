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
				var newIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
				if (isScimRepresentationRemoved)
				{
					result.AddRange(await RemoveReferenceAttributes(newIds, attributeMapping, newSourceScimRepresentation));
				}
				else
				{
					var oldIds = oldScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => oldScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
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
					var attr = targetRepresentation.GetAttributesByAttrSchemaId(targetSchemaAttribute.Id).FirstOrDefault(v => targetRepresentation.GetChildren(v).Any(c => c.ValueString == sourceScimRepresentation.Id));
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
			var attr = scimRepresentation.GetAttributesByAttrSchemaId(attributeId).FirstOrDefault(v => scimRepresentation.GetChildren(v).Any(c => c.ValueString == sourceRepresentation.Id));
			if (attr != null)
			{
				scimRepresentation.RemoveAttribute(attr);
			}

			BuildScimRepresentationAttribute(attributeId, scimRepresentation, sourceRepresentation, resourceType);

		}

		protected virtual void BuildScimRepresentationAttribute(string attributeId, SCIMRepresentation targetRepresentation, SCIMRepresentation sourceRepresentation, string sourceResourceType)
		{
			var rootSchema = targetRepresentation.GetRootSchema();
			var attributes = new List<SCIMRepresentationAttribute>();
			var targetSchemaAttribute = rootSchema.GetAttributeById(attributeId);
			var values = rootSchema.GetChildren(targetSchemaAttribute);
			var value = values.FirstOrDefault(s => s.Name == "value");
			var display = values.FirstOrDefault(s => s.Name == "display");
			var type = values.FirstOrDefault(s => s.Name == "type");
			if (value != null)
            {
				attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), value)
				{
					ValueString = sourceRepresentation.Id
				});
			}

			if (display != null)
			{
				attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), display)
				{
					ValueString = sourceRepresentation.DisplayName
				});
			}

			if (type != null)
			{
				attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type)
				{
					ValueString = sourceResourceType
				});
			}

			var attrId = Guid.NewGuid().ToString();
			var attrs = targetRepresentation.GetAttributesByAttrSchemaId(targetSchemaAttribute.Id);
			if (attrs.Any())
            {
				attrId = attrs.First().AttributeId;
            }

			var parentAttr = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attrId, targetSchemaAttribute)
			{
				SchemaAttribute = targetSchemaAttribute
			};
			targetRepresentation.AddAttribute(parentAttr);
			foreach(var attr in attributes)
			{
				targetRepresentation.AddAttribute(parentAttr, attr);
			}
        }
	}
}
