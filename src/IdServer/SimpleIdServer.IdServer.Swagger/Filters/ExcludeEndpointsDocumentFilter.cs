// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleIdServer.IdServer.Swagger.Filters;

public class ExcludeEndpointsDocumentFilter : IDocumentFilter
{
    private readonly ExcludeEndpointsConfig _config;

    public ExcludeEndpointsDocumentFilter(ExcludeEndpointsConfig config)
    {
        _config = config;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (_config.SegmentsToExclude == null || _config.SegmentsToExclude.Length == 0)
        {
            return;
        }

        var pathsToRemove = swaggerDoc.Paths
            .Where(path =>
                _config.SegmentsToExclude.Any(segment =>
                    path.Key.TrimStart('/')
                        .StartsWith(segment.Trim('/'), StringComparison.OrdinalIgnoreCase))
            )
            .Select(p => p.Key)
            .ToList();

        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }
    }
}
