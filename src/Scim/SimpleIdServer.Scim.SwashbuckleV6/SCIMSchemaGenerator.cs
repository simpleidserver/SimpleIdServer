// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SimpleIdServer.Scim.Api;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Persistence;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DataType = Swashbuckle.AspNetCore.SwaggerGen.DataType;

namespace SimpleIdServer.Scim.Swashbuckle
{
    public class SCIMSchemaGenerator : ISchemaGenerator
    {
        private static Dictionary<SCIMSchemaAttributeTypes, string> MAPPING_ENUM_TO_NAMES = new Dictionary<SCIMSchemaAttributeTypes, string>
        {
            { SCIMSchemaAttributeTypes.BINARY, "binary" },
            { SCIMSchemaAttributeTypes.BOOLEAN, "boolean" },
            { SCIMSchemaAttributeTypes.COMPLEX, "object" },
            { SCIMSchemaAttributeTypes.DATETIME, "string" },
            { SCIMSchemaAttributeTypes.DECIMAL, "number" },
            { SCIMSchemaAttributeTypes.INTEGER, "integer" },
            { SCIMSchemaAttributeTypes.REFERENCE, "string" },
            { SCIMSchemaAttributeTypes.STRING, "string" }
        };
        private static Dictionary<SCIMSchemaAttributeTypes, string> MAPPING_ENUM_TO_FORMAT = new Dictionary<SCIMSchemaAttributeTypes, string>
        {
            { SCIMSchemaAttributeTypes.DATETIME, "date-time" }
        };
        private readonly SchemaGeneratorOptions _generatorOptions;
        private readonly ISerializerDataContractResolver _serializerDataContractResolver;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ILogger<SCIMSchemaGenerator> _logger;

        public SCIMSchemaGenerator(SchemaGeneratorOptions generatorOptions, ISerializerDataContractResolver serializerDataContractResolver, IServiceProvider serviceProvider, ILogger<SCIMSchemaGenerator> logger)
        {
            _generatorOptions = generatorOptions;
            _serializerDataContractResolver = serializerDataContractResolver;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _scimSchemaQueryRepository = serviceProvider.CreateScope().ServiceProvider.GetService<ISCIMSchemaQueryRepository>();
        }

        public OpenApiSchema GenerateSchema(
            Type modelType,
            SchemaRepository schemaRepository,
            MemberInfo memberInfo = null,
            ParameterInfo parameterInfo = null,
            ApiParameterRouteInfo routeInfo = null)
        {
            if (memberInfo != null)
                return GenerateSchemaForMember(modelType, schemaRepository, memberInfo, parameterInfo);

            if (parameterInfo != null)
                return GenerateSchemaForParameter(modelType, schemaRepository, parameterInfo, routeInfo);

            return GenerateSchemaForType(modelType, schemaRepository, parameterInfo);
        }

        private OpenApiSchema GenerateSchemaForMember(
            Type modelType,
            SchemaRepository schemaRepository,
            MemberInfo memberInfo,
            ParameterInfo parameterInfo,
            DataProperty dataProperty = null)
        {
            var dataContract = GetDataContractFor(modelType);

            var schema = _generatorOptions.UseOneOfForPolymorphism && IsBaseTypeWithKnownTypesDefined(dataContract, parameterInfo, out var knownTypesDataContracts)
                ? GeneratePolymorphicSchema(dataContract, schemaRepository, knownTypesDataContracts, parameterInfo)
                : GenerateConcreteSchema(dataContract, schemaRepository, parameterInfo);

            if (_generatorOptions.UseAllOfToExtendReferenceSchemas && schema.Reference != null)
            {
                schema.AllOf = new[] { new OpenApiSchema { Reference = schema.Reference } };
                schema.Reference = null;
            }

            if (schema.Reference == null)
            {
                var customAttributes = memberInfo.GetInlineAndMetadataAttributes();

                // Nullable, ReadOnly & WriteOnly are only relevant for Schema "properties" (i.e. where dataProperty is non-null)
                if (dataProperty != null)
                {
                    schema.Nullable = _generatorOptions.SupportNonNullableReferenceTypes
                        ? dataProperty.IsNullable && !customAttributes.OfType<RequiredAttribute>().Any() && !memberInfo.IsNonNullableReferenceType()
                        : dataProperty.IsNullable && !customAttributes.OfType<RequiredAttribute>().Any();

                    schema.ReadOnly = dataProperty.IsReadOnly;
                    schema.WriteOnly = dataProperty.IsWriteOnly;
                }

                var defaultValueAttribute = customAttributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                if (defaultValueAttribute != null)
                {
                    var defaultAsJson = dataContract.JsonConverter(defaultValueAttribute.Value);
                    schema.Default = OpenApiAnyFactory.CreateFromJson(defaultAsJson);
                }

                var obsoleteAttribute = customAttributes.OfType<ObsoleteAttribute>().FirstOrDefault();
                if (obsoleteAttribute != null)
                {
                    schema.Deprecated = true;
                }

                // NullableAttribute behaves diffrently for Dictionaries
                if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    schema.AdditionalProperties.Nullable = !memberInfo.IsDictionaryValueNonNullable();
                }

                schema.ApplyValidationAttributes(customAttributes);

                ApplyFilters(schema, modelType, schemaRepository, memberInfo: memberInfo);
            }

