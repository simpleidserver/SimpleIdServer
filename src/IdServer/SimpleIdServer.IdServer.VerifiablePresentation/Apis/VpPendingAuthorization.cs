// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis;

public class VpPendingAuthorization
{
    public VpPendingAuthorization(string presentationDefinitionId, string state, string nonce)
    {
        PresentationDefinitionId = presentationDefinitionId;
        State = state;
        Nonce = nonce;
    }

    public string PresentationDefinitionId { get; set; }
    public string State { get; set; }
    public string Nonce { get; set; }
    public bool IsAuthorized { get; set; }
    public Dictionary<string, JsonNode> VcSubjects { get; set; }
}