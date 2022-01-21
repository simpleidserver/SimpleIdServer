// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public async virtual Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult();
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
					await RemoveReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation);
				}
				else
				{
					var oldIds = oldScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => oldScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
					var idsToBeRemoved = oldIds.Except(newIds);
					var duplicateIds = newIds.GroupBy(i => i).Where(i => i.Count() > 1);
					if (duplicateIds.Any())
					{
						throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
					}

					if (!updateAllReferences)
					{
						newIds = newIds.Except(oldIds).ToList();
					}

					await RemoveReferenceAttributes(result, idsToBeRemoved, attributeMapping, newSourceScimRepresentation);
					await UpdateReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation);
				}
			}

			return result;
		}

		public  async Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult();
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			if (!attributeMappingLst.Any())
			{
				return result;
			}

			foreach (var attributeMapping in attributeMappingLst)
			{
				var existingIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
				var newIds = patchOperations
					.Where(p => p.Operation == DTOs.SCIMPatchOperations.ADD && p.Attr.SchemaAttributeId == attributeMapping.SourceAttributeId)
					.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
				var idsToBeRemoved = patchOperations
					.Where(p => p.Operation == DTOs.SCIMPatchOperations.REMOVE && p.Attr.SchemaAttributeId == attributeMapping.SourceAttributeId)
					.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
				var duplicateIds = existingIds.GroupBy(i => i).Where(i => i.Count() > 1);
				if (duplicateIds.Any())
				{
					throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
				}

				await RemoveReferenceAttributes(result, idsToBeRemoved, attributeMapping, newSourceScimRepresentation);
				await UpdateReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation);
			}

			return result;
		}

		public async virtual Task<bool> IsReferenceProperty(ICollection<string> attributes)
        {
			var attrs = await _scimAttributeMappingQueryRepository.GetBySourceAttributes(attributes);
			return attributes.All(a => attrs.Any(at => at.SourceAttributeSelector == a));
		}

		protected virtual async Task RemoveReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation)
		{
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
						result.RemoveReferenceAttr(targetRepresentation, attr.SchemaAttribute.Id, attr.SchemaAttribute.FullPath, sourceScimRepresentation.Id);
						targetRepresentation.RemoveAttributeById(attr);
					}

					result.AddRepresentation(targetRepresentation);
				}
			}
		}

		protected virtual async Task UpdateReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation)
		{
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
					bool isAttrUpdated;
					UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType, out isAttrUpdated);
					UpdateScimRepresentation(sourceScimRepresentation, targetRepresentation, attributeMapping.SourceAttributeId, attributeMapping.TargetResourceType, out bool b);
					if (!isAttrUpdated)
					{
						result.AddReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetRepresentation.GetSchemaAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id);
					}

					result.AddRepresentation(targetRepresentation);
				}
			}
		}

		protected virtual void UpdateScimRepresentation(SCIMRepresentation scimRepresentation, SCIMRepresentation sourceRepresentation, string attributeId, string resourceType, out bool isAttrUpdated)
		{
			isAttrUpdated = false;
			var attr = scimRepresentation.GetAttributesByAttrSchemaId(attributeId).FirstOrDefault(v => scimRepresentation.GetChildren(v).Any(c => c.ValueString == sourceRepresentation.Id));
			if (attr != null)
			{
				scimRepresentation.RemoveAttributeById(attr);
				isAttrUpdated = true;
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
				attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), value, value.SchemaId)
				{
					ValueString = sourceRepresentation.Id
				});
			}

			if (display != null)
			{
				attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), display, display.SchemaId)
				{
					ValueString = sourceRepresentation.DisplayName
				});
			}

			if (type != null)
			{
				attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type, type.SchemaId)
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

			var parentAttr = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attrId, targetSchemaAttribute, targetSchemaAttribute.SchemaId)
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