            return schema;
        }

        private OpenApiSchema GenerateSchemaForParameter(
            Type modelType,
            SchemaRepository schemaRepository,
            ParameterInfo parameterInfo,
            ApiParameterRouteInfo routeInfo)
        {
            var dataContract = GetDataContractFor(modelType);

            var schema = _generatorOptions.UseOneOfForPolymorphism && IsBaseTypeWithKnownTypesDefined(dataContract, parameterInfo, out var knownTypesDataContracts)
                ? GeneratePolymorphicSchema(dataContract, schemaRepository, knownTypesDataContracts, parameterInfo)
                : GenerateConcreteSchema(dataContract, schemaRepository, parameterInfo);

            if (_generatorOptions.UseAllOfToExtendReferenceSchemas && schema.Reference != null)
            {
                schema.AllOf = new[] { new OpenApiSchema { Reference = schema.Reference } };
                schema.Reference = null;
            }

            if (schema.Reference == null)
            {
                var customAttributes = parameterInfo.GetCustomAttributes();

                var defaultValue = parameterInfo.HasDefaultValue
                    ? parameterInfo.DefaultValue
                    : customAttributes.OfType<DefaultValueAttribute>().FirstOrDefault()?.Value;

                if (defaultValue != null)
                {
                    var defaultAsJson = dataContract.JsonConverter(defaultValue);
                    schema.Default = OpenApiAnyFactory.CreateFromJson(defaultAsJson);
                }

                schema.ApplyValidationAttributes(customAttributes);
                if (routeInfo != null)
                {
                    schema.ApplyRouteConstraints(routeInfo);
                }

                ApplyFilters(schema, modelType, schemaRepository, parameterInfo: parameterInfo);
            }

            return schema;
        }

        private OpenApiSchema GenerateSchemaForType(Type modelType, SchemaRepository schemaRepository, ParameterInfo parameterInfo)
        {
            var dataContract = GetDataContractFor(modelType);

            var schema = _generatorOptions.UseOneOfForPolymorphism && IsBaseTypeWithKnownTypesDefined(dataContract, parameterInfo, out var knownTypesDataContracts)
                ? GeneratePolymorphicSchema(dataContract, schemaRepository, knownTypesDataContracts, parameterInfo)
                : GenerateConcreteSchema(dataContract, schemaRepository, parameterInfo);

            if (schema.Reference == null)
            {
                ApplyFilters(schema, modelType, schemaRepository);
            }

            return schema;
        }

        private DataContract GetDataContractFor(Type modelType)
        {
            var effectiveType = Nullable.GetUnderlyingType(modelType) ?? modelType;
            return _serializerDataContractResolver.GetDataContractForType(effectiveType);
        }

        private bool IsBaseTypeWithKnownTypesDefined(DataContract dataContract, ParameterInfo parameterInfo, out IEnumerable<DataContract> knownTypesDataContracts)
        {
            knownTypesDataContracts = null;

            if (dataContract.DataType != DataType.Object) return false;

            var subTypes = _generatorOptions.SubTypesSelector(dataContract.UnderlyingType);

            if (!subTypes.Any()) return false;

            var knownTypes = !dataContract.UnderlyingType.IsAbstract
                ? new[] { dataContract.UnderlyingType }.Union(subTypes)
                : subTypes;

            knownTypesDataContracts = knownTypes.Select(knownType => GetDataContractFor(knownType));
            return true;
        }

