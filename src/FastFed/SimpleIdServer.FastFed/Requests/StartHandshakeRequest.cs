// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.FastFed.Requests;

public class StartHandshakeRequest
{
    [FromQuery(Name = "app_metadata_uri")]
    public string AppMetadataUri { get; set; } = null!;
    [FromQuery(Name = "expiration")]
    public double? Expiration { get; set; } = null;

    public string ToQueryParameters()
    {
        var dic = new Dictionary<string, string>
        {
            { "app_metadata_uri", AppMetadataUri }
        };
        if (Expiration != null)
            dic.Add("expiration", Expiration.Value.ToString());
        return string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}
