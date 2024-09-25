// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.Webfinger.Client;

public class WebfingerLinkResult
{
    [JsonPropertyName("rel")]
    public string Rel { get; set; }
    [JsonPropertyName("href")]
    public string Href { get; set; }
}
