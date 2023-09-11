// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SimpleIdServer.Configuration;

public class ConfigurationDefinitionExtractor
{
    public static ConfigurationDefinition Extract<T>(string language = null)
    {
        var type = typeof(T);
        language ??= Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        var result = new ConfigurationDefinition { Id = type.Name, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, FullQualifiedName = type.FullName };
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach(var property in properties)
            if (TryExtract(property, language, out ConfigurationDefinitionRecord configurationDefinition)) result.Records.Add(configurationDefinition);
        return result;
    }

    private static bool TryExtract(PropertyInfo propertyInfo, string language, out ConfigurationDefinitionRecord result)
    {
        result = null;
        var configurationRecordAttr = propertyInfo.GetCustomAttribute(typeof(ConfigurationRecordAttribute)) as ConfigurationRecordAttribute;
        if (configurationRecordAttr == null) return false;
        result = new ConfigurationDefinitionRecord { Id = Guid.NewGuid().ToString(), CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, Name = propertyInfo.Name };
        result.SetDescription(configurationRecordAttr.Description, language);
        result.SetDisplayName(configurationRecordAttr.DisplayName, language);
        if (configurationRecordAttr.IsProtected)
        {
            result.Type = ConfigurationDefinitionRecordTypes.NUMBER;
            return true;
        }

        if (TryEnrichEnumeration(propertyInfo, result, language)) return true;
        if (TryEnrichList(propertyInfo, result, language)) return true;
        if (TryEnrichBoolean(propertyInfo, result, language)) return true;
        if (TryEnrichNumber(propertyInfo, result, language)) return true;
        if (TryEnrichDateTime(propertyInfo, result, language)) return true;
        return true;
    }

    private static bool TryEnrichEnumeration(PropertyInfo propertyInfo, ConfigurationDefinitionRecord record, string language)
    {
        if (!propertyInfo.PropertyType.IsEnum) return false;
        EnrichEnumeration(propertyInfo.PropertyType, record, language);
        record.Type = ConfigurationDefinitionRecordTypes.SELECT;
        return true;
    }

    private static bool TryEnrichList(PropertyInfo propertyInfo, ConfigurationDefinitionRecord record, string language)
    {
        if (!propertyInfo.PropertyType.IsGenericType || !(propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) return false;
        var genericTypeDef = propertyInfo.PropertyType.GetGenericArguments().Single();
        EnrichEnumeration(genericTypeDef, record, language);
        record.Type = ConfigurationDefinitionRecordTypes.MULTISELECT;
        return true;
    }

    private static bool TryEnrichBoolean(PropertyInfo propertyInfo, ConfigurationDefinitionRecord record, string language)
    {
        if (propertyInfo.PropertyType != typeof(bool)) return false;
        record.Type = ConfigurationDefinitionRecordTypes.CHECKBOX;
        return true;
    }

    private static bool TryEnrichNumber(PropertyInfo propertyInfo, ConfigurationDefinitionRecord record, string language)
    {
        if (!IsNumeric(propertyInfo.PropertyType)) return false;
        record.Type = ConfigurationDefinitionRecordTypes.NUMBER;
        return true;
    }

    private static bool TryEnrichDateTime(PropertyInfo propertyInfo, ConfigurationDefinitionRecord record, string language)
    {
        if (propertyInfo.PropertyType != typeof(DateTime)) return false;
        record.Type = ConfigurationDefinitionRecordTypes.DATETIME;
        return true;
    }


    private static void EnrichEnumeration(Type enumType, ConfigurationDefinitionRecord record, string language)
    {
        var inst = Activator.CreateInstance(enumType);
        var fields = enumType.GetFields();
        foreach (var field in fields)
        {
            var enumAttr = field.GetCustomAttribute<ConfigurationRecordEnumAttribute>();
            if (enumAttr == null) continue;
            var name = enumAttr.Description;
            var value = (int)field.GetValue(inst);
            record.SetValue(name, language, value.ToString());
        }
    }

    private static bool IsNumeric(Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }
}
