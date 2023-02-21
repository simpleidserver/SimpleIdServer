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

		public virtual IEnumerable<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, SCIMSchema schema, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
        {
            var attributeMappingLst = _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType).Result;
            return Sync(attributeMappingLst, resourceType, oldScimRepresentation, newSourceScimRepresentation, location, schema, updateAllReferences, isScimRepresentationRemoved);
		}

		public IEnumerable<RepresentationSyncResult> Sync(IEnumerable<SCIMAttributeMapping> attributeMappingLst, string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, SCIMSchema schema, bool updateAllReferences = false, bool isScimRepresentationRemoved = false)
        {
			var result = new RepresentationSyncResult();
            if (!attributeMappingLst.Any()) yield return result;

            var targetSchemas = _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct()).Result;
			var selfReferenceAttribute = attributeMappingLst.FirstOrDefault(a => a.IsSelf);
            var propagatedAttribute = attributeMappingLst.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
            var mode = propagatedAttribute == null ? Mode.STANDARD : Mode.PROPAGATE_INHERITANCE;
            // Update 'direct' references.
            var allAdded = new List<RepresentationModified>();
            var allRemoved = new List<RepresentationModified>();
            foreach (var kvp in attributeMappingLst.GroupBy(m => m.SourceAttributeId))
            {
                var allIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(kvp.Key).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Value)).Select(v => v.ValueString);
                IEnumerable<string> idsToBeRemoved = allIds, newIds = allIds, oldIds = new List<string>(), existingIds = new List<string>();
                if (!isScimRepresentationRemoved)
                {
                    oldIds = oldScimRepresentation.GetAttributesByAttrSchemaId(kvp.Key).SelectMany(a => oldScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Value)).Select(v => v.ValueString);
                    idsToBeRemoved = oldIds.Except(allIds);
                    var duplicateIds = allIds.GroupBy(i => i).Where(i => i.Count() > 1);
                    if (duplicateIds.Any()) throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
                    newIds = newIds.Except(oldIds).ToList();
                    existingIds = oldIds.Except(idsToBeRemoved);
                }

                foreach (var attributeMapping in kvp.Where(r => !r.IsSelf))
                {
                    result = new RepresentationSyncResult();
                    var targetSchema = targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType);
                    var targetAttribute = targetSchema.GetAttributeById(attributeMapping.TargetAttributeId);
                    var sourceAttribute = schema.GetAttributeById(attributeMapping.SourceAttributeId);
                    var targetAttributeValue = targetSchema.GetChildren(targetAttribute).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                    var targetAttributeType = targetSchema.GetChildren(targetAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Type);
                    if (isScimRepresentationRemoved)
                    {
                        var removedIds = new List<string>();
                        foreach (var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedGraphAttributes(newIds, newSourceScimRepresentation.Id, targetAttributeValue.Id))
                        {
                            result.RemoveReferenceAttributes(paginatedResult);
                            removedIds.AddRange(paginatedResult.Select(a => a.RepresentationId).Distinct());
                        }

                        var notRemovedIds = newIds.Except(removedIds);
                        allRemoved.AddRange(notRemovedIds.Select(id => new RepresentationModified(id, false)));
                        allRemoved.AddRange(removedIds.Select(id => new RepresentationModified(id, true)));
                    }
                    else
                    {
                        foreach (var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedGraphAttributes(idsToBeRemoved, newSourceScimRepresentation.Id, targetAttributeValue.Id))
                            result.RemoveReferenceAttributes(paginatedResult);

                        foreach(var representations in _scimRepresentationCommandRepository.FindPaginatedRepresentations(newIds, attributeMapping.TargetResourceType))
                            foreach(var rep in representations)
                            {
                                var fa = rep.FlatAttributes.SingleOrDefault(a => a.SchemaAttributeId == targetAttributeValue.Id && a.ValueString == newSourceScimRepresentation.Id);
                                if(fa == null)
                                    result.AddReferenceAttributes(BuildScimRepresentationAttribute(rep.Id, attributeMapping.TargetAttributeId, newSourceScimRepresentation, mode, newSourceScimRepresentation.ResourceType, targetSchema));
                                else
                                {
                                    var type = rep.FlatAttributes.Single(a => a.SchemaAttributeId == targetAttributeType.Id && a.ParentAttributeId == fa.ParentAttributeId);
                                    if(type.ValueString != "direct")
                                    {
                                        type.ValueString = "direct";
                                        result.UpdateReferenceAttributes(new List<SCIMRepresentationAttribute> { type });
                                    }
                                }

                                UpdateScimRepresentation(newSourceScimRepresentation, rep, schema, sourceAttribute);                        
                            }
                        
                        var removedIds = result.RemovedRepresentationAttributes.Where(a => a.SchemaAttributeId == targetAttributeValue.Id).Select(r => r.RepresentationId);
                        var addedIds = result.AddedRepresentationAttributes.Where(a => a.SchemaAttributeId == targetAttributeValue.Id).Select(r => r.RepresentationId);
                        var notRemovedIds = idsToBeRemoved.Except(removedIds);
                        var notAddedIds = newIds.Except(addedIds);
                        allRemoved.AddRange(notRemovedIds.Select(id => new RepresentationModified(id, false)));
                        allRemoved.AddRange(removedIds.Select(id => new RepresentationModified(id, true)));
                        allAdded.AddRange(notAddedIds.Select(id => new RepresentationModified(id, false)));
                        allAdded.AddRange(addedIds.Select(id => new RepresentationModified(id, true)));
                    }

                    yield return result;
                }

                if(newIds.Any())
                    foreach (var attributeMapping in kvp.Where(a => a.IsSelf))
                    {
                        var sourceAttribute = schema.GetAttributeById(attributeMapping.SourceAttributeId);
                        foreach (var representations in _scimRepresentationCommandRepository.FindPaginatedRepresentations(newIds, attributeMapping.TargetResourceType))
                            foreach (var rep in representations)
                                UpdateScimRepresentation(newSourceScimRepresentation, rep, schema, sourceAttribute);
                    }
            }

            var syncIndirectReferences = SyncIndirectReferences(newSourceScimRepresentation, allAdded, allRemoved, propagatedAttribute, selfReferenceAttribute, targetSchemas, location, mode, updateAllReferences);
            foreach (var syncIndirectReference in syncIndirectReferences)
                yield return syncIndirectReference;
        }

		public  IEnumerable<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, string location, SCIMSchema schema, bool updateAllReference = false)
		{
			var result = new RepresentationSyncResult();
			var attributeMappingLst = _scimAttributeMappingQueryRepository.GetBySourceResourceType(resourceType).Result;
			if (!attributeMappingLst.Any()) yield return result;

			var targetSchemas = _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.TargetResourceType).Distinct()).Result;
            var selfReferenceAttribute = attributeMappingLst.FirstOrDefault(a => a.IsSelf);
			var propagatedAttribute = attributeMappingLst.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
			var mode = propagatedAttribute == null ? Mode.STANDARD : Mode.PROPAGATE_INHERITANCE;
            var allAdded = new List<RepresentationModified>();
            var allRemoved = new List<RepresentationModified>();

            // Update 'direct' references.
            foreach (var kvp in attributeMappingLst.GroupBy(m => m.SourceAttributeId))
			{
				var allCurrentIds = newSourceScimRepresentation.GetAttributesByAttrSchemaId(kvp.Key).SelectMany(a => newSourceScimRepresentation.GetChildren(a).Where(v => v.SchemaAttribute.Name == "value")).Select(v => v.ValueString);
				var newIds = patchOperations
					.Where(p => p.Operation == SCIMPatchOperations.ADD && p.Attr.SchemaAttributeId == kvp.Key)
					.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Value).Select(po => po.Attr.ValueString));
				var idsToBeRemoved = patchOperations
					.Where(p => p.Operation == SCIMPatchOperations.REMOVE && p.Attr.SchemaAttributeId == kvp.Key)
					.SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Value).Select(po => po.Attr.ValueString));
				var duplicateIds = allCurrentIds.GroupBy(i => i).Where(i => i.Count() > 1);
				if (duplicateIds.Any()) throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
                foreach (var attributeMapping in kvp.Where(a => !a.IsSelf))
                {
                    var sourceAttribute = schema.GetAttributeById(attributeMapping.SourceAttributeId);
                    var targetSchema = targetSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType);
                    var targetAttribute = targetSchema.GetAttributeById(attributeMapping.TargetAttributeId);
                    var targetAttributeValue = targetSchema.GetChildren(targetAttribute).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                    var targetAttributeType = targetSchema.GetChildren(targetAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Type);
                    if (targetAttributeValue != null)
                    {
                        result = new RepresentationSyncResult();
                        foreach (var paginatedResult in _scimRepresentationCommandRepository.FindPaginatedGraphAttributes(idsToBeRemoved, newSourceScimRepresentation.Id, targetAttributeValue.Id))
                            result.RemoveReferenceAttributes(paginatedResult);

                        foreach (var representations in _scimRepresentationCommandRepository.FindPaginatedRepresentations(newIds, attributeMapping.TargetResourceType))
                            foreach (var rep in representations)
                            {
                                var attr = rep.FlatAttributes.FirstOrDefault(a => a.ValueString == newSourceScimRepresentation.Id && a.SchemaAttributeId == targetAttributeValue.Id);
                                if (attr == null)
                                    result.AddReferenceAttributes(BuildScimRepresentationAttribute(rep.Id, attributeMapping.TargetAttributeId, newSourceScimRepresentation, mode, newSourceScimRepresentation.ResourceType, targetSchema));
                                else
                                {
                                    var typeAttr = rep.FlatAttributes.Single(a => a.SchemaAttributeId == targetAttributeType.Id && a.ParentAttributeId == attr.ParentAttributeId);
                                    if(typeAttr.ValueString != "direct")
                                    {
                                        typeAttr.ValueString = "direct";
                                        result.UpdateReferenceAttributes(new List<SCIMRepresentationAttribute> { typeAttr });
                                    }
                                }

                                UpdateScimRepresentation(newSourceScimRepresentation, rep, schema, sourceAttribute);
                            }

                        var removedIds = result.RemovedRepresentationAttributes.Where(a => a.SchemaAttributeId == targetAttributeValue.Id).Select(r => r.RepresentationId);
                        var addedIds = result.AddedRepresentationAttributes.Where(a => a.SchemaAttributeId == targetAttributeValue.Id).Select(r => r.RepresentationId);
                        var notRemovedIds = idsToBeRemoved.Except(removedIds);
                        var notAddedIds = newIds.Except(addedIds);
                        allRemoved.AddRange(notRemovedIds.Select(id => new RepresentationModified(id, false)));
                        allRemoved.AddRange(removedIds.Select(id => new RepresentationModified(id, true)));
                        allAdded.AddRange(notAddedIds.Select(id => new RepresentationModified(id, false)));
                        allAdded.AddRange(addedIds.Select(id => new RepresentationModified(id, true)));
                        yield return result;
                    }
                }

                if(newIds.Any())
                    foreach(var attributeMapping in kvp.Where(a => a.IsSelf))
                    {
                        var sourceAttribute = schema.GetAttributeById(attributeMapping.SourceAttributeId);
                        foreach (var representations in _scimRepresentationCommandRepository.FindPaginatedRepresentations(newIds, attributeMapping.TargetResourceType))
                            foreach(var rep in representations)
                            UpdateScimRepresentation(newSourceScimRepresentation, rep, schema, sourceAttribute);
                    }
            }


            var syncIndirectReferences = SyncIndirectReferences(newSourceScimRepresentation, allAdded, allRemoved, propagatedAttribute, selfReferenceAttribute, targetSchemas, location, mode, updateAllReference);
            foreach (var syncIndirectReference in syncIndirectReferences)
                yield return syncIndirectReference;
		}

		public async virtual Task<bool> IsReferenceProperty(ICollection<string> attributes)
        {
			var attrs = await _scimAttributeMappingQueryRepository.GetBySourceAttributes(attributes);
			return attributes.All(a => attrs.Any(at => at.SourceAttributeSelector == a));
		}

        protected IEnumerable<RepresentationSyncResult> SyncIndirectReferences(SCIMRepresentation newSourceScimRepresentation, IEnumerable<RepresentationModified> allAdded, IEnumerable<RepresentationModified> allRemoved, SCIMAttributeMapping propagatedAttribute, SCIMAttributeMapping selfReferenceAttribute, IEnumerable<SCIMSchema> targetSchemas, string location, Mode mode, bool updateAllReference)
        {
            // Update 'indirect' references.
            if (propagatedAttribute != null && selfReferenceAttribute != null)
            {
                var result = new RepresentationSyncResult();
                var targetSchema = targetSchemas.Single(s => s.Name == propagatedAttribute.TargetResourceType);
                var sourceSchema = targetSchemas.Single(s => s.Name == propagatedAttribute.SourceResourceType);
                var parentAttr = targetSchema.GetAttributeById(propagatedAttribute.TargetAttributeId);
                var selfParentAttr = sourceSchema.GetAttributeById(selfReferenceAttribute.SourceAttributeId);
                var valueAttr = targetSchema.GetChildren(parentAttr).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                var displayAttr = targetSchema.GetChildren(parentAttr).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Display);
                var selfValueAttr = sourceSchema.GetChildren(selfParentAttr).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                var addedDirectChildrenIds = allAdded.Where(r => r.IsDirect).Select(r => r.Id);
                var removedDirectChildrenIds = allRemoved.Where(r => r.IsDirect).Select(r => r.Id);
                var addedIndirectChildrenIds = allAdded.Where(r => !r.IsDirect).Select(r => r.Id);
                var removedIndirectChildrenIds = allRemoved.Where(r => !r.IsDirect).Select(r => r.Id);

                IEnumerable<SCIMRepresentation> allParents = null;
                var sw = new Stopwatch();
                sw.Start();
                // Update displayName.
                if(updateAllReference)
                {
                    foreach (var propagatedChildren in _scimRepresentationCommandRepository.FindPaginatedGraphAttributes(newSourceScimRepresentation.Id, valueAttr.Id))
                    {
                        var typeAttrs = propagatedChildren.Where(c => c.SchemaAttributeId == displayAttr.Id).ToList();
                        foreach (var ta in typeAttrs)
                            ta.ValueString = newSourceScimRepresentation.DisplayName;

                        result.UpdateReferenceAttributes(typeAttrs);
                    }

                    yield return result;
                }

                Console.WriteLine($"update display name {sw.ElapsedMilliseconds}");
                sw.Restart();

                // Add indirect reference to the parent.
                if (addedDirectChildrenIds.Any())
                {
                    result = new RepresentationSyncResult();
                    allParents = ResolveParents(newSourceScimRepresentation, selfReferenceAttribute).ToList();
                    foreach (var parent in allParents)
                        foreach(var addedId in addedDirectChildrenIds)
                            if (!parent.FlatAttributes.Any(a => a.SchemaAttributeId == selfValueAttr.Id && a.ValueString == addedId))
                                result.AddReferenceAttributes(BuildScimRepresentationAttribute(addedId, propagatedAttribute.TargetAttributeId, parent, Mode.PROPAGATE_INHERITANCE, parent.ResourceType, targetSchema, false));

                    yield return result;
                }

                Console.WriteLine($"add indirect reference to the parent {sw.ElapsedMilliseconds}");
                sw.Restart();

                // If at least one parent has a reference to the child then add indirect reference to the child.
                if (removedDirectChildrenIds.Any())
                {
                    result = new RepresentationSyncResult();
                    if(allParents == null)
                        allParents = ResolveParents(newSourceScimRepresentation, selfReferenceAttribute).ToList();
                    var children = ResolveChildren(newSourceScimRepresentation, selfReferenceAttribute);
                    foreach (var removedDirectChild in removedDirectChildrenIds)
                    {
                        if(allParents.Any(p => p.FlatAttributes.Any(a => a.SchemaAttributeId == selfValueAttr.Id && a.ValueString == removedDirectChild)) || children.Any(p => p.FlatAttributes.Any(a => a.SchemaAttributeId == selfValueAttr.Id && a.ValueString == removedDirectChild)))
                            result.AddReferenceAttributes(BuildScimRepresentationAttribute(removedDirectChild, propagatedAttribute.TargetAttributeId, newSourceScimRepresentation, Mode.PROPAGATE_INHERITANCE, newSourceScimRepresentation.ResourceType, targetSchema, false));
                    }

                    yield return result;
                }

                Console.WriteLine($"removedDirectChildrenIds {sw.ElapsedMilliseconds}");
                sw.Restart();

                // Populate the children.
                if (addedIndirectChildrenIds.Any() || removedIndirectChildrenIds.Any())
                {
                    // Refactor this part in order to improve the performance.
                    result = new RepresentationSyncResult();
                    var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
                    var childrenIds = newSourceScimRepresentation.FlatAttributes.Where(f => f.FullPath == fullPath).Select(f => f.ValueString);
                    var allIds = new List<string>();
                    allIds.AddRange(addedIndirectChildrenIds);
                    allIds.AddRange(removedIndirectChildrenIds);
                    var notRemovedChildrenIds = childrenIds.Except(removedIndirectChildrenIds);
                    var indirectChildren = _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(allIds).Result;
                    var notRemoved = _scimRepresentationCommandRepository.FindSCIMRepresentationByIds(notRemovedChildrenIds).Result;
                    var notRemovedChildren = notRemoved.SelectMany(r => ResolveChildren(r, selfReferenceAttribute).ToList());
                    var notRemovedChldIds = notRemovedChildren.Select(r => r.Id);

                    foreach (var indirectChild in indirectChildren)
                    {
                        var allChld = ResolveChildren(indirectChild, selfReferenceAttribute);
                        foreach (var children in ResolvePropagatedChildren(newSourceScimRepresentation.Id, indirectChild, selfReferenceAttribute, valueAttr, allChld))
                        {
                            if(addedIndirectChildrenIds.Contains(indirectChild.Id))
                            {
                                var parentAttrs = children.Where(c => c.SchemaAttributeId == propagatedAttribute.TargetAttributeId);
                                foreach (var pa in parentAttrs)
                                {
                                    if (!children.Any(r => r.ParentAttributeId == pa.Id && r.SchemaAttributeId == valueAttr.Id && r.ValueString == newSourceScimRepresentation.Id))
                                        result.AddReferenceAttributes(BuildScimRepresentationAttribute(pa.RepresentationId, propagatedAttribute.TargetAttributeId, newSourceScimRepresentation, Mode.PROPAGATE_INHERITANCE, newSourceScimRepresentation.ResourceType, targetSchema, false));
                                }
                            }
                            else
                            {
                                var representationIds = children.Select(c => c.RepresentationId).Distinct();
                                foreach(var representationId in representationIds)
                                {
                                    bool atLeastOneParent = false;
                                    SCIMRepresentationAttribute attrToRemove = null;
                                    var parentAttrs = children.Where(c => c.SchemaAttributeId == propagatedAttribute.TargetAttributeId && c.RepresentationId == representationId);
                                    foreach (var pa in parentAttrs)
                                    {
                                        var subAttrs = children.Where(c => c.ParentAttributeId == pa.Id);
                                        if (subAttrs.Any(a => a.SchemaAttributeId == valueAttr.Id && notRemovedChldIds.Contains(a.ValueString)))
                                        {
                                            atLeastOneParent = true;
                                            break;
                                        }

                                        if (subAttrs.Any(a => a.SchemaAttributeId == valueAttr.Id && a.ValueString == newSourceScimRepresentation.Id))
                                            attrToRemove = pa;
                                    }

                                    if(!atLeastOneParent && attrToRemove != null)
                                    {
                                        result.RemoveReferenceAttributes(children.Where(a => a.ParentAttributeId == attrToRemove.Id).ToList());
                                        result.RemoveReferenceAttributes(new List<SCIMRepresentationAttribute> { attrToRemove });
                                    }
                                }
                            }
                        }
                    }

                    yield return result;
                }

                Console.WriteLine($"addedIndirectChildrenIds {sw.ElapsedMilliseconds}");
                sw.Restart();
            }
        }

        protected virtual IEnumerable<SCIMRepresentation> ResolveParents(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfReferenceAttribute)
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
            var parents = _scimRepresentationCommandRepository.FindSCIMRepresentationsByAttributeFullPath(fullPath, new List<string> { scimRepresentation.Id }, selfReferenceAttribute.SourceResourceType).Result;
			foreach (var parent in parents)
				yield return parent;
            foreach (var parent in parents)
			{
				foreach (var p in ResolveParents(parent, selfReferenceAttribute))
					yield return p;
			}
        }

        protected virtual IEnumerable<SCIMRepresentation> ResolveChildren(SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfReferenceAttribute)
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
            var childrenIds = scimRepresentation.FlatAttributes.Where(f => f.FullPath == fullPath).Select(f => f.ValueString);
            foreach (var children in _scimRepresentationCommandRepository.FindPaginatedRepresentations(childrenIds, selfReferenceAttribute.TargetResourceType))
            {
                foreach (var child in children)
                {
                    yield return child;
                    foreach (var chld in ResolveChildren(child, selfReferenceAttribute))
                    {
                        yield return chld;
                    }
                }
            }
        }

        protected virtual IEnumerable<IEnumerable<SCIMRepresentationAttribute>> ResolvePropagatedChildren(string sourceRepresentationId, SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfReferenceAttribute, SCIMSchemaAttribute targetAttributeId, IEnumerable<SCIMRepresentation> allChildren)
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
            var childrenIds = scimRepresentation.FlatAttributes.Where(f => f.FullPath == fullPath).Select(f => f.ValueString);
            foreach (var propagatedChildren in _scimRepresentationCommandRepository.FindPaginatedGraphAttributes(childrenIds, scimRepresentation.Id, targetAttributeId.Id, sourceRepresentationId: sourceRepresentationId))
                yield return propagatedChildren;

            foreach (var child in allChildren.Where(c => childrenIds.Contains(c.Id)))
            {
                foreach (var strLst in ResolvePropagatedChildren(sourceRepresentationId, child, selfReferenceAttribute, targetAttributeId, allChildren))
                    yield return strLst;
            }
        }

        protected class RepresentationModified
        {
            public RepresentationModified(string id, bool isDirect)
            {
                Id = id;
                IsDirect = isDirect;
            }

            public string Id { get; private set; }
            public bool IsDirect { get; private set; }
        }

        protected virtual void UpdateScimRepresentation(SCIMRepresentation sourceRepresentation, SCIMRepresentation targetRepresentation, SCIMSchema sourceSchema, SCIMSchemaAttribute sourceAttribute)
        {
            var sourceAttributeValue = sourceSchema.GetChildren(sourceAttribute).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
            var sourceAttributeType = sourceSchema.GetChildren(sourceAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Type);
            var sourceAttributeDisplay = sourceSchema.GetChildren(sourceAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Display);
            var valAttr = sourceRepresentation.FlatAttributes.Single(a => a.SchemaAttributeId == sourceAttributeValue.Id && a.ValueString == targetRepresentation.Id);
            if (sourceAttributeType != null)
            {
                sourceRepresentation.FlatAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), sourceAttributeType, sourceAttributeType.SchemaId)
                {
                    ValueString = targetRepresentation.ResourceType,
                    ParentAttributeId = valAttr.ParentAttributeId
                });
            }

            if (sourceAttributeDisplay != null)
            {
                sourceRepresentation.FlatAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), sourceAttributeDisplay, sourceAttributeDisplay.SchemaId)
                {
                    ValueString = targetRepresentation.DisplayName,
                    ParentAttributeId = valAttr.ParentAttributeId
                });
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
