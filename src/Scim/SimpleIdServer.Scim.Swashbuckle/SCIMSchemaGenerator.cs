// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SimpleIdServer.Scim.Api;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Persistence;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.Scim.Swashbuckle
{
    public class SCIMSchemaGenerator : ISchemaGenerator
    {
        private static Dictionary<SCIMSchemaAttributeTypes, string> MAPPING_ENUM_TO_NAMES = new Dictionary<SCIMSchemaAttributeTypes, string>
        {
            { SCIMSchemaAttributeTypes.BINARY, "binary" },
            { SCIMSchemaAttributeTypes.BOOLEAN, "boolean" },
            { SCIMSchemaAttributeTypes.COMPLEX, "object" },
            { SCIMSchemaAttributeTypes.DATETIME, "date-time" },
            { SCIMSchemaAttributeTypes.DECIMAL, "number" },
            { SCIMSchemaAttributeTypes.INTEGER, "integer" },
            { SCIMSchemaAttributeTypes.REFERENCE, "string" },
            { SCIMSchemaAttributeTypes.STRING, "string" }
        };
        private readonly ILogger<SCIMSchemaGenerator> _logger;
        private readonly SchemaGeneratorOptions _generatorOptions;
        private readonly IDataContractResolver _dataContractResolver;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;

        public SCIMSchemaGenerator(ILogger<SCIMSchemaGenerator> logger, IServiceProvider serviceProvider, ISCIMSchemaQueryRepository scimSchemaQueryRepository, SchemaGeneratorOptions generatorOptions, IDataContractResolver dataContractResolver)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _generatorOptions = generatorOptions;
            _dataContractResolver = dataContractResolver;
        }

        public OpenApiSchema GenerateSchema(
            Type type,
            SchemaRepository schemaRepository,
            MemberInfo memberInfo = null,
            ParameterInfo parameterInfo = null)
        {
            var schema = GenerateSchemaForType(type, parameterInfo, schemaRepository);

            if (memberInfo != null)
            {
                ApplyMemberMetadata(schema, type, memberInfo);
            }
            else if (parameterInfo != null)
            {
                ApplyParameterMetadata(schema, type, parameterInfo);
            }

            if (schema.Reference == null)
            {
                ApplyFilters(schema, type, schemaRepository, memberInfo, parameterInfo);
            }

            return schema;
        }

        private OpenApiSchema GenerateSchemaForType(Type type, ParameterInfo parameterInfo, SchemaRepository schemaRepository)
        {
            if (TryGetCustomMapping(type, out var mapping))
            {
                return mapping();
            }

            if (type.IsAssignableToOneOf(typeof(IFormFile), typeof(FileResult)))
            {
                return new OpenApiSchema { Type = "string", Format = "binary" };
            }

            if (_generatorOptions.GeneratePolymorphicSchemas)
            {
                var knownSubTypes = _generatorOptions.SubTypesResolver(type);
                if (knownSubTypes.Any())
                {
                    return GeneratePolymorphicSchema(knownSubTypes, schemaRepository);
                }
            }

            var dataContract = _dataContractResolver.GetDataContractForType(type);

            var shouldBeReferenced =
                // regular object
                (dataContract.DataType == DataType.Object && dataContract.Properties != null && !dataContract.UnderlyingType.IsDictionary()) ||
                // dictionary-based AND self-referencing
                (dataContract.DataType == DataType.Object && dataContract.AdditionalPropertiesType == dataContract.UnderlyingType) ||
                // array-based AND self-referencing
                (dataContract.DataType == DataType.Array && dataContract.ArrayItemType == dataContract.UnderlyingType) ||
                // enum-based AND opted-out of inline
                (dataContract.EnumValues != null && !_generatorOptions.UseInlineDefinitionsForEnums);

            return (shouldBeReferenced)
                ? GenerateReferencedSchema(dataContract, parameterInfo, schemaRepository)
                : GenerateInlineSchema(dataContract, parameterInfo, schemaRepository);
        }

        private bool TryGetCustomMapping(Type type, out Func<OpenApiSchema> mapping)
        {
            if (_generatorOptions.CustomTypeMappings.TryGetValue(type, out mapping))
            {
                return true;
            }

            if (type.IsGenericType && !type.IsGenericTypeDefinition &&
                _generatorOptions.CustomTypeMappings.TryGetValue(type.GetGenericTypeDefinition(), out mapping))
            {
                return true;
            }

            return false;
        }

        private OpenApiSchema GeneratePolymorphicSchema(IEnumerable<Type> knownSubTypes, SchemaRepository schemaRepository)
        {
            return new OpenApiSchema
            {
                OneOf = knownSubTypes
                    .Select(subType => GenerateSchema(subType, schemaRepository))
                    .ToList()
            };
        }

        private OpenApiSchema GenerateReferencedSchema(DataContract dataContract, ParameterInfo parameterInfo, SchemaRepository schemaRepository)
        {
            var baseControllerType = typeof(BaseApiController);
            var baseParameterType = typeof(RepresentationParameter);
            string schemaId = dataContract.UnderlyingType.Name;
            Type controllerType = null;
            if (parameterInfo != null && baseParameterType.IsAssignableFrom(parameterInfo.ParameterType))
            {
                if (baseControllerType.IsAssignableFrom(parameterInfo.Member.ReflectedType))
                {
                    schemaId = $"{parameterInfo.Member.ReflectedType.Name}{schemaId}";
                    controllerType = parameterInfo.Member.ReflectedType;
                }
            }

            if (schemaRepository.Schemas.ContainsKey(schemaId))
            {
                return new OpenApiSchema
                {
                    Reference = new OpenApiReference { Id = schemaId, Type = ReferenceType.Schema }
                };
            }

            schemaRepository.Schemas.Add(schemaId, null);
            var schema = GenerateInlineSchema(dataContract, parameterInfo, schemaRepository);
            ApplyFilters(schema, dataContract.UnderlyingType, schemaRepository);
            if (controllerType != null)
            {
                var controller = (BaseApiController)_serviceProvider.GetService(controllerType);
                var scimSchema = _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(controller.ResourceType).Result;
                if (scimSchema != null)
                {
                    Enrich(scimSchema.Attributes, schema.Properties);
                }
                else
                {
                    _logger.LogError($"the schema '{controller.ResourceType}' doesn't exist !");
                }

                schema.Properties.Remove(schema.Properties.First(_ => _.Key == "Attributes"));
            }

            schemaRepository.Schemas[schemaId] = schema;
            return new OpenApiSchema
            {
                Reference = new OpenApiReference { Id = schemaId, Type = ReferenceType.Schema }
            };
        }

        private static void Enrich(ICollection<SCIMSchemaAttribute> attributes, IDictionary<string, OpenApiSchema> properties)
        {
            foreach (var attr in attributes)
            {
                var sc = new OpenApiSchema
                {
                    Description = attr.Description,
                    Type = attr.MultiValued ? "array" : MAPPING_ENUM_TO_NAMES[attr.Type],
                    Properties = new Dictionary<string, OpenApiSchema>()
                };
                properties.Add(new KeyValuePair<string, OpenApiSchema>(attr.Name, sc));
                if (attr.MultiValued && attr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                {
                    sc.Items = new OpenApiSchema
                    {
                        Type = MAPPING_ENUM_TO_NAMES[attr.Type]
                    };
                    continue;
                }

                if (attr.MultiValued && attr.Type == SCIMSchemaAttributeTypes.COMPLEX)
                {
                    sc.Items = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>()
                    };
                    Enrich(attr.SubAttributes, sc.Items.Properties);
                    continue;
                }

                if (attr.Type == SCIMSchemaAttributeTypes.COMPLEX && !attr.MultiValued)
                {
                    Enrich(attr.SubAttributes, sc.Properties);
                }
            }
        }

        private OpenApiSchema GenerateInlineSchema(DataContract dataContract, ParameterInfo parameterInfo, SchemaRepository schemaRepository)
        {
            if (dataContract.DataType == DataType.Unknown)
                return new OpenApiSchema();

            if (dataContract.DataType == DataType.Object)
                return GenerateObjectSchema(dataContract, parameterInfo, schemaRepository);

            if (dataContract.DataType == DataType.Array)
                return GenerateArraySchema(dataContract, schemaRepository);

            else
                return GeneratePrimitiveSchema(dataContract);
        }

        private OpenApiSchema GenerateObjectSchema(DataContract dataContract, ParameterInfo parameterInfo, SchemaRepository schemaRepository)
        {
            var schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>(),
                Required = new SortedSet<string>(),
                AdditionalPropertiesAllowed = false
            };

            // If it's a baseType with known subTypes, add the discriminator property
            if (_generatorOptions.GeneratePolymorphicSchemas && _generatorOptions.SubTypesResolver(dataContract.UnderlyingType).Any())
            {
                var discriminatorName = _generatorOptions.DiscriminatorSelector(dataContract.UnderlyingType);

                if (!schema.Properties.ContainsKey(discriminatorName))
                    schema.Properties.Add(discriminatorName, new OpenApiSchema { Type = "string" });

                schema.Required.Add(discriminatorName);
                schema.Discriminator = new OpenApiDiscriminator { PropertyName = discriminatorName };
            }

            foreach (var dataProperty in dataContract.Properties ?? Enumerable.Empty<DataProperty>())
            {
                var customAttributes = dataProperty.MemberInfo?.GetInlineOrMetadataTypeAttributes() ?? Enumerable.Empty<object>();

                if (_generatorOptions.IgnoreObsoleteProperties && customAttributes.OfType<ObsoleteAttribute>().Any())
                    continue;

                schema.Properties[dataProperty.Name] = GeneratePropertySchema(dataProperty, parameterInfo, schemaRepository);

                if (dataProperty.IsRequired)
                    schema.Required.Add(dataProperty.Name);
            }

            if (dataContract.AdditionalPropertiesType != null)
            {
                schema.AdditionalPropertiesAllowed = true;
                schema.AdditionalProperties = GenerateSchema(dataContract.AdditionalPropertiesType, schemaRepository);
            }

            // If it's a known subType, reference the baseType for inheritied properties
            if (
                _generatorOptions.GeneratePolymorphicSchemas &&
                (dataContract.UnderlyingType.BaseType != null) &&
                _generatorOptions.SubTypesResolver(dataContract.UnderlyingType.BaseType).Contains(dataContract.UnderlyingType))
            {
                var basedataContract = _dataContractResolver.GetDataContractForType(dataContract.UnderlyingType.BaseType);
                var baseSchemaReference = GenerateReferencedSchema(basedataContract, parameterInfo, schemaRepository);

                var baseSchema = schemaRepository.Schemas[baseSchemaReference.Reference.Id];
                foreach (var basePropertyName in baseSchema.Properties.Keys)
                {
                    schema.Properties.Remove(basePropertyName);
                }

                return new OpenApiSchema
                {
                    AllOf = new List<OpenApiSchema> { baseSchemaReference, schema }
                };
            }

            return schema;
        }

        private OpenApiSchema GeneratePropertySchema(DataProperty serializerMember, ParameterInfo parameterInfo, SchemaRepository schemaRepository)
        {
            var schema = GenerateSchemaForType(serializerMember.MemberType, parameterInfo, schemaRepository);

            if (serializerMember.MemberInfo != null)
            {
                ApplyMemberMetadata(schema, serializerMember.MemberType, serializerMember.MemberInfo);
            }

            if (schema.Reference == null)
            {
                schema.Nullable = serializerMember.IsNullable && schema.Nullable;
                schema.ReadOnly = serializerMember.IsReadOnly;
                schema.WriteOnly = serializerMember.IsWriteOnly;

                ApplyFilters(schema, serializerMember.MemberType, schemaRepository, serializerMember.MemberInfo);
            }

            return schema;
        }

        private OpenApiSchema GenerateArraySchema(DataContract dataContract, SchemaRepository schemaRepository)
        {
            return new OpenApiSchema
            {
                Type = "array",
                Items = GenerateSchema(dataContract.ArrayItemType, schemaRepository),
                UniqueItems = dataContract.UnderlyingType.IsSet() ? (bool?)true : null
            };
        }

        private OpenApiSchema GeneratePrimitiveSchema(DataContract dataContract)
        {
            var schema = new OpenApiSchema
            {
                Type = dataContract.DataType.ToString().ToLower(CultureInfo.InvariantCulture),
                Format = dataContract.Format
            };

            if (dataContract.EnumValues != null)
            {
                schema.Enum = dataContract.EnumValues
                    .Distinct()
                    .Select(value => OpenApiAnyFactory.CreateFor(schema, value))
                    .ToList();
            }

            return schema;
        }

        private void ApplyMemberMetadata(OpenApiSchema schema, Type type, MemberInfo memberInfo)
        {
            if (schema.Reference != null && _generatorOptions.UseAllOfToExtendReferenceSchemas)
            {
                schema.AllOf = new[] { new OpenApiSchema { Reference = schema.Reference } };
                schema.Reference = null;
            }

            if (schema.Reference == null)
            {
                schema.Nullable = type.IsReferenceOrNullableType();

                schema.ApplyCustomAttributes(memberInfo.GetInlineOrMetadataTypeAttributes());
            }
        }

        private void ApplyParameterMetadata(OpenApiSchema schema, Type type, ParameterInfo parameterInfo)
        {
            if (schema.Reference != null && _generatorOptions.UseAllOfToExtendReferenceSchemas)
            {
                schema.AllOf = new[] { new OpenApiSchema { Reference = schema.Reference } };
                schema.Reference = null;
            }

            if (schema.Reference == null)
            {
                schema.Nullable = type.IsReferenceOrNullableType();

                schema.ApplyCustomAttributes(parameterInfo.GetCustomAttributes());

                if (parameterInfo.HasDefaultValue)
                {
                    schema.Default = OpenApiAnyFactory.CreateFor(schema, parameterInfo.DefaultValue);
                }
            }
        }

        private void ApplyFilters(
            OpenApiSchema schema,
            Type type,
            SchemaRepository schemaRepository,
            MemberInfo memberInfo = null,
            ParameterInfo parameterInfo = null)
        {
            var filterContext = new SchemaFilterContext(
                type: type,
                schemaGenerator: this,
                schemaRepository: schemaRepository,
                memberInfo: memberInfo,
                parameterInfo: parameterInfo);

            foreach (var filter in _generatorOptions.SchemaFilters)
            {
                filter.Apply(schema, filterContext);
            }
        }

    }
}
