// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Webfinger.Client;

public class GetWebfingerResult
{
    [JsonPropertyName("subject")]
    public string Subject { get; set; }
    [JsonPropertyName("links")]
    public List<WebfingerLinkResult> Links { get; set; } = new List<WebfingerLinkResult>();
}
