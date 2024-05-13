// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Stores;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Auditing
{
    public class SearchAuditingRequest : SearchRequest
    {
        [JsonPropertyName("only_error")]
        public bool DisplayOnlyErrors { get; set; }
    }
}
