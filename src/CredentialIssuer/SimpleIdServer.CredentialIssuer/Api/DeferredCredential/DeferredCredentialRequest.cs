// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.DeferredCredential;

public class DeferredCredentialRequest
{
    [JsonPropertyName(CredentialResultNames.TransactionId)]
    public string TransactionId { get; set; }
}
