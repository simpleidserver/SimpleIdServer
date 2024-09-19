// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Models;
using System;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs;

public class ApplicationProviderViewModel
{
    public string EntityId { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
    public IdentityProviderStatus? Status { get; set; }
    public double? ExpirationTime { get; set; }
}