        private OpenApiSchema GeneratePolymorphicSchema(
            DataContract dataContract,
            SchemaRepository schemaRepository,
            IEnumerable<DataContract> knownTypesDataContracts,
            ParameterInfo parameterInfo)
        {
            return new OpenApiSchema
            {
                OneOf = knownTypesDataContracts
                    .Select(allowedTypeDataContract => GenerateConcreteSchema(allowedTypeDataContract, schemaRepository, parameterInfo))
                    .ToList()
            };
        }

        private OpenApiSchema GenerateConcreteSchema(DataContract dataContract, SchemaRepository schemaRepository, ParameterInfo parameterInfo)
        {
            if (TryGetCustomTypeMapping(dataContract.UnderlyingType, out Func<OpenApiSchema> customSchemaFactory))
            {
                return customSchemaFactory();
            }

            if (dataContract.UnderlyingType.IsAssignableToOneOf(typeof(IFormFile), typeof(FileResult)))
            {
                return new OpenApiSchema { Type = "string", Format = "binary" };
            }

            Func<OpenApiSchema> schemaFactory;
            bool returnAsReference;

            switch (dataContract.DataType)
            {
                case DataType.Boolean:
                case DataType.Integer:
                case DataType.Number:
                case DataType.String:
                    {
                        schemaFactory = () => CreatePrimitiveSchema(dataContract);
                        returnAsReference = dataContract.UnderlyingType.IsEnum && !_generatorOptions.UseInlineDefinitionsForEnums;
                        break;
                    }

                case DataType.Array:
                    {
                        schemaFactory = () => CreateArraySchema(dataContract, schemaRepository);
                        returnAsReference = dataContract.UnderlyingType == dataContract.ArrayItemType;
                        break;
                    }

                case DataType.Dictionary:
                    {
                        schemaFactory = () => CreateDictionarySchema(dataContract, schemaRepository);
                        returnAsReference = dataContract.UnderlyingType == dataContract.DictionaryValueType;
                        break;
                    }

                case DataType.Object:
                    {
                        schemaFactory = () => CreateObjectSchema(dataContract, schemaRepository, parameterInfo);
                        returnAsReference = true;
                        break;
                    }

                default:
                    {
                        schemaFactory = () => new OpenApiSchema();
                        returnAsReference = false;
                        break;
                    }
            }

            return returnAsReference
                ? GenerateReferencedSchema(dataContract, schemaRepository, schemaFactory, parameterInfo)
                : schemaFactory();
        }

        private bool TryGetCustomTypeMapping(Type modelType, out Func<OpenApiSchema> schemaFactory)
        {
            return _generatorOptions.CustomTypeMappings.TryGetValue(modelType, out schemaFactory)
                || (modelType.IsConstructedGenericType && _generatorOptions.CustomTypeMappings.TryGetValue(modelType.GetGenericTypeDefinition(), out schemaFactory));
        }

        private OpenApiSchema CreatePrimitiveSchema(DataContract dataContract)
        {
            var schema = new OpenApiSchema
            {
                Type = dataContract.DataType.ToString().ToLower(CultureInfo.InvariantCulture),
                Format = dataContract.DataFormat
            };

            // For backcompat only - EnumValues is obsolete
            if (dataContract.EnumValues != null)
            {
                schema.Enum = dataContract.EnumValues
                    .Select(value => JsonSerializer.Serialize(value))
                    .Distinct()
                    .Select(valueAsJson => OpenApiAnyFactory.CreateFromJson(valueAsJson))
                    .ToList();

                return schema;
            }

            if (dataContract.UnderlyingType.IsEnum)
            {
                schema.Enum = dataContract.UnderlyingType.GetEnumValues()
                    .Cast<object>()
                    .Select(value => dataContract.JsonConverter(value))
                    .Distinct()
                    .Select(valueAsJson => OpenApiAnyFactory.CreateFromJson(valueAsJson))
                    .ToList();
            }

            return schema;
        }

        private OpenApiSchema CreateArraySchema(DataContract dataContract, SchemaRepository schemaRepository)
        {
            var hasUniqueItems = dataContract.UnderlyingType.IsConstructedFrom(typeof(ISet<>), out _)
                || dataContract.UnderlyingType.IsConstructedFrom(typeof(KeyedCollection<,>), out _);

            return new OpenApiSchema
            {
                Type = "array",
                Items = GenerateSchema(dataContract.ArrayItemType, schemaRepository),
                UniqueItems = hasUniqueItems ? (bool?)true : null
            };
        }

