// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Models.DTOs;
using System.Text.Json.Serialization;
using System;

namespace SimpleIdServer.Did.Models
{
    public class NetworkConfiguration
    {
        [JsonPropertyName(NetworkConfigurationNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(NetworkConfigurationNames.RpcUrl)]
        public string RpcUrl { get; set; } = null!;
        [JsonPropertyName(NetworkConfigurationNames.PrivateAccountKey)]
        public string PrivateAccountKey { get; set; } = null!;
        [JsonPropertyName(NetworkConfigurationNames.ContractAdr)]
        public string? ContractAdr { get; set; } = null;
        [JsonPropertyName(NetworkConfigurationNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(NetworkConfigurationNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
    }
}
