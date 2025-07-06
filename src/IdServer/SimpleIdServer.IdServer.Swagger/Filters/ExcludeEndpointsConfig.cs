// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Swagger.Filters;

public class ExcludeEndpointsConfig
{
    public string[] SegmentsToExclude { get; set; } = Array.Empty<string>();
}
