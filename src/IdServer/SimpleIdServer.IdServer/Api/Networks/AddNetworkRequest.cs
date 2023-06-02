// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Org.BouncyCastle.Asn1.Crmf;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Networks
{
    public class AddNetworkRequest
    {
        [JsonPropertyName(NetworkConfigurationNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(NetworkConfigurationNames.RpcUrl)]
        public string RpcUrl { get; set; } = null!;
        [JsonPropertyName(NetworkConfigurationNames.PrivateAccountKey)]
        public string PrivateAccountKey { get; set; } = null!;
    }
}
