// Licensed under the Apache License, Version 2.0.
// See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Startup.EthereumDID
{
    public class DIDDocumentVertificationMethod
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("controller")]
        public string Controller { get; set; }
        [JsonPropertyName("blockchainAccountId")]
        public string BlockChainAccountId { get; set; }
    }
}
