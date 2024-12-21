﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class ClientUpdatedFailureEvent : IIntegrationEvent
{
    public string EventName => nameof(ClientUpdatedFailureEvent);
    public string Realm { get; set; }
    public string Id { get; set; }
    public string SerializedRequest { get; set; }
}