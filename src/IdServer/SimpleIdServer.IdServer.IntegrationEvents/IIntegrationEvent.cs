// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;

public interface IIntegrationEvent
{
    string EventName { get; }
    string Realm { get; set; }
}