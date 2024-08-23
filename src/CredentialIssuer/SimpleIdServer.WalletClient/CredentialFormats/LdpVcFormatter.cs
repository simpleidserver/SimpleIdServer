// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Vc.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.WalletClient.CredentialFormats;

public class LdpVcFormatter : ICredentialFormatter
{
    public string Format => "ldp_vc";

    public W3CVerifiableCredential Extract(string content)
        => JsonSerializer.Deserialize<W3CVerifiableCredential>(content);

    public DeserializedCredential DeserializeObject(string serializedVc)
    {
        return new DeserializedCredential(JsonObject.Parse(serializedVc), JObject.Parse(serializedVc), null);
    }
}
