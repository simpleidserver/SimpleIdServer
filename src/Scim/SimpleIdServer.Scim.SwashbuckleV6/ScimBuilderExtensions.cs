// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Scim.Swashbuckle;
using SimpleIdServer.Scim.SwashbuckleV6;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.Scim;

public static class ScimBuilderExtensions
{
    public static ScimBuilder EnableSwagger(this ScimBuilder scimBuilder)
    {
        scimBuilder.ServiceCollection.AddSwaggerGen(c =>
        {
            c.SchemaFilter<EnumDocumentFilter>();
            var currentAssembly = Assembly.GetExecutingAssembly();
            var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new [] { currentAssembly.GetName() })
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                .Where(f => File.Exists(f)).ToArray();
            Array.ForEach(xmlDocs, (d) =>
            {
                c.IncludeXmlComments(d);
            });
        });
        scimBuilder.ServiceCollection.RemoveAll<IApiDescriptionGroupCollectionProvider>();
        scimBuilder.ServiceCollection.AddSingleton<IApiDescriptionGroupCollectionProvider, ScimApiDescriptionGroupCollectionProvider>();
        scimBuilder.ServiceCollection.AddTransient<ISchemaGenerator, SCIMSchemaGenerator>();
        return scimBuilder;
    }
}
