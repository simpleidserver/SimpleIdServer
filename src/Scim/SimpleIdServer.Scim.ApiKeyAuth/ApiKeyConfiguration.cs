// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.Scim.ApiKeyAuth;


public class ApiKeyConfiguration
{
    public string Owner { get; set; }
    public string Value { get; set; }
    public List<string> Scopes { get; set; } = new List<string>();
}
