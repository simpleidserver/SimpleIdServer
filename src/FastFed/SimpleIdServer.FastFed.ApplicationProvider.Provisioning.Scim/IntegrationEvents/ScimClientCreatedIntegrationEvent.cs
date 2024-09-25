// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim.IntegrationEvents;

public class ScimClientCreatedIntegrationEvent
{
    public string EntityId { get; set; }
    public string Scope { get; set; }
}
