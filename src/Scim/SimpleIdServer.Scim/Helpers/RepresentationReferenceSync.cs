// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationReferenceSync : IRepresentationReferenceSync
	{
        private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
		private readonly ISCIMSchemaCommandRepository _scimSchemaCommandRepository;

		public RepresentationReferenceSync(
			ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository,
			ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
			ISCIMSchemaCommandRepository scimSchemaCommandRepository)
        {
			_scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
			_scimRepresentationCommandRepository = scimRepresentationCommandRepository;
			_scimSchemaCommandRepository = scimSchemaCommandRepository;
		}

        public async Task<List<RepresentationSyncResult>> Sync(List<SCIMAttributeMapping> attributeMappings, string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, string location, SCIMSchema schema, bool updateAllReference = false, bool isRepresentationRemoved = false)
        {
            var result = new RepresentationSyncResult();
            var sync = new List<RepresentationSyncResult>();
            if (!attributeMappings.Any()) sync.Add(result);

            var mappedSchemas = (await _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappings.Select(a => a.TargetResourceType).Union(attributeMappings.Select(a => a.SourceResourceType)).Distinct())).ToList();
            var selfReferenceAttribute = attributeMappings.FirstOrDefault(a => a.IsSelf);
            var propagatedAttribute = attributeMappings.FirstOrDefault(a => a.Mode == Mode.PROPAGATE_INHERITANCE);
            var mode = propagatedAttribute == null ? Mode.STANDARD : Mode.PROPAGATE_INHERITANCE;
            var allAdded = new List<RepresentationModified>();
            var allRemoved = new List<RepresentationModified>();

            foreach (var kvp in attributeMappings.GroupBy(m => m.SourceAttributeId))
            {
                var sourceSchema = mappedSchemas.First(s => s.ResourceType == kvp.First().SourceResourceType && s.Attributes.Any(a => a.Id == kvp.Key)); 
                var allCurrentIds = patchOperations.Where(o => o.Attr.SchemaAttributeId == kvp.Key).SelectMany(pa => patchOperations.Where(p => p.Attr.ParentAttributeId == pa.Attr.Id && p.Attr.SchemaAttribute.Name == "value").Select(p => p.Attr.ValueString));
                var newIds = patchOperations
                    .Where(p => p.Operation == SCIMPatchOperations.ADD && p.Attr.SchemaAttributeId == kvp.Key)
                    .SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Value).Select(po => po.Attr.ValueString)).ToList();
                var idsToBeRemoved = patchOperations
                    .Where(p => p.Operation == SCIMPatchOperations.REMOVE && p.Attr.SchemaAttributeId == kvp.Key)
                    .SelectMany(p => patchOperations.Where(po => po.Attr.ParentAttributeId == p.Attr.Id && po.Attr.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Value).Select(po => po.Attr.ValueString)).ToList();
                var duplicateIds = allCurrentIds.GroupBy(i => i).Where(i => i.Count() > 1).ToList();
                if (duplicateIds.Any() && !isRepresentationRemoved)
                {
                    throw new SCIMUniquenessAttributeException(string.Format(Global.DuplicateReference, string.Join(",", duplicateIds.Select(_ => _.Key).Distinct())));
                }

                if(allCurrentIds.Contains(newSourceScimRepresentation.Id))
                {
                    throw new SCIMAttributeException(Global.RepresentationCannotHaveSelfReference);
                }

                // Update 'direct' references : GROUP => USER.
                foreach (var attributeMapping in kvp.Where(a => !a.IsSelf)) 
                {
                    var sourceAttribute = sourceSchema.GetAttributeById(attributeMapping.SourceAttributeId);
                    var targetSchema = mappedSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType && s.Attributes.Any(a => a.Id == attributeMapping.TargetAttributeId));
                    var targetAttribute = targetSchema.GetAttributeById(attributeMapping.TargetAttributeId);
                    var targetAttributeValue = targetSchema.GetChildren(targetAttribute).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                    var targetAttributeType = targetSchema.GetChildren(targetAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Type);
                    if (targetAttributeValue != null)
                    {
                        result = new RepresentationSyncResult();
                        var paginatedResult = await _scimRepresentationCommandRepository.FindGraphAttributes(idsToBeRemoved, new List<string> { newSourceScimRepresentation.Id }, targetAttributeValue.Id);
                        result.RemoveReferenceAttributes(paginatedResult);

                        var referencedAttrLst = SCIMRepresentation.BuildHierarchicalAttributes(await _scimRepresentationCommandRepository.FindGraphAttributes(newIds, new List<string> { newSourceScimRepresentation.Id }, targetAttributeValue.Id));
                        // Change indirect to direct.
                        foreach (var referencedAttr in referencedAttrLst)
                        {
                            var flatAttributes = referencedAttr.ToFlat();
                            var typeAttr = flatAttributes.Single(a => a.SchemaAttributeId == targetAttributeType.Id);
                            var valueAttr = flatAttributes.Single(a => a.SchemaAttributeId == targetAttributeValue.Id);
                            if(typeAttr.ValueString != "direct")
                            {
                                typeAttr.ValueString = "direct";
                                result.UpdateReferenceAttributes(new List<SCIMRepresentationAttribute> { typeAttr }, sync);
                            }
                        }

                        // Add reference attributes.
                        var missingReferenceAttributes = newIds.Where(i => !referencedAttrLst.Any(r => r.RepresentationId == i)).ToList();
                        var referencedRepresentations = (await _scimRepresentationCommandRepository.FindRepresentations(missingReferenceAttributes)).Where(r => r.ResourceType == attributeMapping.TargetResourceType);
                        foreach(var missingReferenceAttribute in missingReferenceAttributes)
                        {
                            var referencedRepresentation = referencedRepresentations.SingleOrDefault(r => r.Id == missingReferenceAttribute);
                            if (referencedRepresentation == null) continue;
                            var attr = BuildScimRepresentationAttribute(missingReferenceAttribute, attributeMapping.TargetAttributeId, newSourceScimRepresentation, mode, newSourceScimRepresentation.ResourceType, targetSchema);
                            result.AddReferenceAttributes(attr);
                            UpdateScimRepresentation(sync, result, patchOperations, referencedRepresentation, schema, sourceAttribute);
                        }

                        var removedIds = result.RemovedRepresentationAttributes.Where(a => a.SchemaAttributeId == targetAttributeValue.Id).Select(r => r.RepresentationId).ToList();
                        var addedIds = result.AddedRepresentationAttributes.Where(a => a.SchemaAttributeId == targetAttributeValue.Id).Select(r => r.RepresentationId).ToList();
                        var notRemovedIds = idsToBeRemoved.Except(removedIds);
                        var notAddedIds = newIds.Except(addedIds);
                        allRemoved.AddRange(notRemovedIds.Select(id => new RepresentationModified(id, false)));
                        allRemoved.AddRange(removedIds.Select(id => new RepresentationModified(id, true)));
                        allAdded.AddRange(notAddedIds.Select(id => new RepresentationModified(id, false)));
                        allAdded.AddRange(addedIds.Select(id => new RepresentationModified(id, true)));
                        sync.Add(result);
                    }
                }

                // Update indirect references : GROUP => GROUP.
                foreach (var attributeMapping in kvp.Where(a => a.IsSelf))
                {
                    var sourceAttribute = schema.GetAttributeById(attributeMapping.SourceAttributeId);
                    // Update the type.
                    var referencedRepresentations = (await _scimRepresentationCommandRepository.FindRepresentations(newIds)).Where(r => r.ResourceType == attributeMapping.TargetResourceType);
                    foreach (var referencedRepresentation in referencedRepresentations)
                    {
                        UpdateScimRepresentation(sync, result, patchOperations, referencedRepresentation, schema, sourceAttribute);
                    }

                    // Remove the reference.
                    if(isRepresentationRemoved)
                    {
                        var targetSchema = mappedSchemas.First(s => s.ResourceType == attributeMapping.TargetResourceType && s.Attributes.Any(a => a.Id == attributeMapping.SourceAttributeId));
                        var targetAttribute = targetSchema.GetAttributeById(attributeMapping.SourceAttributeId);
                        var targetAttributeValue = targetSchema.GetChildren(targetAttribute).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                        var paginatedResult = await _scimRepresentationCommandRepository.FindGraphAttributes(newSourceScimRepresentation.Id, targetAttributeValue.Id);
                        result.RemoveReferenceAttributes(paginatedResult);
                    }
                }
            }

            var syncIndirectReferences = await SyncIndirectReferences(newSourceScimRepresentation, allAdded, allRemoved, propagatedAttribute, selfReferenceAttribute, mappedSchemas, sync, updateAllReference);
            sync.AddRange(syncIndirectReferences);
            var r = await UpdateDisplayName(newSourceScimRepresentation, patchOperations, sync, CancellationToken.None);
            if(r != null)
            {
                sync.Add(r);
            }

            return sync;
        }

        public virtual async Task<bool> IsReferenceProperty(ICollection<string> attributes)
        {
            var attrs = await _scimAttributeMappingQueryRepository.GetBySourceAttributes(attributes);
            return attributes.All(a => attrs.Any(at => at.SourceAttributeSelector == a));
        }

        protected async Task<List<RepresentationSyncResult>> SyncIndirectReferences(SCIMRepresentation newSourceScimRepresentation, List<RepresentationModified> allAdded, List<RepresentationModified> allRemoved, SCIMAttributeMapping propagatedAttribute, SCIMAttributeMapping selfReferenceAttribute, List<SCIMSchema> mappedSchemas, List<RepresentationSyncResult> sync, bool updateAllReference)
        {
            // Update 'indirect' references.
            var result = new RepresentationSyncResult();
            var references = new List<RepresentationSyncResult>();
            List<SCIMRepresentation> allParents = null, allSelfChildren = null;
            SCIMSchema targetSchema, sourceSchema;
            SCIMSchemaAttribute parentAttr, valueAttr;
            if (propagatedAttribute != null && selfReferenceAttribute != null) 
            {
                sourceSchema = mappedSchemas.First(s => s.ResourceType == propagatedAttribute.SourceResourceType && s.Attributes.Any(a => a.Id == propagatedAttribute.SourceAttributeId));
                targetSchema = mappedSchemas.First(s => s.ResourceType == propagatedAttribute.TargetResourceType && s.Attributes.Any(a => a.Id == propagatedAttribute.TargetAttributeId));
                parentAttr = targetSchema.GetAttributeById(propagatedAttribute.TargetAttributeId);
                var selfParentAttr = sourceSchema.GetAttributeById(selfReferenceAttribute.SourceAttributeId);
                valueAttr = targetSchema.GetChildren(parentAttr).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                var displayAttr = targetSchema.GetChildren(parentAttr).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Display);
                var selfValueAttr = sourceSchema.GetChildren(selfParentAttr).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
                var addedDirectChildrenIds = allAdded.Where(r => r.IsDirect).Select(r => r.Id).ToList();
                var removedDirectChildrenIds = allRemoved.Where(r => r.IsDirect).Select(r => r.Id).ToList();
                var addedIndirectChildrenIds = allAdded.Where(r => !r.IsDirect).Select(r => r.Id).ToList();
                var removedIndirectChildrenIds = allRemoved.Where(r => !r.IsDirect).Select(r => r.Id).ToList();

                if(updateAllReference)
                {
                    var propagatedChildren = await _scimRepresentationCommandRepository.FindGraphAttributes(newSourceScimRepresentation.Id, valueAttr.Id);
                    var typeAttrs = propagatedChildren.Where(c => c.SchemaAttributeId == displayAttr.Id && !addedDirectChildrenIds.Contains(c.RepresentationId)).ToList();
                    foreach (var ta in typeAttrs)
                        ta.ValueString = newSourceScimRepresentation.DisplayName;

                    result.UpdateReferenceAttributes(typeAttrs, sync);
                    references.Add(result);
                }

                // Add indirect reference to the parent.
                if (addedDirectChildrenIds.Any())
                {
                    result = new RepresentationSyncResult();
                    allParents = await GetParents(new List<SCIMRepresentation> { newSourceScimRepresentation }, selfReferenceAttribute);
                    var existingParentReferencedIds = (await _scimRepresentationCommandRepository.FindAttributesBySchemaAttributeAndValues(propagatedAttribute.Id, allParents.Select(p => p.Id), CancellationToken.None)).Select(p => p.ValueString).ToList();
                    foreach(var parent in allParents.Where(p => !existingParentReferencedIds.Contains(p.Id)))
                        foreach(var addedId in addedDirectChildrenIds)
                            result.AddReferenceAttributes(BuildScimRepresentationAttribute(addedId, propagatedAttribute.TargetAttributeId, parent, Mode.PROPAGATE_INHERITANCE, parent.ResourceType, targetSchema, false));

                    references.Add(result);
                }

                // If at least one parent has a reference to the child then add indirect reference to the child.
                // Otherwise remove all the indirect references.
                if(removedDirectChildrenIds.Any())
                {
                    result = new RepresentationSyncResult();
                    allParents ??= await GetParents(new List<SCIMRepresentation> { newSourceScimRepresentation }, selfReferenceAttribute);
                    allSelfChildren = await GetChildren(new List<SCIMRepresentation> { newSourceScimRepresentation }, selfReferenceAttribute);
                    var allTargetIds = allSelfChildren.Where(r => r.ResourceType == targetSchema.ResourceType).Select(r => r.Id).ToList();
                    var allSelfIds = allSelfChildren.Where(r => r.ResourceType == sourceSchema.ResourceType).Select(r => r.Id).ToList();
                    var targets = await _scimRepresentationCommandRepository.FindGraphAttributesBySchemaAttributeId(allTargetIds, parentAttr.Id, CancellationToken.None);
                    foreach (var removedDirectChild in removedDirectChildrenIds)
                    {
                        var referencedParentIds = targets.Where(t => t.RepresentationId == removedDirectChild && t.SchemaAttributeId == valueAttr.Id).Select(t => t.ValueString);
                        if (referencedParentIds.Any(i => allSelfIds.Contains(i)))
                            result.AddReferenceAttributes(BuildScimRepresentationAttribute(removedDirectChild, propagatedAttribute.TargetAttributeId, newSourceScimRepresentation, Mode.PROPAGATE_INHERITANCE, newSourceScimRepresentation.ResourceType, targetSchema, false));
                        else
                        {
                            var hierarchicalTargetAttrs = SCIMRepresentation.BuildHierarchicalAttributes(targets.Where(t => t.RepresentationId == removedDirectChild));
                            foreach (var parent in allParents)
                            {
                                var attrToBeRemove = hierarchicalTargetAttrs.FirstOrDefault(t => t.CachedChildren.Any(c => c.ValueString == parent.Id && c.SchemaAttributeId == valueAttr.Id) &&
                                    t.CachedChildren.Any(c => c.ValueString == "indirect" && c.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Type));
                                if (attrToBeRemove != null)
                                    result.RemoveReferenceAttributes(attrToBeRemove.ToFlat());
                            }
                        }
                    }

                    references.Add(result);
                }

                // Populate the children.
                if (removedIndirectChildrenIds.Any() || addedIndirectChildrenIds.Any())
                {
                    // Refactor this part in order to improve the performance.
                    result = new RepresentationSyncResult();
                    allParents ??= await GetParents(new List<SCIMRepresentation> { newSourceScimRepresentation }, selfReferenceAttribute);
                    var allParentIds = allParents.Select(p => p.Id).ToList();
                    var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
                    var allParentAttributes = await _scimRepresentationCommandRepository.FindAttributesByExactFullPathAndValues(fullPath, new List<string> { newSourceScimRepresentation.Id }, CancellationToken.None);
                    var existingChildrenIds = (await _scimRepresentationCommandRepository.FindAttributesByAproximativeFullPath(newSourceScimRepresentation.Id, fullPath, CancellationToken.None)).Select(f => f.ValueString);
                    var allIds = new List<string>();
                    allIds.AddRange(addedIndirectChildrenIds);
                    allIds.AddRange(removedIndirectChildrenIds);
                    var notRemovedChildrenIds = existingChildrenIds.Except(removedIndirectChildrenIds).ToList();
                    var indirectChildren = await _scimRepresentationCommandRepository.FindRepresentations(allIds);
                    var notRemoved = await _scimRepresentationCommandRepository.FindRepresentations(notRemovedChildrenIds);

                    var notRemovedChildren = new List<SCIMRepresentation>();
                    notRemovedChildren.AddRange(await GetChildren(notRemoved, selfReferenceAttribute));

                    var notRemovedChldIds = notRemovedChildren.Select(r => r.Id);
                    var allDirectIds = allAdded.Where(a => a.IsDirect).Select(a => a.Id);
                    foreach (var indirectChild in indirectChildren)
                    {
                        var allChld = await GetChildren(new List<SCIMRepresentation> { indirectChild }, selfReferenceAttribute);
                        foreach (var children in await ResolvePropagatedChildren(newSourceScimRepresentation.Id, indirectChild, selfReferenceAttribute, valueAttr, allChld, allParentIds))
                        {
                            var filteredChildren = children.Where(c => !allDirectIds.Contains(c.RepresentationId));
                            if (addedIndirectChildrenIds.Contains(indirectChild.Id))
                            {
                                var parentAttrs = filteredChildren.Where(c => c.SchemaAttributeId == propagatedAttribute.TargetAttributeId);
                                foreach (var grp in parentAttrs.GroupBy(c => c.RepresentationId))
                                {
                                    var representationRootAttrIds = grp.Select(a => a.Id);
                                    var representationAttrs = filteredChildren.Where(a => representationRootAttrIds.Contains(a.ParentAttributeId) || representationRootAttrIds.Contains(a.Id));
                                    if (!representationAttrs.Any(r => r.SchemaAttributeId == valueAttr.Id && r.ValueString == newSourceScimRepresentation.Id))
                                    {
                                        result.AddReferenceAttributes(BuildScimRepresentationAttribute(grp.Key, propagatedAttribute.TargetAttributeId, newSourceScimRepresentation, Mode.PROPAGATE_INHERITANCE, newSourceScimRepresentation.ResourceType, targetSchema, false));
                                    }

                                    foreach(var allParent in allParents)
                                    {
                                        if (!representationAttrs.Any(r => r.SchemaAttributeId == valueAttr.Id && r.ValueString == allParent.Id))
                                        {
                                            result.AddReferenceAttributes(BuildScimRepresentationAttribute(grp.Key, propagatedAttribute.TargetAttributeId, allParent, Mode.PROPAGATE_INHERITANCE, allParent.ResourceType, targetSchema, false));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var representationIds = filteredChildren.Select(c => c.RepresentationId).Distinct();
                                foreach (var representationId in representationIds)
                                {
                                    var attrToRemoveLst = new List<SCIMRepresentationAttribute>();
                                    var parentAttrs = filteredChildren.Where(c => c.SchemaAttributeId == propagatedAttribute.TargetAttributeId && c.RepresentationId == representationId);
                                    foreach (var pa in parentAttrs)
                                    {
                                        var subAttrs = filteredChildren.Where(c => c.ParentAttributeId == pa.Id).ToList();
                                        if (subAttrs.Any(a => a.SchemaAttributeId == valueAttr.Id && notRemovedChldIds.Contains(a.ValueString)))
                                        {
                                            break;
                                        }

                                        if (subAttrs.Any(a => a.SchemaAttributeId == valueAttr.Id && (a.ValueString == newSourceScimRepresentation.Id || allParentIds.Contains(a.ValueString))))
                                            attrToRemoveLst.Add(pa);
                                        var parentAttribute = allParentAttributes.FirstOrDefault(p => subAttrs.Any(a => a.SchemaAttributeId == valueAttr.Id && a.ValueString == p.RepresentationId));
                                        if (parentAttribute != null) attrToRemoveLst.Add(parentAttribute);
                                    }

                                    if (attrToRemoveLst.Any())
                                    {
                                        foreach (var attrToRemove in attrToRemoveLst)
                                        {
                                            result.RemoveReferenceAttributes(filteredChildren.Where(a => a.ParentAttributeId == attrToRemove.Id).ToList());
                                            result.RemoveReferenceAttributes(new List<SCIMRepresentationAttribute> { attrToRemove });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    references.Add(result);
                }
            }

            return references;
        }

        protected async Task<RepresentationSyncResult> UpdateDisplayName(SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, List<RepresentationSyncResult> sync, CancellationToken cancellationToken)
        {
            var attributeMappingLst = await _scimAttributeMappingQueryRepository.GetByTargetResourceType(newSourceScimRepresentation.ResourceType);
            if(!attributeMappingLst.Any())
            {
                return null;
            }

            var updateDisplayName = patchOperations.FirstOrDefault(p => p.Operation == SCIMPatchOperations.REPLACE && IsDisplayName(p.Attr.SchemaAttribute.Name));
            if(updateDisplayName == null)
            {
                return null;
            }

            var mappedSchemas = (await _scimSchemaCommandRepository.FindSCIMSchemaByResourceTypes(attributeMappingLst.Select(a => a.SourceResourceType).Distinct())).ToList();
            var attrIds = attributeMappingLst.Select(m =>
            {
                var attr = mappedSchemas.First(s => s.ResourceType == m.SourceResourceType).GetChildren(m.SourceAttributeId);
                if(attr == null)
                {
                    return null;
                }

                var displayName = attr.SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Display)?.Id;
                var value = attr.SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value)?.Id;
                if(displayName == null || value == null)
                {
                    return null;
                }

                return new
                {
                    display = displayName,
                    value = value
                };
            }).Where(m => m != null).ToList();

            var result = new RepresentationSyncResult();
            foreach (var record in attrIds)
            {
                var attrs = await _scimRepresentationCommandRepository.FindGraphAttributes(newSourceScimRepresentation.Id, record.value, cancellationToken: cancellationToken);
                var displayAttrs = attrs.Where(a => a.SchemaAttributeId == record.display);
                foreach (var attr in displayAttrs)
                {
                    attr.ValueString = updateDisplayName.Attr.ValueString;
                    result.UpdateReferenceAttributes(new List<SCIMRepresentationAttribute> { attr }, sync);
                }
            }

            return result;
        }

        protected virtual async Task<List<SCIMRepresentation>> GetChildren(List<SCIMRepresentation> scimRepresentations, SCIMAttributeMapping selfReferenceAttribute) 
        {
            var childrenIds = new List<string>();
            await ResolveChildrenIds(scimRepresentations.Select(p => p.Id).ToList(), selfReferenceAttribute, childrenIds);
            return await _scimRepresentationCommandRepository.FindRepresentations(childrenIds);
        }

        protected virtual async Task<List<SCIMRepresentation>> GetParents(List<SCIMRepresentation> scimRepresentations, SCIMAttributeMapping selfReferenceAttribute) 
        {
            var parentIds = new List<string>();
            await ResolveParentIds(scimRepresentations.Select(p => p.Id).ToList(), selfReferenceAttribute, parentIds);
            return await _scimRepresentationCommandRepository.FindRepresentations(parentIds);
        }

        private async Task ResolveParentIds(List<string> ids,SCIMAttributeMapping selfReferenceAttribute, List<string> representationIds)
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
            var parents = await _scimRepresentationCommandRepository.FindAttributesByExactFullPathAndValues(fullPath, ids, CancellationToken.None);
            var newParents = parents.Where(p => !representationIds.Contains(p.RepresentationId)).ToList();
            if (!newParents.Any()) return;
            foreach (var parent in newParents)
                representationIds.Add(parent.RepresentationId);

            await ResolveParentIds(newParents.Select(p => p.RepresentationId).ToList(), selfReferenceAttribute, representationIds);
        }

        private async Task ResolveChildrenIds(List<string> ids, SCIMAttributeMapping selfReferenceAttribute, List<string> representationIds) 
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
            // members.value => for all the representationIds
            var children = await _scimRepresentationCommandRepository.FindAttributesByExactFullPathAndRepresentationIds(fullPath, ids, CancellationToken.None);
            var newChildren = children.Where(p => !representationIds.Contains(p.ValueString));
            if(!newChildren.Any()) return;
            foreach (var child in newChildren)
                representationIds.Add(child.ValueString);
            await ResolveChildrenIds(newChildren.Select(p => p.ValueString).ToList(), selfReferenceAttribute, representationIds);
        }

        protected virtual async Task<List<List<SCIMRepresentationAttribute>>> ResolvePropagatedChildren(string sourceRepresentationId, SCIMRepresentation scimRepresentation, SCIMAttributeMapping selfReferenceAttribute, SCIMSchemaAttribute targetAttributeId, List<SCIMRepresentation> allChildren, List<string> parentIds)
        {
            var fullPath = $"{selfReferenceAttribute.SourceAttributeSelector}.{SCIMConstants.StandardSCIMReferenceProperties.Value}";
            var attrs = await _scimRepresentationCommandRepository.FindAttributesByExactFullPathAndRepresentationIds(fullPath, new List<string> { scimRepresentation.Id }, CancellationToken.None);
            var childrenIds = attrs.Select(f => f.ValueString).ToList();
            var ids = new List<string>
            {
                scimRepresentation.Id
            };
            ids.AddRange(parentIds);
            var children = new List<List<SCIMRepresentationAttribute>>
            {
                await _scimRepresentationCommandRepository.FindGraphAttributes(childrenIds, ids, targetAttributeId.Id, sourceRepresentationId: sourceRepresentationId)
            };
            var allRepresentationIds = children.SelectMany(c => c.Select(a => a.RepresentationId)).Distinct().ToList();
            var parentAttributes = await _scimRepresentationCommandRepository.FindGraphAttributes(allRepresentationIds, parentIds, targetAttributeId.Id);
            children.First().AddRange(parentAttributes);
            foreach (var child in allChildren.Where(c => childrenIds.Contains(c.Id) && c.ResourceType == selfReferenceAttribute.SourceResourceType)) 
            {
                allChildren = await GetChildren(new List<SCIMRepresentation> { child }, selfReferenceAttribute);
                children.AddRange(await ResolvePropagatedChildren(sourceRepresentationId, child, selfReferenceAttribute, targetAttributeId, allChildren, parentIds));
            }

            return children;
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

        protected virtual void UpdateScimRepresentation(List<RepresentationSyncResult> sync, RepresentationSyncResult result, ICollection<SCIMPatchResult> patches, SCIMRepresentation targetRepresentation, SCIMSchema sourceSchema, SCIMSchemaAttribute sourceAttribute)
        {
            var sourceAttributeValue = sourceSchema.GetChildren(sourceAttribute).Single(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Value);
            var sourceAttributeType = sourceSchema.GetChildren(sourceAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Type);
            var sourceAttributeDisplay = sourceSchema.GetChildren(sourceAttribute).SingleOrDefault(a => a.Name == SCIMConstants.StandardSCIMReferenceProperties.Display);
            var flatAttrs = patches.Select(p => p.Attr);
            var representationId = flatAttrs.First().RepresentationId;
            var valAttr = flatAttrs.Single(a => a.SchemaAttributeId == sourceAttributeValue.Id && a.ValueString == targetRepresentation.Id);
            // When the attribute doesn't exist then insert it.
            // When the attribute already exists then update its value.
            if (sourceAttributeType != null)
            {
                var existingTypeAttribute = flatAttrs.SingleOrDefault(a => a.ParentAttributeId == valAttr.ParentAttributeId && a.SchemaAttributeId == sourceAttributeType.Id);
                if(existingTypeAttribute == null)
                {
                    result.AddReferenceAttributes(new List<SCIMRepresentationAttribute>
                    {
                        new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), sourceAttributeType, sourceAttributeType.SchemaId)
                        {
                            ValueString = targetRepresentation.ResourceType,
                            ParentAttributeId = valAttr.ParentAttributeId,
                            RepresentationId = representationId,
                            IsComputed = true
                        }
                    });
                }
                else if(existingTypeAttribute.ValueString != targetRepresentation.ResourceType)
                {
                    existingTypeAttribute.ValueString = targetRepresentation.ResourceType;
                    result.UpdateReferenceAttributes(new List<SCIMRepresentationAttribute> { existingTypeAttribute }, sync);
                }
            }

            if (sourceAttributeDisplay != null)
            {
                var existingDisplayAttribute = flatAttrs.SingleOrDefault(a => a.ParentAttributeId == valAttr.ParentAttributeId && a.SchemaAttributeId == sourceAttributeDisplay.Id);
                if(existingDisplayAttribute == null)
                {
                    result.AddReferenceAttributes(new List<SCIMRepresentationAttribute>
                    {
                        new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), sourceAttributeDisplay, sourceAttributeDisplay.SchemaId)
                        {
                            ValueString = targetRepresentation.DisplayName,
                            ParentAttributeId = valAttr.ParentAttributeId,
                            RepresentationId = representationId,
                            IsComputed = true
                        }
                    });
                }
                else if (existingDisplayAttribute.ValueString != targetRepresentation.DisplayName)
                {
                    existingDisplayAttribute.ValueString = targetRepresentation.DisplayName;
                    result.UpdateReferenceAttributes(new List<SCIMRepresentationAttribute> { existingDisplayAttribute }, sync);
                }
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

            var parentAttr = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), targetSchemaAttribute, targetSchemaAttribute.SchemaId)
            {
                SchemaAttribute = targetSchemaAttribute,
                RepresentationId = representationId
            };
            attributes.Add(parentAttr);

            if (value != null)
            {
                attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), value, value.SchemaId)
                {
                    ValueString = sourceRepresentation.Id,
                    RepresentationId = representationId,
                    ParentAttributeId = parentAttr.Id
                });
            }

            if (display != null)
            {
                attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), display, display.SchemaId)
                {
                    ValueString = sourceRepresentation.DisplayName,
                    RepresentationId = representationId,
                    IsComputed = true,
                    ParentAttributeId = parentAttr.Id
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
                            RepresentationId = representationId,
                            IsComputed = true,
                            ParentAttributeId = parentAttr.Id
                        });
                        break;
                    case Mode.PROPAGATE_INHERITANCE:
                        attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), type, type.SchemaId)
                        {
                            ValueString = isDirect ? "direct" : "indirect",
                            RepresentationId = representationId,
                            IsComputed = true,
                            ParentAttributeId = parentAttr.Id
                        });
                        break;
                }
            }

            return attributes;
        }

        private static bool IsDisplayName(string name)
        {
			return name == SCIMConstants.StandardSCIMReferenceProperties.Display || name == SCIMConstants.StandardSCIMReferenceProperties.DisplayName;
		}

        private class RepresentationTreeNode
        {
            public string RepresentationId { get; set; }
            public ICollection<RepresentationTreeNode> Nodes { get; set; }
        }
    }
}
