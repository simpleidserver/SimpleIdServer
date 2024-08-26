// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.WalletClient.CredentialFormats;

public interface ICredentialFormatter
{
    string Format { get; }
    W3CVerifiableCredential Extract(string content);
    DeserializedCredential DeserializeObject(string serializedVc);
}

public record DeserializedCredential
{
    public DeserializedCredential(object vpObject, JObject jsonPayload, JObject jsonHeader)
    {
        VpObject = vpObject;
        JsonPayload = jsonPayload;
        JsonHeader = jsonHeader;
    }

    public object VpObject { get; private set; }
    public JObject JsonPayload { get; private set; }
    public JObject JsonHeader { get; private set; }
}
