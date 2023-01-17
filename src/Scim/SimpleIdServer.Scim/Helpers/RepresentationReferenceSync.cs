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
			var selfReferenceAttribute = attributeMappingLst.FirstOrDefault(a => a.IsSelf);
            var propagatedAttribute = attributeMappingLst.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
			var resolvedParents = new List<SCIMRepresentation> { newSourceScimRepresentation };
            var resolvedIndirectChildren = new List<SCIMRepresentation>();
            if (selfReferenceAttribute != null)
            {
                await ResolveParents(newSourceScimRepresentation, selfReferenceAttribute, resolvedParents);
				resolvedParents = resolvedParents.GroupBy(p => p.Id).Select(kvp => kvp.First()).ToList();
                if (propagatedAttribute != null)
                {
                    await ResolveIndirectChildren(oldScimRepresentation, selfReferenceAttribute, propagatedAttribute, resolvedIndirectChildren);
                    await ResolveIndirectChildren(newSourceScimRepresentation, selfReferenceAttribute, propagatedAttribute, resolvedIndirectChildren);
                    resolvedIndirectChildren = resolvedIndirectChildren.GroupBy(p => p.Id).Select(kvp => kvp.First()).ToList();
                }
            }

            foreach (var kvp in attributeMappingLst.GroupBy(m => m.SourceAttributeId))
            {
                bool purgeIndirectReferences = false;
                var newIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(kvp.Key).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
				IEnumerable<string> idsToBeRemoved = newIds, oldIds = new List<string>();
                List<SCIMRepresentation> newScimRepresentations = new List<SCIMRepresentation>();
                if (!isScimRepresentationRemoved)
                {
                    oldIds = oldScimRepresentation.GetAttributesByAttrSchemaId(kvp.Key).SelectMany(a => oldScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
                    idsToBeRemoved = oldIds.Except(newIds);
                    var duplicateIds = newIds.GroupBy(i => i).Where(i => i.Count() > 1);
                    if (duplicateIds.Any()) throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
                    if (!updateAllReferences) newIds = newIds.Except(oldIds).ToList();
                }

                foreach (var attributeMapping in kvp)
				{
                    if (isScimRepresentationRemoved)
                    {
                        idsToBeRemoved = newIds;
                        if (await RemoveReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation, location) && attributeMapping.IsSelf)
                            purgeIndirectReferences = true;
                    }
                    else
                    {
                        if (await RemoveReferenceAttributes(result, idsToBeRemoved, attributeMapping, newSourceScimRepresentation, location) && attributeMapping.IsSelf)
                            purgeIndirectReferences = true;
                        newScimRepresentations.AddRange((await AddReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation, location, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType))).Item2);
                    }
                }

                var parentIds = resolvedParents.Select(p => p.Id).ToList();
                if (purgeIndirectReferences)
                    await PurgeIndirectReferences(result, kvp, targetSchemas, resolvedIndirectChildren, newSourceScimRepresentation.Id, idsToBeRemoved, parentIds, location);
                else
                {
                    var attrMapping = kvp.FirstOrDefault(r => r.Mode == Mode.PROPAGATE_INHERITANCE);
                    if (attrMapping != null)
                    {
                        var sch = targetSchemas.First(s => s.ResourceType == attrMapping.TargetResourceType);
                        var parentAttr = sch.GetAttributeById(attrMapping.TargetAttributeId);
                        var valueAttr = sch.GetChildren(parentAttr).First(a => a.Name == "value");
                        await PropagateIndirectReferences(result, attrMapping, targetSchemas, newScimRepresentations, newSourceScimRepresentation.Id, resolvedParents, location);
                        await PropagateIndirectReferences(result, attrMapping, targetSchemas, resolvedIndirectChildren, newSourceScimRepresentation.Id, resolvedParents, location);
                    }
                }
            }

            return result;
		}

		public  async Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, string location)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult(_resourceTypeResolver);
			var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType);
			if (!attributeMappingLst.Any() || !patchOperations.Any() || !attributeMappingLst.Any(a => patchOperations.Any(o => o.Attr.SchemaAttributeId == a.SourceAttributeId))) return result;

			var targetSchemas = await _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct());
			var selfReferenceAttribute = attributeMappingLst.FirstOrDefault(a => a.IsSelf);
			var propagatedAttribute = attributeMappingLst.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
            var resolvedParents = new List<SCIMRepresentation> { newSourceScimRepresentation };
            var resolvedIndirectChildren = new List<SCIMRepresentation>();
			if (selfReferenceAttribute != null)
			{
				await ResolveParents(newSourceScimRepresentation, selfReferenceAttribute, resolvedParents);
				if (propagatedAttribute != null)
				{
					await ResolveIndirectChildren(patchOperations, selfReferenceAttribute, propagatedAttribute, resolvedIndirectChildren);
                    await ResolveIndirectChildren(newSourceScimRepresentation, selfReferenceAttribute, propagatedAttribute, resolvedIndirectChildren);
                    resolvedIndirectChildren = resolvedIndirectChildren.GroupBy(p => p.Id).Select(kvp => kvp.First()).ToList();
                }
            }
			
			foreach (var kvp in attributeMappingLst.GroupBy(m => m.SourceAttributeId))
			{
				var missingIds = new List<string>();
				bool isReferenceMissing = true, purgeIndirectReferences = false;
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
                ICollection<SCIMRepresentation> newScimRepresentations = new List<SCIMRepresentation>();
                foreach (var attributeMapping in kvp)
                {
                    if (await RemoveReferenceAttributes(result, idsToBeRemoved, attributeMapping, newSourceScimRepresentation, location) && attributeMapping.IsSelf)
						purgeIndirectReferences = true;
					var res = await AddReferenceAttributes(result, newIds, attributeMapping, newSourceScimRepresentation, location, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType), true);
					if (res.Item1)
						isReferenceMissing = false;
					else
						missingIds.AddRange(res.Item2.Select(r => r.Id));
					foreach(var rec in res.Item2) newScimRepresentations.Add(rec);
                    await UpdateReferenceAttributes(result, existingIds, attributeMapping, newSourceScimRepresentation, patchOperations, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType), location: location);
                }

				if (missingIds.Any() && isReferenceMissing)
					throw new SCIMNotFoundException(string.Format(Global.ReferencesDontExist, string.Join(",", missingIds.Distinct())));

                var parentIds = resolvedParents.Select(p => p.Id).ToList();
				if (purgeIndirectReferences)
					await PurgeIndirectReferences(result, kvp, targetSchemas, resolvedIndirectChildren, newSourceScimRepresentation.Id, idsToBeRemoved, parentIds, location);
				else
				{
                    var attrMapping = kvp.FirstOrDefault(r => r.Mode == Mode.PROPAGATE_INHERITANCE);
					if(attrMapping != null)
                    {
                        await PropagateIndirectReferences(result, attrMapping, targetSchemas, newScimRepresentations, newSourceScimRepresentation.Id, resolvedParents, location);
                        await PropagateIndirectReferences(result, attrMapping, targetSchemas, resolvedIndirectChildren, newSourceScimRepresentation.Id, resolvedParents, location);
                    }
                }
            }

			return result;
		}

		public async virtual Task<bool> IsReferenceProperty(ICollection<string> attributes)
        {
			var attrs = await _scimAttributeMappingQueryRepository.GetBySourceAttributes(attributes);
			return attributes.All(a => attrs.Any(at => at.SourceAttributeSelector == a));
		}

		protected virtual async Task ResolveIndirectChildren(ICollection<SCIMPatchResult> patchOperations, SCIMAttributeMapping selfAttributeMapping, SCIMAttributeMapping attributeMapping, List<SCIMRepresentation> lst)
		{
			var ids = patchOperations.
				Where(p => (p.Operation == SCIMPatchOperations.REMOVE || p.Operation == SCIMPatchOperations.ADD) && p.Attr.SchemaAttributeId == selfAttributeMapping.SourceAttributeId)
				.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
			if (!ids.Any()) return;
            var children = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, selfAttributeMapping.SourceResourceType);
            foreach (var child in children)
            {
                var indirectChildren = child.HierarchicalAttributes.Where(a => a.SchemaAttributeId == attributeMapping.SourceAttributeId).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
                lst.AddRange(await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(indirectChildren, attributeMapping.TargetResourceType));
                await ResolveIndirectChildren(child, selfAttributeMapping, attributeMapping, lst);
            }
        }

        protected virtual async Task ResolveIndirectChildren(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfAttributeMapping, SCIMAttributeMapping attributeMapping, List<SCIMRepresentation> lst)
        {
			var childrenIds = scimRepresentation.HierarchicalAttributes.Where(a => a.SchemaAttributeId == selfAttributeMapping.SourceAttributeId && a.CachedChildren.Any()).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
			var children = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(childrenIds, selfAttributeMapping.SourceResourceType);
			foreach(var child in children)
			{
				var indirectChildren = child.HierarchicalAttributes.Where(a => a.SchemaAttributeId == attributeMapping.SourceAttributeId).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
				lst.AddRange(await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(indirectChildren, attributeMapping.TargetResourceType));
				await ResolveIndirectChildren(child, selfAttributeMapping, attributeMapping, lst);
            }
        }

        protected virtual async Task ResolveParents(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfReferenceAttribute, List<SCIMRepresentation> lst)
        {
			var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.value";
            var parents = await _scimRepresentationCommandRepository.FindSCIMRepresentationsByAttributeFullPath(fullPath, new List<string> { scimRepresentation.Id }, selfReferenceAttribute.SourceResourceType);
			lst.AddRange(parents);
            foreach (var parent in parents)
                await ResolveParents(parent, selfReferenceAttribute, lst);
        }

		protected virtual async Task PurgeIndirectReferences(RepresentationSyncResult res, IEnumerable<SCIMAttributeMapping> mappings, IEnumerable<SCIMSchema> schemas, IEnumerable<SCIMRepresentation> children, string representationId, IEnumerable<string> removedIds, IEnumerable<string> resolvedParentIds, string location)
        {
            var attributeMappingToBePurged = mappings.FirstOrDefault(r => r.Mode == Mode.PROPAGATE_INHERITANCE);
            if (attributeMappingToBePurged == null) return;
            var sch = schemas.First(s => s.ResourceType == attributeMappingToBePurged.TargetResourceType);
            var parentAttr = sch.GetAttributeById(attributeMappingToBePurged.TargetAttributeId);
			var valueAttr = sch.GetChildren(parentAttr).First(a => a.Name == "value");
            var fullPath = valueAttr.FullPath;
            foreach (var child in children)
            {
                foreach (var resolvedParentId in resolvedParentIds)
                {
                    var parent = child.GetAttributesByPath(fullPath).FirstOrDefault(a => a.ValueString == resolvedParentId);
                    child.RemoveIndirectReference(resolvedParentId, attributeMappingToBePurged.TargetAttributeId);
                    if (child.IndirectReferences.Any(r => r.TargetAttributeId == attributeMappingToBePurged.TargetAttributeId && r.TargetReferenceId == resolvedParentId && r.NbReferences <= 0))
					{
                        res.RemoveReferenceAttr(child, attributeMappingToBePurged.TargetAttributeId, parentAttr.FullPath, resolvedParentId, location);
                        child.RemoveAttributesById(new string[] { parent.ParentAttributeId });
                    }
                }
            }
        }

		protected virtual async Task PropagateIndirectReferences(RepresentationSyncResult result, SCIMAttributeMapping mapping, IEnumerable<SCIMSchema> schemas, string representationId, IEnumerable<SCIMRepresentation> resolvedParents, IEnumerable<string> excludedAttributeIds, string location)
		{
            var sch = schemas.First(s => s.ResourceType == mapping.TargetResourceType);
            var parentAttr = sch.GetAttributeById(mapping.TargetAttributeId);
            var valueAttr = sch.GetChildren(parentAttr).First(a => a.Name == "value");
            var children = await _scimRepresentationCommandRepository.FindSCIMRepresentationsByAttributeFullPath(valueAttr.FullPath, new List<string> { representationId }, mapping.TargetResourceType);
			children = children.Where(c => !excludedAttributeIds.Contains(c.Id));
			await PropagateIndirectReferences(result, mapping, schemas, children, representationId, resolvedParents, location);
        }

		protected virtual async Task PropagateIndirectReferences(RepresentationSyncResult result, SCIMAttributeMapping attributeMapping, IEnumerable<SCIMSchema> schemas, IEnumerable<SCIMRepresentation> children, string representationId, IEnumerable<SCIMRepresentation> resolvedParents, string location)
		{
            foreach (var child in children)
            {
                foreach (var resolvedParent in resolvedParents)
                {
                    var parentAttrs = child.GetAttributesByAttrSchemaId(attributeMapping.TargetAttributeId);
                    child.AddIndirectReference(resolvedParent.Id, attributeMapping.TargetAttributeId);
                    if (parentAttrs.Any(p => child.GetChildren(p).Any(c => c.SchemaAttribute.Name == "value" && c.ValueString == resolvedParent.Id))) continue;
                    var schema = schemas.First(s => s.ResourceType == child.ResourceType);
                    result.AddReferenceAttr(child, attributeMapping.TargetAttributeId, schema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, resolvedParent.Id, location);
                    BuildScimRepresentationAttribute(attributeMapping.TargetAttributeId,
                        child, resolvedParent, Mode.PROPAGATE_INHERITANCE,
                        attributeMapping.SourceResourceType, schema, false);
                }
            }
        }

        protected virtual async Task<bool> RemoveReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, string location)
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

				return true;
			}

			return false;
		}

		protected virtual async Task<(bool, ICollection<SCIMRepresentation>)> AddReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, string location, SCIMSchema targetSchema, bool checkMissing = false)
		{
			var lst = new List<SCIMRepresentation>();
			var targetRepresentations = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			if (checkMissing)
			{
                var missingIds = ids.Where(i => !targetRepresentations.Any(r => r.Id == i));
				if (missingIds.Any()) return (false, lst);
            }

			if (!targetRepresentations.Any()) return (true, lst);
			foreach (var targetRepresentation in targetRepresentations)
			{
				bool isAttrUpdated;
				var sourceSchema = sourceScimRepresentation.Schemas.First(s => s.ResourceType == attributeMapping.SourceResourceType && s.GetAttributeById(attributeMapping.SourceAttributeId) != null);
				UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, attributeMapping.Mode, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType, targetSchema, true, out isAttrUpdated);
				if (isAttrUpdated)
                    result.UpdateReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetSchema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id, location);
                UpdateScimRepresentation(sourceScimRepresentation, targetRepresentation, Mode.STANDARD, attributeMapping.SourceAttributeId, attributeMapping.TargetResourceType, sourceSchema, true, out bool b);
				if (!string.IsNullOrWhiteSpace(attributeMapping.TargetAttributeId) && !isAttrUpdated)
                {
                    result.AddReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetSchema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id, location);
                    lst.Add(targetRepresentation);
                }

				result.AddRepresentation(targetRepresentation);
			}

			return (true, lst);
		}

		protected virtual async Task UpdateReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, SCIMSchema targetSchema, string location)
        {
			if (!patchOperations.Any(p => IsDisplayName(p.Attr.FullPath))) return;
			var targetRepresentations = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			foreach (var targetRepresentation in targetRepresentations)
			{
				UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, Mode.STANDARD, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType, targetSchema, true, out bool isAttrUpdated);
				result.UpdateReferenceAttr(targetRepresentation, attributeMapping.TargetAttributeId, targetSchema.GetAttributeById(attributeMapping.TargetAttributeId).FullPath, sourceScimRepresentation.Id, location);
				targetRepresentation.SetUpdated(DateTime.UtcNow);
			}
		}

		protected virtual void UpdateScimRepresentation(SCIMRepresentation scimRepresentation, SCIMRepresentation sourceRepresentation, Mode mode, string attributeId, string resourceType, SCIMSchema targetSchema, bool isDirect, out bool isAttrUpdated)
		{
			isAttrUpdated = false;
			if (string.IsNullOrWhiteSpace(attributeId)) return;
			var attr = scimRepresentation.GetAttributesByAttrSchemaId(attributeId).FirstOrDefault(v => scimRepresentation.GetChildren(v).Any(c => c.ValueString == sourceRepresentation.Id));
			if (attr != null)
			{
				scimRepresentation.RemoveAttributeById(attr);
				isAttrUpdated = true;
			}

			BuildScimRepresentationAttribute(attributeId, scimRepresentation, sourceRepresentation, mode, resourceType, targetSchema, isDirect);
		}

		protected virtual void BuildScimRepresentationAttribute(string attributeId, SCIMRepresentation targetRepresentation, SCIMRepresentation sourceRepresentation, Mode mode, string sourceResourceType, SCIMSchema targetSchema, bool isDirect = true)
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
				switch(mode)
				{
					case Mode.STANDARD:
                        attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type, type.SchemaId)
                        {
                            ValueString = sourceResourceType
                        });
                        break;
					case Mode.PROPAGATE_INHERITANCE:
                        attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type, type.SchemaId)
                        {
                            ValueString = isDirect ? "direct" : "indirect"
                        });
                        break;
				}
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
