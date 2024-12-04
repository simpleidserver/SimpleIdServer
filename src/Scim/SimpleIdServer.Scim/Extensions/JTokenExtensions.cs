// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.Extensions;

public static class JTokenExtensions
{
    public static bool IsEmpty(this JToken token)
        => token == null || string.IsNullOrWhiteSpace(token.ToString());
}
