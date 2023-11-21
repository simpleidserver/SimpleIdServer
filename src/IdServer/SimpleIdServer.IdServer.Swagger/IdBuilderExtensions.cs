// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SimpleIdServer.IdServer.Swagger;

public static class IdBuilderExtensions
{
    public static IdServerBuilder AddSwagger(this IdServerBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.RemoveAll<IApiDescriptionGroupCollectionProvider>();
        builder.Services.AddSingleton<IApiDescriptionGroupCollectionProvider, SidApiDescriptionGroupCollectionProvider>();
        return builder;
    }
}
