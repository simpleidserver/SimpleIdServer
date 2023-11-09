// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Clients;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.ExternalEvents;

public class ClientUpdatedSuccessEvent : IExternalEvent
{
    public string EventName => nameof(ClientUpdatedSuccessEvent);
    public string Realm { get; set; }
    public string Id { get; set; }
    public UpdateClientRequest Client { get; set; }
}
