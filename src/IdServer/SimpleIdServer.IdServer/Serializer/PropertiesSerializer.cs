// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.IdServer.Serializer
{
    public static class PropertiesSerializer
    {
        public static IEnumerable<T> SerializePropertyDefinitions<T>(Type optionsType) where T : IPropertyDefinition
        {
            return optionsType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p =>
            {
                var attr = p.GetCustomAttribute<VisiblePropertyAttribute>();
                return attr != null;
            }).Select(p =>
            {
                var attr = p.GetCustomAttribute<VisiblePropertyAttribute>();
                var result = (IPropertyDefinition)Activator.CreateInstance<T>();
                result.PropertyName = p.Name;
                result.Description = attr.Description;
                result.DisplayName = attr.DisplayName;
                return (T)result;
            });
        }

        public static IEnumerable<T> SerializeProperties<T>(object options) where T : IPropertyInstance
        {
            return options.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p =>
            {
                var attr = p.GetCustomAttribute<VisiblePropertyAttribute>();
                var val = p.GetValue(options)?.ToString();
                return attr != null;
            }).Select(p =>
            {
                var attr = p.GetCustomAttribute<VisiblePropertyAttribute>();
                var result = (IPropertyInstance)Activator.CreateInstance<T>();
                result.PropertyName = p.Name;
                result.Value = p.GetValue(options)?.ToString();
                return (T)result;
            });
        }

        public static TOpts DeserializeOptions<TOpts, T>(IEnumerable<T> properties) where T : IPropertyInstance => (TOpts)DeserializeOptions(typeof(TOpts), properties);

        public static object DeserializeOptions<T>(Type type, IEnumerable<T> properties) where T : IPropertyInstance
        {
            var visibleProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p =>
            {
                var attr = p.GetCustomAttribute<VisiblePropertyAttribute>();
                return attr != null && properties.Any(pr => pr.PropertyName == p.Name);
            });
            var result = Activator.CreateInstance(type);
            foreach (var visibleProperty in visibleProperties)
            {
                var prop = properties.Single(p => p.PropertyName == visibleProperty.Name);
                if (string.IsNullOrWhiteSpace(prop.Value)) continue;
                var propValue = TypeDescriptor.GetConverter(visibleProperty.PropertyType);
                visibleProperty.SetValue(result, propValue.ConvertFromInvariantString(prop.Value));
            }

            return result;
        }
    }
}
