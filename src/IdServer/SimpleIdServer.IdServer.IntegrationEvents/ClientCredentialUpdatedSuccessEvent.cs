// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.IntegrationEvents;
public class ClientCredentialUpdatedSuccessEvent : IIntegrationEvent
{
    public string EventName => nameof(ClientCredentialUpdatedSuccessEvent);
    public string Realm { get; set; }
    public string ClientId { get; set; }
    public string TokenEndpointAuthMethod { get; set; }
    public string ClientSecret { get; set; }
    public string TlsClientAuthSubjectDN { get; set; }
    public string TlsClientAuthSanDNS { get; set; }
    public string TlsClientAuthSanEmail { get; set; }
    public string TlsClientAuthSanIp { get; set; }
}