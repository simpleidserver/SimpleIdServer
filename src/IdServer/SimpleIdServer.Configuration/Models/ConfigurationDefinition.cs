// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Configuration.Models;

public class ConfigurationDefinition
{
    public string Id { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string FullQualifiedName { get; set; } = null!;
    public List<ConfigurationDefinitionRecord> Records { get; set; } = new List<ConfigurationDefinitionRecord>();
}
