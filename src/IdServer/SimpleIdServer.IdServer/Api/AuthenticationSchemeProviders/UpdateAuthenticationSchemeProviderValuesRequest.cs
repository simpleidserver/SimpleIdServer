// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;

public class UpdateAuthenticationSchemeProviderValuesRequest
{
    public Dictionary<string, string> Values { get; set; }
}
