// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Scim.SqlServer.Startup.Services
{
    public class ApiKeysConfiguration
    {
        public ICollection<ApiKeyConfiguration> ApiKeys { get; set; } = new List<ApiKeyConfiguration>();
    }

    public class ApiKeyConfiguration
    {
        public string Owner { get; set; }
        public string Value { get; set; }
        public ICollection<string> Scopes { get; set; } = new List<string>();
    }
}
