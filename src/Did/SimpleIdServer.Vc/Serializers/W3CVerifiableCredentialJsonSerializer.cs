// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc.CredentialFormats.Serializers;

public class W3CVerifiableCredentialJsonSerializer
{
    public string Serialize(W3CVerifiableCredential verifiableCredential)
    {
        if (verifiableCredential == null) throw new ArgumentNullException(nameof(verifiableCredential));
        var json = JsonSerializer.Serialize(verifiableCredential);
        return json;
    }

    public Dictionary<string, object> SerializeToDic(W3CVerifiableCredential verifiableCredential)
    {
        if (verifiableCredential == null) throw new ArgumentNullException(nameof(verifiableCredential));
        var jObj = JsonObject.Parse(JsonSerializer.Serialize(verifiableCredential)).AsObject();
        return jObj.Serialize() as Dictionary<string, object>;
    }

    public W3CVerifiableCredential Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        return JsonSerializer.Deserialize<W3CVerifiableCredential>(json);
    }
}
