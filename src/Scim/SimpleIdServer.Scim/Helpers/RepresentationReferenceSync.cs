// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.CodeAnalysis;
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
using static MassTransit.ValidationResultExtensions;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationReferenceSync : IRepresentationReferenceSync
	{
		private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
		private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
		private readonly ISCIMSchemaCommandRepository _scimSchemaCommandRepository;
		private readonly IResourceTypeResolver _resourceTypeResolver;

		public RepresentationReferenceSync(
			ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
			ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
			ISCIMSchemaCommandRepository scimSchemaCommandRepository,
			IResourceTypeResolver resourceTypeResolver)
        {
			_scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
			_scimRepresentationCommandRepository = scimRepresentationCommandRepository;
			_scimSchemaCommandRepository = scimSchemaCommandRepository;
			_resourceTypeResolver = resourceTypeResolver;
		}

		public async virtual Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
		{
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			return await Sync(attributeMappingLst, resourceType, oldScimRepresentation, newSourceScimRepresentation, location, updateAllReferences, isScimRepresentationRemoved);
		}

		public async virtual Task<RepresentationSyncResult> Sync(IEnumerable<SCIMAttributeMapping> attributeMappingLst, string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult(_resourceTypeResolver);
			if (!attributeMappingLst.Any()) return result;
			var targetSchemas = await _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct());
			foreach (var attributeMapping in attributeMappingLst)
			{
				var newIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(attributeMapping.SourceAttributeId).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
				if (isScimRepresentationRemoved)
				{
					await RemoveReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation, location);
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

					await RemoveReferenceAttributes(result, idsToBeRemoved, attributeMapping, newSourceScimRepresentation, location);
					await AddReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation, location, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType));
				}
			}

			return result;
		}

		public  async Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, string location)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult(_resourceTypeResolver);
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			if (!attributeMappingLst.Any()) return result;

			var targetSchemas = await _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct());
			foreach (var kvp in attributeMappingLst.GroupBy(m => m.SourceAttributeId))
			{
				var missingIds = new List<string>();
				bool isReferenceMissing = true;
				var allCurrentIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(kvp.Key).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
				var newIds = patchOperations
					.Where(p => p.Operation == SCIMPatchOperations.ADD && p.Attr.SchemaAttributeId == kvp.Key)
					.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
				var idsToBeRemoved = patchOperations
					.Where(p => p.Operation == SCIMPatchOperations.REMOVE && p.Attr.SchemaAttributeId == kvp.Key)
					.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
				var existingIds = allCurrentIds.Where(i => !idsToBeRemoved.Contains(i));
				var duplicateIds = allCurrentIds.GroupBy(i => i).Where(i => i.Count() > 1);
				if (duplicateIds.Any()) throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
				foreach(var attributeMapping in kvp)
				{
                    await RemoveReferenceAttributes(result, idsToBeRemoved, attributeMapping, newSourceScimRepresentation, location);
					var res = await AddReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation, location, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType));
					if (res.Item1)
						isReferenceMissing = false;
					else
						missingIds.AddRange(res.Item2.ToList());
                    await UpdateReferenceAttributes(result, existingIds, attributeMapping, newSourceScimRepresentation, patchOperations, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType), location: location);
                }

				if (missingIds.Any() && isReferenceMissing)
					throw new SCIMNotFoundException(string.Format(Global.ReferencesDontExist, string.Join(",", missingIds.Distinct())));
			}

			return result;
		}

		public async virtual Task<bool> IsReferenceProperty(ICollection<string> attributes)
        {
			var attrs = await _scimAttributeMappingQueryRepository.GetBySourceAttributes(attributes);
			return attributes.All(a => attrs.Any(at => at.SourceAttributeSelector == a));
		}

		protected virtual async Task RemoveReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, string location)
		{
			var targetRepresentations = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			if (targetRepresentations.Any())
            {
				var firstTargetRepresentation = targetRepresentations.First();
				foreach(var targetRepresentation in targetRepresentations)
				{
					var attr = targetRepresentation.GetAttributesByAttrSchemaId(attributeMapping.TargetAttributeId).FirstOrDefault(v => targetRepresentation.GetChildren(v).Any(c => c.ValueString == sourceScimRepresentation.Id));
					if (attr != null)
					{
						targetRepresentation.RemoveAttributeById(attr);
						result.RemoveReferenceAttr(targetRepresentation, attr.SchemaAttribute.Id, attr.SchemaAttribute.FullPath, sourceScimRepresentation.Id, location);
					}

					result.AddRepresentation(targetRepresentation);
				}
			}
		}

		protected virtual async Task<(bool, IEnumerable<string>)> AddReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, string location, SCIMSchema targetSchema)
		{
			var targetRepresentations = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			var missingIds = ids.Where(i => !targetRepresentations.Any(r => r.Id == i));
			if (missingIds.Any()) return (false, missingIds);
			if (targetRepresentations.Any())
			{
				foreach (var targetRepresentation in targetRepresentations)
				{
					bool isAttrUpdated;
					var sourceSchema = sourceScimRepresentation.Schemas.First(s => s.ResourceType == attributeMapping.SourceResourceType && s.GetAttributeById(attributeMapping.SourceAttributeId) != null);
					UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType, targetSchema, out isAttrUpdated);
					if (isAttrUpdated)
                        result.UpdateReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetSchema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id, location);
                    UpdateScimRepresentation(sourceScimRepresentation, targetRepresentation, attributeMapping.SourceAttributeId, attributeMapping.TargetResourceType, sourceSchema, out bool b);
					if (!string.IsNullOrWhiteSpace(attributeMapping.TargetAttributeId) && !isAttrUpdated)
						result.AddReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetSchema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id, location);
					result.AddRepresentation(targetRepresentation);
				}
			}

			return (true, null);
		}

		protected virtual async Task UpdateReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, SCIMSchema targetSchema, string location)
        {
			if (!patchOperations.Any(p => IsDisplayName(p.Attr.FullPath))) return;
			var targetRepresentations = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			foreach (var targetRepresentation in targetRepresentations)
			{
				UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType, targetSchema, out bool isAttrUpdated);
				result.UpdateReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetSchema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id, location);
				targetRepresentation.SetUpdated(DateTime.UtcNow);
			}
		}

		protected virtual void UpdateScimRepresentation(SCIMRepresentation scimRepresentation, SCIMRepresentation sourceRepresentation, string attributeId, string resourceType, SCIMSchema targetSchema, out bool isAttrUpdated)
		{
			isAttrUpdated = false;
			if (string.IsNullOrWhiteSpace(attributeId)) return;
			var attr = scimRepresentation.GetAttributesByAttrSchemaId(attributeId).FirstOrDefault(v => scimRepresentation.GetChildren(v).Any(c => c.ValueString == sourceRepresentation.Id));
			if (attr != null)
			{
				scimRepresentation.RemoveAttributeById(attr);
				isAttrUpdated = true;
			}

			BuildScimRepresentationAttribute(attributeId, scimRepresentation, sourceRepresentation, resourceType, targetSchema);
		}

		protected virtual void BuildScimRepresentationAttribute(string attributeId, SCIMRepresentation targetRepresentation, SCIMRepresentation sourceRepresentation, string sourceResourceType, SCIMSchema targetSchema)
		{
			var attributes = new List<SCIMRepresentationAttribute>();
			var targetSchemaAttribute = targetSchema.GetAttributeById(attributeId);
			var values = targetSchema.GetChildren(targetSchemaAttribute);
			var value = values.FirstOrDefault(s => s.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
			var display = values.FirstOrDefault(s => IsDisplayName(s.Name));
			var type = values.FirstOrDefault(s => s.Name == SCIMConstants.StandardSCIMReferenceProperties.Type);
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

		private static bool IsDisplayName(string name)
        {
			return name == SCIMConstants.StandardSCIMReferenceProperties.Display || name == SCIMConstants.StandardSCIMReferenceProperties.DisplayName;
		}
    }
}
