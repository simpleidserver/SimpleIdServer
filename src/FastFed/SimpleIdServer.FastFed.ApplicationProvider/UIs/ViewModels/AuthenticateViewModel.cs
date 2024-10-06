// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;

public class AuthenticateViewModel
{
    public List<ExternalIdProvider> ExternalIdProviders { get; set; } = new List<ExternalIdProvider>();
}

public class ExternalIdProvider
{
    public string AuthenticationScheme {  get; set; }
    public string DisplayName { get; set; }
}