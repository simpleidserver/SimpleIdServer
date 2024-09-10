// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;
using SimpleIdServer.Webfinger.Client;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;

public class DiscoverProvidersViewModel
{
    public string Email { get; set; }
    public string Href { get; set; }
    public GetWebfingerResult? WebfingerResult { get; set; } = null;
    public ProviderMetadata ProviderMetadata { get; set; } = null;
    public DiscoverProviderActions? Action { get; set; } = null;
}

public enum DiscoverProviderActions
{
    CONFIRMEMAIL = 0,
    SELECTPROVIDER = 1,
    CONFIRMPROVIDER = 2
}
