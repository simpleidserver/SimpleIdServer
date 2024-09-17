// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class UpdateUserSuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(UpdateUserSuccessEvent);
    public string Realm{ get; set; }
    public string Id { get; set; }
}
