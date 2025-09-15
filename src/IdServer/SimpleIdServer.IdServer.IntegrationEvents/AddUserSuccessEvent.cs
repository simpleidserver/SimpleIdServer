// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class AddUserSuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(AddUserSuccessEvent);
    public string Realm { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Middlename { get; set; }
    public string? Email { get; set; } = null;
    public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}
