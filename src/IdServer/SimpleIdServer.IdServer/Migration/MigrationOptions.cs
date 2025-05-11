// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Migration;

public class MigrationOptions
{
    [ConfigurationRecord("Page size", null, order: 0)]
    public int PageSize { get; set; } = 1000;
}
