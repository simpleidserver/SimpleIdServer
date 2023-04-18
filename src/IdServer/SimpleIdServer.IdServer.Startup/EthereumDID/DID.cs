// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Startup.EthereumDID
{
    public class DID
    {
        [JsonPropertyName("@context")]
        public IEnumerable<string> Context { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("vertificationMethod")]
        public ICollection<DIDDocumentVertificationMethod> VertificationMethods { get; set; }
    }
}
