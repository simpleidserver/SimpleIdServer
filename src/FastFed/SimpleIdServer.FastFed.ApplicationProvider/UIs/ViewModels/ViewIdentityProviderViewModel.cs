// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Models;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;

public class ViewIdentityProviderViewModel
{
    public string EntityId {  get; set; }
    public IdentityProviderStatus Status { get; set; }
    public List<string> ProvisioningProfiles { get; set; }
}
