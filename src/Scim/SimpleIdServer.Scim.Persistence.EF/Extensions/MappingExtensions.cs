// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.EF.Extensions
{
    public static class MappingExtensions
    {
        public static SCIMRepresentationAttributeModel ToModel(this SCIMRepresentationAttribute a, string representationId)
        {
            var values = new List<SCIMRepresentationAttributeValueModel>();
            values.AddRange(a.ValuesBoolean.Select(b => new SCIMRepresentationAttributeValueModel
            {
                ValueBoolean = b,
                Id = Guid.NewGuid().ToString()
            }));
            values.AddRange(a.ValuesDateTime.Select(d => new SCIMRepresentationAttributeValueModel
            {
                ValueDateTime = d,
                Id = Guid.NewGuid().ToString()
            }));
            values.AddRange(a.ValuesInteger.Select(i => new SCIMRepresentationAttributeValueModel
            {
                ValueInteger = i,
                Id = Guid.NewGuid().ToString()
            }));
            values.AddRange(a.ValuesReference.Select(r => new SCIMRepresentationAttributeValueModel
            {
                ValueReference = r,
                Id = Guid.NewGuid().ToString()
            }));
            values.AddRange(a.ValuesString.Select(s => new SCIMRepresentationAttributeValueModel
            {
                ValueString = s,
                Id = Guid.NewGuid().ToString()
            }));
            var result = new SCIMRepresentationAttributeModel
            {
                Id = a.Id,
                Values = values,
                RepresentationId = representationId,
                ParentId = a.Parent == null ? null : a.Parent.Id,
                SchemaAttributeId = a.SchemaAttribute.Id,
                Children = new List<SCIMRepresentationAttributeModel>()
            };

            if (a.Values != null && a.Values.Any())
            {
                foreach (var r in a.Values)
                {
                    result.Children.Add(ToModel(r, representationId));
                }
            }

            return result;
        }

        public static SCIMSchemaModel ToModel(this SCIMSchema schema)
        {
            var result = new SCIMSchemaModel
            {
                Description = schema.Description,
                Id = schema.Id,
                IsRootSchema = schema.IsRootSchema,
                Name = schema.Name,
                ResourceType = schema.ResourceType,
                SchemaExtensions = schema.SchemaExtensions.Select(s => new SCIMSchemaExtensionModel
                {
                    Id = s.Id,
                    Required = s.Required,
                    Schema = s.Schema
                }).ToList(),
                Attributes = schema.Attributes.Select(s => ToModel(s, schema.Id)).ToList()
            };
            return result;
        }

        public static SCIMAttributeMappingModel ToModel(this SCIMAttributeMapping attributeMapping)
        {
            return new SCIMAttributeMappingModel
            {
                Id = attributeMapping.Id,
                SourceAttributeSelector = attributeMapping.SourceAttributeSelector,
                SourceResourceType = attributeMapping.SourceResourceType,
                TargetAttributeId = attributeMapping.TargetAttributeId,
                TargetResourceType = attributeMapping.TargetResourceType
            };
        }

        public static SCIMSchemaAttributeModel ToModel(this SCIMSchemaAttribute schemaAttribute, string schemaId)
        {
            var result = new SCIMSchemaAttributeModel
            {
                Id = schemaAttribute.Id,
                SchemaId = schemaId,
                CanonicalValues = schemaAttribute.CanonicalValues == null ? new List<string>() : schemaAttribute.CanonicalValues.ToList(),
                CaseExact = schemaAttribute.CaseExact,
                DefaultValueInt = schemaAttribute.DefaultValueInt.ToList(),
                DefaultValueString = schemaAttribute.DefaultValueString.ToList(),
                Description = schemaAttribute.Description,
                MultiValued = schemaAttribute.MultiValued,
                Mutability = schemaAttribute.Mutability,
                Name = schemaAttribute.Name,
                ReferenceTypes = schemaAttribute.ReferenceTypes.ToList(),
                Required = schemaAttribute.Required,
                Returned = schemaAttribute.Returned,
                Type = schemaAttribute.Type,
                Uniqueness = schemaAttribute.Uniqueness,
                SubAttributes = new List<SCIMSchemaAttributeModel>()
            };

            if (schemaAttribute.SubAttributes.Any())
            {
                foreach (var subAttr in schemaAttribute.SubAttributes.ToList())
                {
                    result.SubAttributes.Add(ToModel(subAttr, schemaId));
                }
            }

            return result;
        }

        public static SCIMAttributeMapping ToDomain(this SCIMAttributeMappingModel attribute)
        {
            return new SCIMAttributeMapping
            {
                Id = attribute.Id,
                SourceAttributeSelector = attribute.SourceAttributeSelector,
                SourceResourceType = attribute.SourceResourceType,
                TargetAttributeId = attribute.TargetAttributeId,
                TargetResourceType = attribute.TargetResourceType
            };
        }


        public static SCIMRepresentation ToDomain(this SCIMRepresentationModel representation) 
        {
            var result = new SCIMRepresentation
            {
                Created = representation.Created,
                ExternalId = representation.ExternalId,
                LastModified = representation.LastModified,
                Version = representation.Version,
                ResourceType = representation.ResourceType,
                Id = representation.Id,
                Schemas = representation.Schemas.Select(s => ToDomain(s.Schema)).ToList(),
                Attributes = representation.Attributes.Select(s =>
                {
                    return ToDomain(s);
                }).ToList()                
            };
            return result;
        }

        public static SCIMSchema ToDomain(this SCIMSchemaModel schema) 
        {
            var result = new SCIMSchema
            {
                Id = schema.Id,
                Description = schema.Description,
                IsRootSchema = schema.IsRootSchema,
                Name = schema.Name,
                ResourceType = schema.ResourceType,
                Attributes = schema.Attributes.Select(a => ToDomain(a)).ToList(),
                SchemaExtensions = schema.SchemaExtensions.Select(s => new SCIMSchemaExtension
                {
                    Id = s.Id,
                    Required = s.Required,
                    Schema = s.Schema
                }).ToList()
            };
            return result;
        }

        public static SCIMSchemaAttribute ToDomain(this SCIMSchemaAttributeModel schemaAttribute)
        {
            var result = new SCIMSchemaAttribute(schemaAttribute.Id)
            {
                CanonicalValues = schemaAttribute.CanonicalValues.ToList(),
                CaseExact = schemaAttribute.CaseExact,
                DefaultValueInt = schemaAttribute.DefaultValueInt.ToList(),
                DefaultValueString = schemaAttribute.DefaultValueString.ToList(),
                Description = schemaAttribute.Description,
                MultiValued = schemaAttribute.MultiValued,
                Mutability = schemaAttribute.Mutability,
                Name = schemaAttribute.Name,
                ReferenceTypes = schemaAttribute.ReferenceTypes.ToList(),
                Required = schemaAttribute.Required,
                Returned = schemaAttribute.Returned,
                Type = schemaAttribute.Type,
                Uniqueness = schemaAttribute.Uniqueness
            };

            if (schemaAttribute.SubAttributes != null && schemaAttribute.SubAttributes.Any())
            {
                foreach(var subAttr in schemaAttribute.SubAttributes.ToList())
                {
                    result.AddSubAttribute(ToDomain(subAttr));
                }
            }

            return result;
        }

        public static SCIMRepresentationAttribute ToDomain(this SCIMRepresentationAttributeModel representationAttribute, SCIMRepresentationAttribute parent = null)
        {
            var result = new SCIMRepresentationAttribute
            {
                Id = representationAttribute.Id,
                Parent = parent == null ? (representationAttribute.Parent == null ? null : representationAttribute.Parent.ToDomain()) : parent,
                SchemaAttribute = representationAttribute.SchemaAttribute.ToDomain(),
                ValuesBoolean = representationAttribute.SchemaAttribute.Type != SCIMSchemaAttributeTypes.BOOLEAN ? new List<bool>() : representationAttribute.Values.Select(v => v.ValueBoolean.Value).ToList(),
                ValuesDateTime = representationAttribute.SchemaAttribute.Type != SCIMSchemaAttributeTypes.DATETIME ? new List<DateTime>() : representationAttribute.Values.Select(v => v.ValueDateTime.Value).ToList(),
                ValuesInteger = representationAttribute.SchemaAttribute.Type != SCIMSchemaAttributeTypes.INTEGER ? new List<int>() : representationAttribute.Values.Select(v => v.ValueInteger.Value).ToList(),
                ValuesReference = representationAttribute.SchemaAttribute.Type != SCIMSchemaAttributeTypes.REFERENCE ? new List<string>() : representationAttribute.Values.Select(v => v.ValueReference).ToList(),
                ValuesString = representationAttribute.SchemaAttribute.Type != SCIMSchemaAttributeTypes.STRING ? new List<string>() : representationAttribute.Values.Select(v => v.ValueString).ToList(),
                Values = new List<SCIMRepresentationAttribute>()
            };
            if (representationAttribute.Children != null && representationAttribute.Children.Any())
            {
                foreach(var val in representationAttribute.Children)
                {
                    result.Values.Add(val.ToDomain(result));
                }
            }


            return result;
        }
    }
}