        private OpenApiSchema CreateDictionarySchema(DataContract dataContract, SchemaRepository schemaRepository)
        {
            if (dataContract.DictionaryKeys != null)
            {
                // This is a special case where the set of key values is known (e.g. if the key type is an enum)
                return new OpenApiSchema
                {
                    Type = "object",
                    Properties = dataContract.DictionaryKeys.ToDictionary(
                        name => name,
                        name => GenerateSchema(dataContract.DictionaryValueType, schemaRepository)),
                    AdditionalPropertiesAllowed = false,
                };
            }
            else
            {
                return new OpenApiSchema
                {
                    Type = "object",
                    AdditionalPropertiesAllowed = true,
                    AdditionalProperties = GenerateSchema(dataContract.DictionaryValueType, schemaRepository)
                };
            }
        }

        private OpenApiSchema CreateObjectSchema(DataContract dataContract, SchemaRepository schemaRepository, ParameterInfo parameterInfo)
        {
            var schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>(),
                Required = new SortedSet<string>(),
                AdditionalPropertiesAllowed = false
            };

            var applicableDataProperties = dataContract.ObjectProperties;

            if (_generatorOptions.UseAllOfForInheritance || _generatorOptions.UseOneOfForPolymorphism)
            {
                if (IsKnownSubType(dataContract, out var baseTypeDataContract))
                {
                    var baseTypeSchema = GenerateConcreteSchema(baseTypeDataContract, schemaRepository, parameterInfo);

                    schema.AllOf.Add(baseTypeSchema);

                    applicableDataProperties = applicableDataProperties
                        .Where(dataProperty => dataProperty.MemberInfo.DeclaringType == dataContract.UnderlyingType);
                }

                if (IsBaseTypeWithKnownTypesDefined(dataContract, parameterInfo, out var knownTypesDataContracts))
                {
                    foreach (var knownTypeDataContract in knownTypesDataContracts)
                    {
                        // Ensure schema is generated for all known types
                        GenerateConcreteSchema(knownTypeDataContract, schemaRepository, parameterInfo);
                    }

                    if (TryGetDiscriminatorFor(dataContract, schemaRepository, knownTypesDataContracts, parameterInfo, out var discriminator))
                    {
                        schema.Properties.Add(discriminator.PropertyName, new OpenApiSchema { Type = "string" });
                        schema.Required.Add(discriminator.PropertyName);
                        schema.Discriminator = discriminator;
                    }
                }
            }

            foreach (var dataProperty in applicableDataProperties)
            {
                var customAttributes = dataProperty.MemberInfo?.GetInlineAndMetadataAttributes() ?? Enumerable.Empty<object>();

                if (_generatorOptions.IgnoreObsoleteProperties && customAttributes.OfType<ObsoleteAttribute>().Any())
                    continue;

                schema.Properties[dataProperty.Name] = (dataProperty.MemberInfo != null)
                    ? GenerateSchemaForMember(dataProperty.MemberType, schemaRepository, dataProperty.MemberInfo, parameterInfo, dataProperty)
                    : GenerateSchemaForType(dataProperty.MemberType, schemaRepository, parameterInfo);

                if ((dataProperty.IsRequired || customAttributes.OfType<RequiredAttribute>().Any())
                    && !schema.Required.Contains(dataProperty.Name))
                {
                    schema.Required.Add(dataProperty.Name);
                }
            }

            if (dataContract.ObjectExtensionDataType != null)
            {
                schema.AdditionalPropertiesAllowed = true;
                schema.AdditionalProperties = GenerateSchema(dataContract.ObjectExtensionDataType, schemaRepository);
            }

