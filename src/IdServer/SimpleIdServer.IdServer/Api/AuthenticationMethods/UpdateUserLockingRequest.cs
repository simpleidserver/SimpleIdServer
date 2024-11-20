﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.AuthenticationMethods;

public class UpdateUserLockingRequest
{
    [JsonPropertyName(AuthenticationMethodNames.Values)]
    public Dictionary<string, string> Values { get; set; }
}
