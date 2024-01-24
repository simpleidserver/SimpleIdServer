// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer
{
    public class ShareCredentialTemplateRequest
    {
        [JsonPropertyName(ShareCredentialTemplateNames.CredentialTemplateId)]
        public string CredentialTemplateId { get; set; } = null!;
        [JsonPropertyName(ShareCredentialTemplateNames.WalletClientId)]
        public string WalletClientId { get; set; }
    }
}