            return schema;
        }

        private bool IsKnownSubType(DataContract dataContract, out DataContract baseTypeDataContract)
        {
            baseTypeDataContract = null;

            var baseType = dataContract.UnderlyingType.BaseType;

            if (baseType == null || baseType == typeof(object) || !_generatorOptions.SubTypesSelector(baseType).Contains(dataContract.UnderlyingType))
                return false;

            baseTypeDataContract = GetDataContractFor(baseType);
            return true;
        }

        private bool TryGetDiscriminatorFor(
            DataContract dataContract,
            SchemaRepository schemaRepository,
            IEnumerable<DataContract> knownTypesDataContracts,
            ParameterInfo parameterInfo,
            out OpenApiDiscriminator discriminator)
        {
            discriminator = null;

            var discriminatorName = _generatorOptions.DiscriminatorNameSelector(dataContract.UnderlyingType)
                ?? dataContract.ObjectTypeNameProperty;

            if (discriminatorName == null) return false;

            discriminator = new OpenApiDiscriminator
            {
                PropertyName = discriminatorName
            };

            foreach (var knownTypeDataContract in knownTypesDataContracts)
            {
                var discriminatorValue = _generatorOptions.DiscriminatorValueSelector(knownTypeDataContract.UnderlyingType)
                    ?? knownTypeDataContract.ObjectTypeNameValue;

                if (discriminatorValue == null) continue;

                discriminator.Mapping.Add(discriminatorValue, GenerateConcreteSchema(knownTypeDataContract, schemaRepository, parameterInfo).Reference.ReferenceV3);
            }

            return true;
        }

        private OpenApiSchema GenerateReferencedSchema(
            DataContract dataContract,
            SchemaRepository schemaRepository,
            Func<OpenApiSchema> definitionFactory,
            ParameterInfo parameterInfo)
        {
            var dataContractType = dataContract.UnderlyingType;
            var baseControllerType = typeof(BaseApiController);
            var baseParameterType = typeof(RepresentationParameter);
            var schemaId = _generatorOptions.SchemaIdSelector(dataContract.UnderlyingType);
            Type controllerType = null;
            if (baseParameterType.IsAssignableFrom(dataContract.UnderlyingType))
            {
                if (baseControllerType.IsAssignableFrom(parameterInfo.Member.ReflectedType))
                {
                    schemaId = $"{parameterInfo.Member.ReflectedType.Name}{schemaId}";
                    controllerType = parameterInfo.Member.ReflectedType;
                    dataContractType = controllerType;
                }
            }

            if (schemaRepository.TryLookupByType(dataContractType, out OpenApiSchema referenceSchema))
                return referenceSchema;

            schemaRepository.RegisterType(dataContractType, schemaId);

            var schema = definitionFactory();
            ApplyFilters(schema, dataContractType, schemaRepository);
            if (controllerType != null)
            {
                var controller = (BaseApiController)_serviceProvider.CreateScope().ServiceProvider.GetService(controllerType);
                var scimSchema = _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(controller.ResourceType).Result;
                if (scimSchema != null)
                {
                    Enrich(scimSchema, scimSchema.HierarchicalAttributes.Select(a => a.Leaf).ToList(), schema.Properties);
                    var schemaDef = schema.Properties.First(p => p.Key == "schemas").Value.Items;
                    schemaDef.Example = new OpenApiString(scimSchema.Id);
                }
                else
                {
                    _logger.LogError($"the schema '{controller.ResourceType}' doesn't exist !");
                }

                var kvp = schema.Properties.FirstOrDefault(_ => _.Key == "attributes");
                if (!kvp.Equals(default(KeyValuePair<string, OpenApiSchema>)) && !string.IsNullOrWhiteSpace(kvp.Key))
                {
                    schema.Properties.Remove(kvp);
                }
            }

            return schemaRepository.AddDefinition(schemaId, schema);
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

        private static void Enrich(SCIMSchema schema, ICollection<SCIMSchemaAttribute> attributes, IDictionary<string, OpenApiSchema> properties)
        {
            foreach (var attr in attributes.Where(a => a.Required))
            {
                var sc = new OpenApiSchema
                {
                    Description = attr.Description,
                    Type = attr.MultiValued ? "array" : MAPPING_ENUM_TO_NAMES[attr.Type],
                    Properties = new Dictionary<string, OpenApiSchema>()
                };
                if (!attr.MultiValued && MAPPING_ENUM_TO_FORMAT.ContainsKey(attr.Type))
                {
                    sc.Format = MAPPING_ENUM_TO_FORMAT[attr.Type];
                }

                properties.Add(new KeyValuePair<string, OpenApiSchema>(attr.Name, sc));
                if (attr.MultiValued && attr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                {
                    sc.Items = new OpenApiSchema
                    {
                        Type = MAPPING_ENUM_TO_NAMES[attr.Type]
                    };
                    if (MAPPING_ENUM_TO_FORMAT.ContainsKey(attr.Type))
                    {
                        sc.Format = MAPPING_ENUM_TO_FORMAT[attr.Type];
                    }

                    continue;
                }

                var values = schema.GetChildren(attr).ToList();
                if (attr.MultiValued && attr.Type == SCIMSchemaAttributeTypes.COMPLEX)
                {
                    sc.Items = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>()
                    };
                    Enrich(schema, values, sc.Items.Properties);
                    continue;
                }

                if (attr.Type == SCIMSchemaAttributeTypes.COMPLEX && !attr.MultiValued)
                {
                    Enrich(schema, values, sc.Properties);
                }
            }
        }
    }
}
