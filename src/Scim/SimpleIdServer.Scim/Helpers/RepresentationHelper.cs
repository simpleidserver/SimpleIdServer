// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public interface IRepresentationHelper
    {
        Task<SCIMRepresentationPatchResult> Apply(SCIMRepresentation representation, IEnumerable<PatchOperationParameter> patchLst, IEnumerable<SCIMAttributeMapping> attributeMappings, bool ignoreUnsupportedCanonicalValues, CancellationToken cancellationToken);
    }

    public class RepresentationHelper : IRepresentationHelper
    {
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;

        public RepresentationHelper(ISCIMRepresentationCommandRepository scimRepresentationCommandRepository)
        {
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
        }

        public async Task<SCIMRepresentationPatchResult> Apply(SCIMRepresentation representation, IEnumerable<PatchOperationParameter> patchLst, IEnumerable<SCIMAttributeMapping> attributeMappings, bool ignoreUnsupportedCanonicalValues, CancellationToken cancellationToken)
        {
            var result = new SCIMRepresentationPatchResult();
            foreach (var patch in patchLst)
            {
                var scimFilter = SCIMFilterParser.Parse(patch.Path, representation.Schemas);
                var schemaAttributes = representation.Schemas.SelectMany(_ => _.Attributes);
                List<SCIMRepresentationAttribute> filteredAttributes = null, hierarchicalNewAttributes = null, hierarchicalFilteredAttributes = null;
                string fullPath = null;
                SCIMSchemaAttribute scimExprSchemaAttr = null;
                if (scimFilter != null)
                {
                    var scimExpr = scimFilter as SCIMAttributeExpression;
                    if (scimExpr == null) throw new SCIMAttributeException(Global.InvalidAttributeExpression);
                    scimExprSchemaAttr = scimExpr.GetLastChild().SchemaAttribute;
                    fullPath = scimExpr.GetFullPath();
                    schemaAttributes = representation.Schemas.Select(s => s.GetAttribute(fullPath)).Where(s => s != null);
                    fullPath = SCIMAttributeExpression.RemoveNamespace(fullPath);
                    filteredAttributes = await _scimRepresentationCommandRepository.FindAttributes(representation.Id, scimExpr, cancellationToken);
                    hierarchicalFilteredAttributes = SCIMRepresentation.BuildHierarchicalAttributes(filteredAttributes);
                }

                if (patch.Value != null)
                {
                    var attributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                    attributes = RemoveStandardReferenceProperties(attributes, attributeMappings);
                    hierarchicalNewAttributes = SCIMRepresentation.BuildHierarchicalAttributes(attributes);
                    if (scimFilter != null && attributes != null && !attributes.Any(a => a.IsLeaf()))
                    {
                        var lst = new List<SCIMRepresentationAttribute>();
                        foreach(var hNewAttribute in hierarchicalNewAttributes)
                        {
                            var record = scimFilter.BuildEmptyAttributes().FirstOrDefault();
                            record.UpdateValue(hNewAttribute.FullPath, hNewAttribute);
                            lst.Add(record);
                        }

                        hierarchicalNewAttributes = SCIMRepresentation.BuildHierarchicalAttributes(lst);
                    }
                }

                if(hierarchicalFilteredAttributes != null && hierarchicalNewAttributes != null) hierarchicalNewAttributes = FilterDuplicate(hierarchicalFilteredAttributes, hierarchicalNewAttributes);

                var removeCallback = new Action<ICollection<SCIMRepresentationAttribute>>((attrs) =>
                {
                    foreach (var a in attrs)
                    {
                        result.Remove(a);
                    }
                });

                switch (patch.Operation)
                {
                    case SCIMPatchOperations.ADD:
                        {
                            UpdateCommonAttributes(patch, representation, result, schemaAttributes);
                            if (hierarchicalFilteredAttributes == null)
                            {
                                await OverrideRootAttributes(result, hierarchicalNewAttributes, representation, cancellationToken);
                            }
                            else
                            {
                                
                                SCIMRepresentationAttribute arrayParentAttribute = null;
                                if (hierarchicalFilteredAttributes.Any() && hierarchicalNewAttributes.Any() && scimExprSchemaAttr.MultiValued && !SCIMRepresentationAttribute.IsLeaf(fullPath))
                                {
                                    var parentFullPath = SCIMRepresentationAttribute.GetParentFullPath(fullPath);
                                    arrayParentAttribute = filteredAttributes.First(f => f.FullPath == parentFullPath);
                                }

                                foreach (var newAttribute in hierarchicalNewAttributes)
                                {
                                    if (TryInsertComplexMultivaluedAttribute(result, newAttribute, arrayParentAttribute, scimExprSchemaAttr, representation)) continue;
                                    if (TryInsertValueIntoArray(result, newAttribute, scimExprSchemaAttr, representation)) continue;
                                    if (TryInsertWhenTargetLocationIsUnknown(result, newAttribute, hierarchicalFilteredAttributes, representation)) continue;
                                    ReplaceAttributes(result, newAttribute, hierarchicalFilteredAttributes, filteredAttributes, representation);
                                }
                            }
                        }
                        break;
                    case SCIMPatchOperations.REMOVE:
                        {
                            if (scimFilter == null) throw new SCIMNoTargetException(string.Format(Global.InvalidPath, patch.Path));
                            var attrToBeRemoved = filteredAttributes.Where(a => a.FullPath == fullPath || a.FullPath.StartsWith(fullPath)).ToList();
                            var removedRequiredAttributes = attrToBeRemoved.Where(a => a.SchemaAttribute.Required);
                            if (removedRequiredAttributes.Any())
                                throw new SCIMImmutableAttributeException(string.Format(string.Format(Global.RequiredAttributesCannotBeRemoved, string.Join(",", removedRequiredAttributes.Select(r => r.FullPath)))));

                            var removedReadOnlyAttributes = attrToBeRemoved.Where(a => a.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READONLY);
                            if (removedReadOnlyAttributes.Any())
                                throw new SCIMImmutableAttributeException(string.Format(string.Format(Global.ReadOnlyAttributesCannotBeRemoved, string.Join(",", removedReadOnlyAttributes.Select(r => r.FullPath)))));

                            removeCallback(attrToBeRemoved);
                        }
                        break;
                    case SCIMPatchOperations.REPLACE:
                        {
                            UpdateCommonAttributes(patch, representation, result, schemaAttributes);
                            var complexAttr = scimFilter as SCIMComplexAttributeExpression;
                            if (complexAttr != null && !hierarchicalFilteredAttributes.Any() && complexAttr.GroupingFilter != null) throw new SCIMNoTargetException(Global.PatchMissingAttribute);
                            if(hierarchicalFilteredAttributes == null)
                            {
                                await OverrideRootAttributes(result, hierarchicalNewAttributes, representation, cancellationToken);
                            }
                            else
                            {
                                foreach (var newAttribute in hierarchicalNewAttributes) ReplaceAttributes(result, newAttribute, hierarchicalFilteredAttributes, filteredAttributes, representation);
                            }
                        }
                        break;
                }
            }

            return result;
        }

        private async Task OverrideRootAttributes(SCIMRepresentationPatchResult result, List<SCIMRepresentationAttribute> hierarchicalNewAttributes, SCIMRepresentation representation, CancellationToken cancellationToken)
        {
            foreach (var newAttribute in hierarchicalNewAttributes)
            {
                var existingAttributes = await _scimRepresentationCommandRepository.FindAttributesByValueIndex(representation.Id, newAttribute.ComputedValueIndex, newAttribute.SchemaAttributeId, cancellationToken);
                if (existingAttributes.Any()) continue;
                var attrLstToBeRemoved = await _scimRepresentationCommandRepository.FindAttributesByFullPath(representation.Id, newAttribute.FullPath, cancellationToken);
                foreach (var attrToBeRemove in attrLstToBeRemoved) result.Remove(attrToBeRemove);
                if (newAttribute.SchemaAttribute.Type != SCIMSchemaAttributeTypes.COMPLEX)
                {
                    newAttribute.RepresentationId = representation.Id;
                    if (attrLstToBeRemoved.Any()) newAttribute.ParentAttributeId = attrLstToBeRemoved.First().ParentAttributeId;
                    result.Add(newAttribute);
                }
                else
                {
                    var flatAttrs = newAttribute.ToFlat();
                    foreach(var flatAttr in flatAttrs)
                    {
                        flatAttr.RepresentationId = representation.Id;
                        result.Add(flatAttr);
                    }
                }
            }
        }

        private void UpdateCommonAttributes(PatchOperationParameter patch, SCIMRepresentation representation, SCIMRepresentationPatchResult result, IEnumerable<SCIMSchemaAttribute> schemaAttributes)
        {
            bool hasExternalId = false;
            if ((hasExternalId = TryGetExternalId(patch, out string externalId)))
            {
                representation.ExternalId = externalId;
                result.AddExternalId();
            }

            if (!hasExternalId && (schemaAttributes == null || !schemaAttributes.Any()))
                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));

            if (TryGetDisplayName(patch, out string displayName))
                representation.SetDisplayName(displayName);
        }

        private bool TryInsertComplexMultivaluedAttribute(SCIMRepresentationPatchResult result, SCIMRepresentationAttribute newAttribute, SCIMRepresentationAttribute arrayParentAttribute, SCIMSchemaAttribute scimExprSchemaAttr, SCIMRepresentation representation)
        {
            if (!(scimExprSchemaAttr.Type == SCIMSchemaAttributeTypes.COMPLEX && scimExprSchemaAttr.MultiValued)) return false;
            var newFlatAttributes = newAttribute.ToFlat();
            foreach (var newFlatAttr in newFlatAttributes)
            {
                newFlatAttr.RepresentationId = representation.Id;
                if (arrayParentAttribute != null && newFlatAttr.FullPath == arrayParentAttribute.FullPath) continue;
                if (arrayParentAttribute != null && newFlatAttr.GetParentFullPath() == arrayParentAttribute.FullPath) newFlatAttr.ParentAttributeId = arrayParentAttribute.Id;
                result.Add(newFlatAttr);
            }

            return true;
        }

        private bool TryInsertValueIntoArray(SCIMRepresentationPatchResult result, SCIMRepresentationAttribute newAttribute, SCIMSchemaAttribute scimExprSchemaAttr, SCIMRepresentation representation)
        {
            if (!(scimExprSchemaAttr.Type != SCIMSchemaAttributeTypes.COMPLEX && scimExprSchemaAttr.MultiValued)) return false;
            newAttribute.RepresentationId = representation.Id;
            result.Add(newAttribute);
            return true;
        }

        private bool TryInsertWhenTargetLocationIsUnknown(SCIMRepresentationPatchResult result, SCIMRepresentationAttribute newAttribute, List<SCIMRepresentationAttribute> hierarchicalFilteredAttributes, SCIMRepresentation representation)
        {
            if (hierarchicalFilteredAttributes.Any()) return false;
            var newFlatAttributes = newAttribute.ToFlat();
            foreach (var newFlatAttr in newFlatAttributes)
            {
                newFlatAttr.RepresentationId = representation.Id;
                result.Add(newFlatAttr);
            }

            return true;
        }

        private void ReplaceAttributes(SCIMRepresentationPatchResult result, SCIMRepresentationAttribute newAttribute, List<SCIMRepresentationAttribute> hierarchicalFilteredAttributes, List<SCIMRepresentationAttribute> flatFilteredAttributes, SCIMRepresentation representation)
        {
            var newFlatAttributes = newAttribute.ToFlat();
            foreach (var newFlatAttr in newFlatAttributes)
            {
                if (newFlatAttr.SchemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX) continue;
                var path = newFlatAttr.FullPath;
                var schemaAttr = newFlatAttr.SchemaAttribute;
                // Update existing attributes.
                var existingAttributes = flatFilteredAttributes.Where(a => a.FullPath == path && !a.IsSimilar(newFlatAttr, true));
                foreach (var attr in existingAttributes)
                {
                    var clone = (SCIMRepresentationAttribute)newFlatAttr.Clone();
                    clone.Id = attr.Id;
                    clone.ParentAttributeId = attr.ParentAttributeId;
                    clone.RepresentationId = representation.Id;
                    result.Update(clone);
                }

                // Orphan parent.
                var orphanParents = hierarchicalFilteredAttributes.Where(a => !a.CachedChildren.Any(c => c.FullPath == path));
                foreach (var orphanParent in orphanParents)
                {
                    var clone = (SCIMRepresentationAttribute)newFlatAttr.Clone();
                    clone.Id = Guid.NewGuid().ToString();
                    clone.ParentAttributeId = orphanParent.Id;
                    clone.RepresentationId = representation.Id;
                    result.Add(clone);
                }
            }
        }

        private static bool TryGetExternalId(PatchOperationParameter patchOperation, out string externalId) => TryGetAttributeValue(patchOperation, StandardSCIMRepresentationAttributes.ExternalId, out externalId);

        private static bool TryGetDisplayName(PatchOperationParameter patchOperation, out string displayName) => TryGetAttributeValue(patchOperation, StandardSCIMRepresentationAttributes.ExternalId, out displayName);

        private static bool TryGetAttributeValue(PatchOperationParameter patchOperation, string attrName, out string value)
        {
            value = null;
            if (patchOperation.Value == null)
            {
                return false;
            }

            var jObj = patchOperation.Value as JObject;
            if (patchOperation.Path == attrName &&
                (patchOperation.Value.GetType() == typeof(string) || patchOperation.Value.GetType() == typeof(JValue)))
            {
                value = patchOperation.Value.ToString();
                return true;
            }

            if (jObj != null && jObj.ContainsKey(attrName))
            {
                value = jObj[attrName].ToString();
                return true;
            }

            return false;
        }

        private static ICollection<SCIMRepresentationAttribute> ExtractRepresentationAttributesFromJSON(ICollection<SCIMSchema> schemas, ICollection<SCIMSchemaAttribute> schemaAttributes, object obj, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMRepresentationAttribute>();
            var jArr = obj as JArray;
            var jObj = obj as JObject;
            if (jObj != null && schemaAttributes != null &&
                schemaAttributes.Any() &&
                schemaAttributes.Count() == 1 &&
                schemaAttributes.First().Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                jArr = new JArray();
                jArr.Add(jObj);
            }

            var mainSchema = schemas.First(s => s.IsRootSchema);
            var extensionSchemas = schemas.Where(s => !s.IsRootSchema).ToList();
            if (jArr != null)
            {
                var schemaAttr = schemaAttributes.First();
                var schema = schemas.FirstOrDefault(s => s.HasAttribute(schemaAttr));
                result.AddRange(SCIMRepresentationHelper.BuildAttributes(jArr, schemaAttr, schema, ignoreUnsupportedCanonicalValues));
            }
            else if (jObj != null)
            {
                var resolutionResult = SCIMRepresentationHelper.Resolve(jObj, mainSchema, extensionSchemas);
                result.AddRange(SCIMRepresentationHelper.BuildRepresentationAttributes(resolutionResult, resolutionResult.AllSchemaAttributes, ignoreUnsupportedCanonicalValues, true));
            }
            else if (schemaAttributes.Any() && schemaAttributes.Count() == 1)
            {
                var schemaAttribute = schemaAttributes.First();
                switch (schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valueBoolean = false;
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBoolean, schemaAttribute.FullPath));
                        if (!bool.TryParse(obj.ToString(), out valueBoolean)) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBoolean, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueBoolean: valueBoolean));
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidString, schemaAttribute.FullPath));
                        if (schemaAttribute.Required && string.IsNullOrWhiteSpace(obj.ToString())) throw new SCIMSchemaViolatedException(string.Format(Global.RequiredAttributesAreMissing, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueString: obj.ToString()));
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        int valueInteger;
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidInteger, schemaAttribute.FullPath));
                        if (!int.TryParse(obj.ToString(), out valueInteger)) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidInteger, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueInteger: valueInteger));
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        DateTime dt;
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDateTime, schemaAttribute.FullPath));
                        if (!DateTime.TryParse(obj.ToString(), out dt)) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDateTime, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueDateTime: dt));
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        throw new SCIMSchemaViolatedException(string.Format(Global.NotValidJSON, schemaAttribute.FullPath));
                }
            }

            return result;
        }

        private static List<SCIMRepresentationAttribute> RemoveStandardReferenceProperties(IEnumerable<SCIMRepresentationAttribute> newFlatAttributes, IEnumerable<SCIMAttributeMapping> attributeMappings)
        {
            return newFlatAttributes.Where((nfa) =>
            {
                var parentAttr = newFlatAttributes.FirstOrDefault(a => a.Id == nfa.ParentAttributeId);
                if (parentAttr == null || !attributeMappings.Any(am => am.SourceAttributeId == parentAttr.SchemaAttributeId)) return true;
                if (nfa.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Type || nfa.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Display) return false;
                return true;
            }).ToList();
        }

        private static List<SCIMRepresentationAttribute> FilterDuplicate(IEnumerable<SCIMRepresentationAttribute> existingAttributes, IEnumerable<SCIMRepresentationAttribute> newHierarchicalAttributes)
        {
            var result = new List<SCIMRepresentationAttribute>();
            foreach (var newHierarchicalAttribute in newHierarchicalAttributes)
            {
                if (existingAttributes.Any(a => a.IsSimilar(newHierarchicalAttribute, true))) continue;
                result.Add(newHierarchicalAttribute);
            }

            return result;
        }
    }
}
