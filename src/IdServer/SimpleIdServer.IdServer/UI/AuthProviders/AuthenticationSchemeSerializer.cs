// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.IdServer.UI.AuthProviders
{
    public class AuthenticationSchemeSerializer
    {
        public static IEnumerable<AuthenticationSchemeProviderDefinitionProperty> SerializePropertyDefinitions(Type optionsType)
        {
            return optionsType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p =>
            {
                var attr = p.GetCustomAttribute<VisibleAuthSchemeAttribute>();
                return attr != null;
            }).Select(p =>
            {
                var attr = p.GetCustomAttribute<VisibleAuthSchemeAttribute>();
                return new AuthenticationSchemeProviderDefinitionProperty
                {
                    PropertyName = p.Name,
                    Description = attr.Description,
                    DisplayName = attr.DisplayName
                };
            });
        }

        public static IEnumerable<AuthenticationSchemeProviderProperty> SerializeProperties(object options)
        {
            return options.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p =>
            {
                var attr = p.GetCustomAttribute<VisibleAuthSchemeAttribute>();
                var val = p.GetValue(options)?.ToString();
                return attr != null;
            }).Select(p =>
            {
                var attr = p.GetCustomAttribute<VisibleAuthSchemeAttribute>();
                return new AuthenticationSchemeProviderProperty
                {
                    PropertyName = p.Name,
                    Value = p.GetValue(options)?.ToString()
                };
            });
        }

        public static object DeserializeOptions(Type type, IEnumerable<AuthenticationSchemeProviderProperty> properties)
        {
            var visibleProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p =>
            {
                var attr = p.GetCustomAttribute<VisibleAuthSchemeAttribute>();
                return attr != null && properties.Any(pr => pr.PropertyName == p.Name);
            });
            var result = Activator.CreateInstance(type);
            foreach(var visibleProperty in visibleProperties)
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
