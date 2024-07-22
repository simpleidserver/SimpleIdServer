// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;

namespace SimpleIdServer.IdServer.Federation.Apis.FederationEntity;

public class AddTrustedAnchorRequest
{
    [JsonProperty("url")]
    public string Url { get; set; } = null!;
}
