// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.IdServer.Swagger;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddSwagger(this IdServerBuilder builder, Action<IdServerSwaggerApiConfiguration> action = null)
    {
        builder.Services.AddSwaggerGen(o =>
        {
            o.SchemaFilter<DescribeEnumMemberValues>();
            var conf = new IdServerSwaggerApiConfiguration(o);
            conf.AddOAuthSecurity();
            if(action != null)
            {
                action(conf);
            }
        });
        builder.Services.RemoveAll<IApiDescriptionGroupCollectionProvider>();
        builder.Services.AddSingleton<IApiDescriptionGroupCollectionProvider, SidApiDescriptionGroupCollectionProvider>();
        return builder;
    }
}
