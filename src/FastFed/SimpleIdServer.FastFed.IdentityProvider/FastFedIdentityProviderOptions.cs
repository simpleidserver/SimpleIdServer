// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Globalization;

namespace SimpleIdServer.FastFed.IdentityProvider;

public class FastFedIdentityProviderOptions
{
    public List<CultureInfo> SupportedCultures { get; set; } = new List<CultureInfo>();
    public List<SigningCredentials> SigningCredentials { get; set; } = new List<SigningCredentials>();
}
