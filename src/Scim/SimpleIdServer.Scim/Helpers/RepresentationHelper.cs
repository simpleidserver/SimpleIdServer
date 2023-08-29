// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
        Task<List<SCIMPatchResult>> Apply(SCIMRepresentation representation, IEnumerable<PatchOperationParameter> patchLst, IEnumerable<SCIMAttributeMapping> attributeMappings, bool ignoreUnsupportedCanonicalValues, CancellationToken cancellationToken);
    }

    public class RepresentationHelper : IRepresentationHelper
    {
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;

        public RepresentationHelper(ISCIMRepresentationCommandRepository scimRepresentationCommandRepository)
        {
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
        }

        public async Task<List<SCIMPatchResult>> Apply(SCIMRepresentation representation, IEnumerable<PatchOperationParameter> patchLst, IEnumerable<SCIMAttributeMapping> attributeMappings, bool ignoreUnsupportedCanonicalValues, CancellationToken cancellationToken)
        {
            var result = new List<SCIMPatchResult>();
            foreach (var patch in patchLst)
            {
                var scimFilter = SCIMFilterParser.Parse(patch.Path, representation.Schemas);
                var schemaAttributes = representation.Schemas.SelectMany(_ => _.Attributes);
                List<SCIMRepresentationAttribute> filteredAttributes = null;
                string fullPath = null;
                SCIMRepresentationAttribute emptyParent = null;
                if (scimFilter != null)
                {
                    var scimExpr = scimFilter as SCIMAttributeExpression;
                    if (scimExpr == null) throw new SCIMAttributeException(Global.InvalidAttributeExpression);
                    fullPath = scimExpr.GetFullPath();
                    schemaAttributes = representation.Schemas.Select(s => s.GetAttribute(fullPath)).Where(s => s != null);
                    fullPath = SCIMAttributeExpression.RemoveNamespace(fullPath);
                    filteredAttributes = await _scimRepresentationCommandRepository.FindAttributes(representation.Id, scimExpr, cancellationToken);
                    emptyParent = scimExpr.BuildEmptyAttributes().FirstOrDefault();
                }
                // else filteredAttributes = representation.FlatAttributes.Where(h => h.IsLeaf()).ToList();

                var removeCallback = new Action<ICollection<SCIMRepresentationAttribute>>((attrs) =>
                {
                    foreach (var a in attrs)
                    {
                        result.Add(new SCIMPatchResult { Attr = a, Operation = SCIMPatchOperations.REMOVE, Path = a.FullPath });
                    }
                });

                var hierarchicalFilteredAttributes = SCIMRepresentation.BuildHierarchicalAttributes(filteredAttributes);
                switch (patch.Operation)
                {
                    case SCIMPatchOperations.ADD:
                        {
                            bool hasExternalId = false;
                            if ((hasExternalId = TryGetExternalId(patch, out string externalId)))
                            {
                                representation.ExternalId = externalId;
                                result.Add(new SCIMPatchResult { Attr = new SCIMRepresentationAttribute(), Operation = SCIMPatchOperations.ADD, Path = StandardSCIMRepresentationAttributes.ExternalId });
                            }

                            if (!hasExternalId && (schemaAttributes == null || !schemaAttributes.Any()))
                                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));

                            var hierarchicalNewAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                            var flatNewAttributes = hierarchicalNewAttributes.SelectMany(a => a.ToFlat());
                            flatNewAttributes = RemoveStandardReferenceProperties(flatNewAttributes, attributeMappings);
                            flatNewAttributes = FilterDuplicate(filteredAttributes, flatNewAttributes);
                            foreach (var newAttribute in flatNewAttributes.OrderBy(a => a.GetLevel()))
                            {
                                if (newAttribute.SchemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX) continue;
                                var path = newAttribute.FullPath;
                                var schemaAttr = newAttribute.SchemaAttribute;
                                // Update existing attributes.
                                var existingAttributes = filteredAttributes.Where(a => a.FullPath == path);
                                foreach (var attr in existingAttributes)
                                {
                                    var clone = (SCIMRepresentationAttribute)newAttribute.Clone();
                                    clone.Id = attr.Id;
                                    clone.ParentAttributeId = attr.ParentAttributeId;
                                    clone.RepresentationId = representation.Id;
                                    result.Add(new SCIMPatchResult { Attr = clone, Operation = SCIMPatchOperations.REPLACE, Path = patch.Path });
                                }

                                // Orphan parent.
                                var orphanParents = hierarchicalFilteredAttributes.Where(a => a.FullPath == emptyParent.FullPath && !a.CachedChildren.Any(c => c.FullPath == path));
                                foreach(var orphanParent in orphanParents)
                                {
                                    var clone = (SCIMRepresentationAttribute)newAttribute.Clone();
                                    clone.Id = Guid.NewGuid().ToString();
                                    clone.ParentAttributeId = orphanParent.Id;
                                    clone.RepresentationId = representation.Id;
                                    result.Add(new SCIMPatchResult { Attr = clone, Operation = SCIMPatchOperations.ADD, Path = patch.Path });
                                }
                            }

                                // var isAttributeExits = !string.IsNullOrWhiteSpace(fullPath) && filteredAttributes.Any(a => a.FullPath == fullPath);
                                // var parentExists = representation.FlatAttributes.Any(fa => fa.SchemaAttributeId == emptyParent?.SchemaAttribute?.Id);
                                //if (newAttributes.Any(a => a.GetLevel() == 1) || filteredAttributes.Any() /* || parentExists*/)
                                /*
                                    foreach (var newAttribute in newAttributes.OrderBy(a => a.GetLevel()))
                                    {
                                        var path = newAttribute.FullPath;
                                        var schemaAttr = newAttribute.SchemaAttribute;
                                        foreach(var attr in filteredAttributes)
                                        {
                                            var clone = (SCIMRepresentationAttribute)newAttribute.Clone();
                                            clone.Id = Guid.NewGuid().ToString();
                                            clone.ParentAttributeId = attr.ParentAttributeId;
                                            clone.RepresentationId = representation.Id;
                                            result.Add(new SCIMPatchResult { Attr = clone, Operation = SCIMPatchOperations.ADD, Path = patch.Path });
                                        }

                                        /*
                                        IEnumerable<SCIMRepresentationAttribute> parentAttributes = null;
                                        if (newAttribute.GetLevel() > 1 && fullPath == null)
                                            parentAttributes = representation.GetAttributesByPath(newAttribute.GetParentFullPath()).ToList();
                                        if (fullPath == path)
                                        {
                                            var attr = filteredAttributes.FirstOrDefault(a => a.FullPath == fullPath);
                                            if (attr != null)
                                            {
                                                var parent = representation.GetParentAttribute(attr);
                                                if (parent != null)
                                                {
                                                    parentAttributes = new[] { parent };
                                                }
                                            }
                                            else
                                            {
                                                parentAttributes = representation.GetParentAttributesByPath(fullPath).ToList();
                                            }
                                        }

                                        if (schemaAttr.MultiValued && schemaAttr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                                        {
                                            var filteredAttribute = filteredAttributes.FirstOrDefault(_ => _.FullPath == path);
                                            if (filteredAttribute != null)
                                            {
                                                newAttribute.AttributeId = filteredAttribute.AttributeId;
                                            }

                                            representation.AddAttribute(newAttribute);
                                        }
                                        else if (parentAttributes != null && parentAttributes.Any())
                                        {
                                            foreach (var parentAttribute in parentAttributes)
                                            {
                                                // representation.RefreshHierarchicalAttributesCache();
                                                // var attr = parentAttribute.CachedChildren.FirstOrDefault(c => c.SchemaAttributeId == newAttribute.SchemaAttributeId);
                                                // if (attr != null) representation.FlatAttributes.Remove(attr);
                                                // representation.AddAttribute(parentAttribute, newAttribute);
                                            }
                                        }
                                        else
                                        {
                                            // representation.FlatAttributes.Add(newAttribute);
                                        }

                                        result.Add(new SCIMPatchResult { Attr = newAttribute, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                                    }
                                else
                                {
                                    var flatAttrs = emptyParent.ToFlat();
                                    foreach (var newAttribute in newAttributes)
                                    {
                                        var attrsToRemove = flatAttrs.Where(a => a.SchemaAttribute.Id == newAttribute.SchemaAttribute.Id).ToList();
                                        foreach (var attrToRemove in attrsToRemove)
                                        {
                                            var parentAttr = flatAttrs.First(a => a.Id == attrToRemove.ParentAttributeId);
                                            parentAttr.Children.Remove(attrToRemove);
                                            parentAttr.Children.Add(newAttribute);
                                            newAttribute.ParentAttributeId = parentAttr.Id;
                                        }

                                        flatAttrs = emptyParent.ToFlat();
                                        foreach (var attr in flatAttrs)
                                        {
                                            attr.RepresentationId = representation.Id;
                                            representation.AddAttribute(attr);
                                        }

                                        result.Add(new SCIMPatchResult { Attr = emptyParent, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                                    }
                                }
                                */
                            }
                        break;
                }
            }

            return result;
        }

        private void Add(List<SCIMPatchResult> result, SCIMRepresentationAttribute attribute)
        {

        }

        private static bool TryGetExternalId(PatchOperationParameter patchOperation, out string externalId)
        {
            externalId = null;
            if (patchOperation.Value == null)
            {
                return false;
            }

            var jObj = patchOperation.Value as JObject;
            if (patchOperation.Path == StandardSCIMRepresentationAttributes.ExternalId &&
                (patchOperation.Value.GetType() == typeof(string) || patchOperation.Value.GetType() == typeof(JValue)))
            {
                externalId = patchOperation.Value.ToString();
                return true;
            }

            if (jObj != null && jObj.ContainsKey(StandardSCIMRepresentationAttributes.ExternalId))
            {
                externalId = jObj[StandardSCIMRepresentationAttributes.ExternalId].ToString();
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

        private static ICollection<SCIMRepresentationAttribute> RemoveStandardReferenceProperties(IEnumerable<SCIMRepresentationAttribute> newFlatAttributes, IEnumerable<SCIMAttributeMapping> attributeMappings)
        {
            return newFlatAttributes.Where((nfa) =>
            {
                var parentAttr = newFlatAttributes.FirstOrDefault(a => a.Id == nfa.ParentAttributeId);
                if (parentAttr == null || !attributeMappings.Any(am => am.SourceAttributeId == parentAttr.SchemaAttributeId)) return true;
                if (nfa.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Type || nfa.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Display) return false;
                return true;
            }).ToList();
        }

        private static ICollection<SCIMRepresentationAttribute> FilterDuplicate(IEnumerable<SCIMRepresentationAttribute> existingAttributes, IEnumerable<SCIMRepresentationAttribute> newFlatAttributes)
        {
            var result = new List<SCIMRepresentationAttribute>();
            var rootAttributes = SCIMRepresentation.BuildHierarchicalAttributes(newFlatAttributes);
            foreach (var newAttribute in rootAttributes)
            {
                if (existingAttributes.Any(a => a.IsSimilar(newAttribute, true))) continue;
                result.Add(newAttribute);
            }

            return SCIMRepresentation.BuildFlatAttributes(result);
        }
    }
}
