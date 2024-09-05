// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.FastFed.ApplicationProvider;

public class FastFedApplicationProviderOptions
{
    public TimeSpan WhitelistingExpirationTime { get; set; } = TimeSpan.FromSeconds(60 * 5);
}
