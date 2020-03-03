// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.EF.Extensions
{
    public static class MappingExtensions
    {
        public static SCIMRepresentationAttributeModel ToModel(this SCIMRepresentationAttribute a)
        {
            var result = new SCIMRepresentationAttributeModel
            {
                Id = a.Id,
                ValuesBoolean = a.ValuesBoolean.AsQueryable(),
                ValuesDateTime = a.ValuesDateTime.AsQueryable(),
                ValuesInteger = a.ValuesInteger.AsQueryable(),
                ValuesString = a.ValuesString.AsQueryable(),
                ValuesReference = a.ValuesReference.AsQueryable(),
                ParentId = a.Parent == null ? null : a.Parent.Id,
                SchemaAttributeId = a.SchemaAttribute.Id,
                Values = new List<SCIMRepresentationAttributeModel>()
            };

            if (a.Values != null && a.Values.Any())
            {
                foreach (var r in a.Values)
                {
                    result.Values.Add(ToModel(r));
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
                Attributes = schema.Attributes.Select(s => ToModel(s)).ToList()
            };
            return result;
        }

        public static SCIMSchemaAttributeModel ToModel(this SCIMSchemaAttribute schemaAttribute)
        {
            var result = new SCIMSchemaAttributeModel
            {
                Id = schemaAttribute.Id,
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
                Uniqueness = schemaAttribute.Uniqueness,
                SubAttributes = new List<SCIMSchemaAttributeModel>()
            };

            if (schemaAttribute.SubAttributes.Any())
            {
                foreach (var subAttr in schemaAttribute.SubAttributes.ToList())
                {
                    result.SubAttributes.Add(ToModel(subAttr));
                }
            }

            return result;
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
                ValuesBoolean = representationAttribute.ValuesBoolean.ToList(),
                ValuesDateTime = representationAttribute.ValuesDateTime.ToList(),
                ValuesInteger = representationAttribute.ValuesInteger.ToList(),
                ValuesReference = representationAttribute.ValuesReference.ToList(),
                ValuesString = representationAttribute.ValuesString.ToList(),
                Values = new List<SCIMRepresentationAttribute>()
            };
            if (representationAttribute.Values != null && representationAttribute.Values.Any())
            {
                foreach(var val in representationAttribute.Values)
                {
                    result.Values.Add(val.ToDomain(result));
                }
            }


            return result;
        }
    }
}
