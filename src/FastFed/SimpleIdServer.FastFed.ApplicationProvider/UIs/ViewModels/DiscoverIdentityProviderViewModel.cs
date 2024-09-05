// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Webfinger.Client;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;

public class DiscoverIdentityProviderViewModel
{
    public GetWebfingerResult WebfingerResult { get; set; }
}
