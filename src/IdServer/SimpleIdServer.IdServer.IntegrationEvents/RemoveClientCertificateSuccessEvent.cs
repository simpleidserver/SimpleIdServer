// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class RemoveClientCertificateSuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(RemoveClientCertificateSuccessEvent);
    public string Realm { get; set; }
    public string CAId { get; set; }
    public string ClientCertificateId { get; set; }
}
