// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class UserLoginSuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(UserLoginSuccessEvent);
    public string UserName { get; set; }
    public string Amr { get; set; }
    public string Realm { get; set; }
}