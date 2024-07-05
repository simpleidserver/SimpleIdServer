// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OpenidFederation.Domains;

public class FederationEntity
{
    public string Id { get; set; }
    public string Sub { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public bool IsSubordinate { get; set; } = false;
}