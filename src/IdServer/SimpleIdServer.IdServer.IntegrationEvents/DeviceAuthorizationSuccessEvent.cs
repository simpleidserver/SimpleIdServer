// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class DeviceAuthorizationSuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(DeviceAuthorizationSuccessEvent);
    public string ClientId { get; set; }
    public string Realm { get; set; }
    public string RequestJSON { get; set; }
}
