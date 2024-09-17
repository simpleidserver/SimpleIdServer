// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.IntegrationEvents;
public class AddClientEncryptionKeySuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(AddClientEncryptionKeySuccessEvent);
    public string Realm { get; set; }
    public string KeyId { get; set; }
    public SecurityKeyTypes KeyType { get; set; }
    public string SerializedJsonWebKey { get; set; }
    public string Alg { get; set; }
    public string ClientId { get; set; }
}
