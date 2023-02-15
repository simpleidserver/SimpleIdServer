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

		public virtual IEnumerable<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
		{
			var attributeMappingLst = _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType).Result;
			return Sync(attributeMappingLst, resourceType, oldScimRepresentation, newSourceScimRepresentation, location, updateAllReferences, isScimRepresentationRemoved);
		}

		public IEnumerable<RepresentationSyncResult> Sync(IEnumerable<SCIMAttributeMapping> attributeMappingLst, string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult(_resourceTypeResolver);
            if (!attributeMappingLst.Any()) yield return result;

            var targetSchemas = _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct()).Result;
			var selfReferenceAttribute = attributeMappingLst.FirstOrDefault(a => a.IsSelf);
            var propagatedAttribute = attributeMappingLst.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
			var resolvedParents = new List<SCIMRepresentation> { newSourceScimRepresentation };
            var resolvedIndirectChildren = new List<SCIMRepresentation>();
            var mode = propagatedAttribute == null ? Mode.STANDARD : Mode.PROPAGATE_INHERITANCE;

            // Update 'direct' references.
            var allAddedIds = new List<string>();
            var allRemovedIds = new List<string>();
            foreach (var kvp in attributeMappingLst.GroupBy(m => m.SourceAttributeId))
            {
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
                        foreach(var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(newIds))
                            RemoveReferenceAttributes(result, paginatedResult.Item1, attributeMapping, newSourceScimRepresentation, location);
                        allRemovedIds.AddRange(newIds);
                        idsToBeRemoved = newIds;
                    }
                    else
                    {
                        foreach (var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(idsToBeRemoved))
                            RemoveReferenceAttributes(result, paginatedResult.Item1, attributeMapping, newSourceScimRepresentation, location);

                        if(!string.IsNullOrWhiteSpace(attributeMapping.TargetAttributeId))
                        foreach(var newId in newIds)
                            result.AddReferenceAttributes(BuildScimRepresentationAttribute(newId, attributeMapping.TargetAttributeId, newSourceScimRepresentation, mode, newSourceScimRepresentation.ResourceType, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType)));
                        
                        // foreach(var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(newIds))
                        //   AddReferenceAttributes(result, paginatedResult.Item1, attributeMapping, newSourceScimRepresentation, location, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType), paginatedResult.Item2);
                        allRemovedIds.AddRange(idsToBeRemoved);
                        allAddedIds.AddRange(newIds);
                    }
                }
            }

            yield return result;

            SyncIndirectReferences(result, newSourceScimRepresentation, allAddedIds, allRemovedIds, propagatedAttribute, selfReferenceAttribute, targetSchemas, location, mode);
        }

		public  IEnumerable<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, string location)
		{
			var stopWatch = new Stopwatch();
			var result = new RepresentationSyncResult(_resourceTypeResolver);
			var attributeMappingLst = _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType).Result;
			if (!attributeMappingLst.Any()) yield return result;

			var targetSchemas = _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct()).Result;
			var selfReferenceAttribute = attributeMappingLst.FirstOrDefault(a => a.IsSelf);
			var propagatedAttribute = attributeMappingLst.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
			var mode = propagatedAttribute == null ? Mode.STANDARD : Mode.PROPAGATE_INHERITANCE;
            var allIds = patchOperations.Where(p => p.Attr != null && !string.IsNullOrWhiteSpace(p.Attr.ValueString)).Select(p => p.Attr.ValueString).Distinct();
            var targetRepresentations = _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(allIds).Result;

            // Update 'direct' references.
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
                foreach (var attributeMapping in kvp)
                {
                    foreach(var paginationResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(idsToBeRemoved, attributeMapping.TargetResourceType))
                        RemoveReferenceAttributes(result, paginationResult.Item1, attributeMapping, newSourceScimRepresentation, location);

                    foreach (var paginationResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(newIds, attributeMapping.TargetResourceType))
                    {
                        var res = AddReferenceAttributes(result, targetRepresentations.Where(t => newIds.Contains(t.Id) && t.ResourceType == attributeMapping.TargetResourceType), attributeMapping, newSourceScimRepresentation, location, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType), newIds, true); ;
                        if (res.Item1)
                            isReferenceMissing = false;
                        else
                            missingIds.AddRange(res.Item2.Select(r => r.Id));
                        // UpdateReferenceAttributes(result, existingIds, attributeMapping, newSourceScimRepresentation, patchOperations, targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType), location: location, mode).Wait();
                    }
                }

				if (missingIds.Any() && isReferenceMissing)
					throw new SCIMNotFoundException(string.Format(Global.ReferencesDontExist, string.Join(",", missingIds.Distinct())));
            }

            yield return result;

            var allAddedIds = patchOperations.Where(p => p.Operation == SCIMPatchOperations.ADD).SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
            var allRemovedIds = patchOperations.Where(p => p.Operation == SCIMPatchOperations.REMOVE).SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == "value").Select(po => po.Attr.ValueString));
            SyncIndirectReferences(result, newSourceScimRepresentation, allAddedIds, allRemovedIds, propagatedAttribute, selfReferenceAttribute, targetSchemas, location, mode);
		}

		public async virtual Task<bool> IsReferenceProperty(ICollection<string> attributes)
        {
			var attrs = await _scimAttributeMappingQueryRepository.GetBySourceAttributes(attributes);
			return attributes.All(a => attrs.Any(at => at.SourceAttributeSelector == a));
		}

        protected IEnumerable<RepresentationSyncResult> SyncIndirectReferences(RepresentationSyncResult result, SCIMRepresentation newSourceScimRepresentation, IEnumerable<string> addedIds, IEnumerable<string> removedIds, SCIMAttributeMapping propagatedAttribute, SCIMAttributeMapping selfReferenceAttribute, IEnumerable<SCIMSchema> targetSchemas, string location, Mode mode)
        {
            // Update 'indirect' references.
            if (propagatedAttribute != null && selfReferenceAttribute != null)
            {
                var targetSchema = targetSchemas.First(s => s.ResourceType == propagatedAttribute.TargetResourceType);
                var sch = targetSchemas.First(s => s.ResourceType == propagatedAttribute.TargetResourceType);
                var parentAttr = sch.GetAttributeById(propagatedAttribute.TargetAttributeId);
                var valueAttr = sch.GetChildren(parentAttr).FirstOrDefault(a => a.Name == "value");
                var fullPath = valueAttr.FullPath;
                var addedUserIds = result.AddAttrEvts.Where(r => r.SchemaAttributeId == propagatedAttribute.TargetAttributeId).Select(r => r.RepresentationAggregateId).Distinct().ToList();
                var removedUserIds = result.RemoveAttrEvts.Where(r => r.SchemaAttributeId == propagatedAttribute.TargetAttributeId).Select(r => r.RepresentationAggregateId).Distinct().ToList();
                result = new RepresentationSyncResult(_resourceTypeResolver);

                IEnumerable<SCIMRepresentation> allParents = null;
                // Resolve parents and update 'indirect' references.
                if (addedUserIds.Any())
                {
                    allParents = ResolveParents(newSourceScimRepresentation, selfReferenceAttribute).ToList();
                    foreach (var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(addedUserIds))
                    {
                        foreach (var parent in allParents)
                        {
                            foreach (var child in paginatedResult.Item1)
                            {
                                var parentAttrs = child.GetAttributesByAttrSchemaId(propagatedAttribute.TargetAttributeId);
                                if (child.AddIndirectReference(parent.Id, propagatedAttribute.TargetAttributeId))
                                {
                                    var schema = targetSchemas.First(s => s.ResourceType == child.ResourceType);
                                    BuildScimRepresentationAttribute(propagatedAttribute.TargetAttributeId,
                                        child, parent, Mode.PROPAGATE_INHERITANCE,
                                        propagatedAttribute.SourceResourceType, schema, false);
                                    result.AddReferenceAttr(child, propagatedAttribute.TargetAttributeId, schema.GetAttributeById(propagatedAttribute.TargetAttributeId).FullPath, parent.Id, location);
                                }
                            }
                        }
                    }

                    yield return result;
                }

                // Resolve all children and update 'indirect' references.
                var users = new List<string>();
                if (removedUserIds.Any())
                {
                    var allSelfUserRepresentationIds = ResolveSelfChildrenIds(newSourceScimRepresentation, selfReferenceAttribute).ToList().Distinct().SelectMany(s => s).ToList();
                    foreach(var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(removedUserIds))
                    {
                        foreach(var removedUser in paginatedResult.Item1)
                        {
                            var directReferences = removedUser.HierarchicalAttributes.Where(a => a.SchemaAttributeId == propagatedAttribute.TargetAttributeId && a.CachedChildren.Any(c => c.SchemaAttribute?.Name == "value")).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
                            if (removedUser.IndirectReferences.Any(r => allSelfUserRepresentationIds.Contains(r.TargetReferenceId)) || directReferences.Any(r => allSelfUserRepresentationIds.Contains(r)))
                            {
                                UpdateScimRepresentation(removedUser, newSourceScimRepresentation, mode, propagatedAttribute.TargetAttributeId, propagatedAttribute.SourceResourceType, targetSchema, false, out bool isAttrUpdated);
                                result.AddReferenceAttr(removedUser, propagatedAttribute.TargetAttributeId, targetSchema.GetAttributeById(propagatedAttribute.TargetAttributeId).FullPath, newSourceScimRepresentation.Id, location);
                                removedUser.SetUpdated(DateTime.UtcNow);
                            }
                        }
                    }

                    yield return result;
                }

                if (addedIds.Any())
                {
                    foreach (var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(addedIds, newSourceScimRepresentation.ResourceType))
                    {
                        foreach(var addedGroup in paginatedResult.Item1)
                        {
                            foreach (var children in ResolveChildren(addedGroup, selfReferenceAttribute, propagatedAttribute))
                            {
                                result = new RepresentationSyncResult(_resourceTypeResolver);
                                foreach (var child in children)
                                {
                                    UpdateScimRepresentation(child, newSourceScimRepresentation, mode, propagatedAttribute.TargetAttributeId, propagatedAttribute.SourceResourceType, targetSchema, false, out bool isAttrUpdated);
                                    result.AddReferenceAttr(child, propagatedAttribute.TargetAttributeId, targetSchema.GetAttributeById(propagatedAttribute.TargetAttributeId).FullPath, newSourceScimRepresentation.Id, location);
                                    child.SetUpdated(DateTime.UtcNow);
                                }

                                yield return result;
                            }
                        }
                    }
                }

                if (removedIds.Any())
                {
                    foreach(var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedSCIMRepresentationByIds(removedIds, newSourceScimRepresentation.ResourceType))
                    {
                        foreach (var removedGroup in paginatedResult.Item1)
                        {
                            foreach (var children in ResolveChildren(removedGroup, selfReferenceAttribute, propagatedAttribute))
                            {
                                result = new RepresentationSyncResult(_resourceTypeResolver);
                                foreach (var child in children)
                                {
                                    child.RemoveIndirectReference(newSourceScimRepresentation.Id, propagatedAttribute.TargetAttributeId);
                                    if (child.IndirectReferences.Any(r => r.TargetAttributeId == propagatedAttribute.TargetAttributeId && r.TargetReferenceId == newSourceScimRepresentation.Id && r.NbReferences <= 0))
                                    {
                                        var parent = child.GetAttributesByPath(fullPath).FirstOrDefault(a => a.ValueString == newSourceScimRepresentation.Id);
                                        child.RemoveAttributesById(new string[] { parent.ParentAttributeId });
                                        result.RemoveReferenceAttr(child, propagatedAttribute.TargetAttributeId, parentAttr.FullPath, newSourceScimRepresentation.Id, location);
                                    }
                                }

                                yield return result;
                            }
                        }
                    }
                }
            }
        }

        protected virtual async Task ResolveIndirectChildren(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfAttributeMapping, SCIMAttributeMapping attributeMapping, List<SCIMRepresentation> lst)
        {
			var childrenIds = scimRepresentation.HierarchicalAttributes.Where(a => a.SchemaAttributeId == selfAttributeMapping.SourceAttributeId && a.CachedChildren.Any(c => c.SchemaAttribute?.Name == "value")).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
			var children = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(childrenIds, selfAttributeMapping.SourceResourceType);
			foreach(var child in children)
			{
				var indirectChildren = child.HierarchicalAttributes.Where(a => a.SchemaAttributeId == attributeMapping.SourceAttributeId && a.CachedChildren.Any(c => c.SchemaAttribute?.Name == "value")).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
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

        protected virtual IEnumerable<SCIMRepresentation> ResolveParents(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfReferenceAttribute)
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.value";
            var parents = _scimRepresentationCommandRepository.FindSCIMRepresentationsByAttributeFullPath(fullPath, new List<string> { scimRepresentation.Id }, selfReferenceAttribute.SourceResourceType).Result;
			foreach (var parent in parents)
				yield return parent;
            foreach (var parent in parents)
			{
				foreach (var p in ResolveParents(parent, selfReferenceAttribute))
					yield return p;
			}
        }

        protected virtual IEnumerable<IEnumerable<SCIMRepresentation>> ResolveChildren(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfAttributeMapping, SCIMAttributeMapping attributeMapping)
        {
            var childrenIds = scimRepresentation.HierarchicalAttributes.Where(a => a.SchemaAttributeId == selfAttributeMapping.SourceAttributeId && a.CachedChildren.Any(c => c.SchemaAttribute?.Name == "value")).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
            var indirectChildren = scimRepresentation.HierarchicalAttributes.Where(a => a.SchemaAttributeId == attributeMapping.SourceAttributeId && a.CachedChildren.Any(c => c.SchemaAttribute?.Name == "value")).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
            yield return _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(indirectChildren, attributeMapping.TargetResourceType).Result;
            var children = _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(childrenIds, selfAttributeMapping.SourceResourceType).Result;
            foreach (var child in children)
            {
				foreach (var sub in ResolveChildren(child, selfAttributeMapping, attributeMapping))
					yield return sub;
            }
        }

        protected virtual IEnumerable<IEnumerable<string>> ResolveSelfChildrenIds(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfAttributeMapping)
        {
            var childrenIds = scimRepresentation.HierarchicalAttributes.Where(a => a.SchemaAttributeId == selfAttributeMapping.SourceAttributeId && a.CachedChildren.Any(c => c.SchemaAttribute?.Name == "value")).Select(a => a.CachedChildren.First(c => c.SchemaAttribute.Name == "value").ValueString);
            yield return childrenIds;
            var children = _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(childrenIds, selfAttributeMapping.SourceResourceType).Result;
            foreach (var child in children)
                foreach (var strLst in ResolveSelfChildrenIds(child, selfAttributeMapping))
                    yield return strLst;
        }

        protected virtual bool RemoveReferenceAttributes(RepresentationSyncResult result, IEnumerable<SCIMRepresentation> targetRepresentations, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, string location)
        {
            if (targetRepresentations.Any())
            {
                var firstTargetRepresentation = targetRepresentations.First();
                foreach (var targetRepresentation in targetRepresentations)
                {
                    var attr = targetRepresentation.GetAttributesByAttrSchemaId(attributeMapping.TargetAttributeId).FirstOrDefault(v => targetRepresentation.GetChildren(v).Any(c => c.ValueString == sourceScimRepresentation.Id));
                    if (attr != null)
                    {
                        targetRepresentation.RemoveAttributeById(attr);
                        result.RemoveReferenceAttr(targetRepresentation, attr.SchemaAttribute.Id, attr.SchemaAttribute.FullPath, sourceScimRepresentation.Id, location);
                    }
                }

                return true;
            }

            return false;
        }

        protected virtual (bool, ICollection<SCIMRepresentation>) AddReferenceAttributes(RepresentationSyncResult result, IEnumerable<SCIMRepresentation> targetRepresentations, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, string location, SCIMSchema targetSchema, IEnumerable<string> ids, bool checkMissing = false)
        {
            var lst = new List<SCIMRepresentation>();
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
            }

            return (true, lst);
        }

        protected virtual async Task UpdateReferenceAttributes(RepresentationSyncResult result, IEnumerable<string> ids, SCIMAttributeMapping attributeMapping, SCIMRepresentation sourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, SCIMSchema targetSchema, string location, Mode mode)
        {
			if (!patchOperations.Any(p => IsDisplayName(p.Attr.FullPath)) || string.IsNullOrWhiteSpace(attributeMapping.TargetAttributeId)) return;
			var targetRepresentations = await _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(ids, attributeMapping.TargetResourceType);
			foreach (var targetRepresentation in targetRepresentations)
			{
				UpdateScimRepresentation(targetRepresentation, sourceScimRepresentation, mode, attributeMapping.TargetAttributeId, attributeMapping.SourceResourceType, targetSchema, true, out bool isAttrUpdated);
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

        public List<SCIMRepresentationAttribute> BuildScimRepresentationAttribute(string representationId, string attributeId, SCIMRepresentation sourceRepresentation, Mode mode, string sourceResourceType, SCIMSchema targetSchema, bool isDirect = true)
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
                    ValueString = sourceRepresentation.Id,
                    RepresentationId = representationId
                });
            }

            if (display != null)
            {
                attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), display, display.SchemaId)
                {
                    ValueString = sourceRepresentation.DisplayName,
                    RepresentationId = representationId
                });
            }

            if (type != null)
            {
                switch (mode)
                {
                    case Mode.STANDARD:
                        attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type, type.SchemaId)
                        {
                            ValueString = sourceResourceType,
                            RepresentationId = representationId
                        });
                        break;
                    case Mode.PROPAGATE_INHERITANCE:
                        attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type, type.SchemaId)
                        {
                            ValueString = isDirect ? "direct" : "indirect",
                            RepresentationId = representationId
                        });
                        break;
                }
            }

            var attrId = Guid.NewGuid().ToString();
            /*
            var attrs = targetRepresentation.GetAttributesByAttrSchemaId(targetSchemaAttribute.Id);
            if (attrs.Any())
            {
                attrId = attrs.First().AttributeId;
            }
            */

            var parentAttr = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attrId, targetSchemaAttribute, targetSchemaAttribute.SchemaId)
            {
                SchemaAttribute = targetSchemaAttribute
            };
            parentAttr.RepresentationId = representationId;
            foreach (var attr in attributes) attr.ParentAttributeId = parentAttr.Id;
            attributes.Add(parentAttr);
            return attributes;
        }

        private static bool IsDisplayName(string name)
        {
			return name == SCIMConstants.StandardSCIMReferenceProperties.Display || name == SCIMConstants.StandardSCIMReferenceProperties.DisplayName;
		}
    }
}
