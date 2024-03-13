// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains;

public class PresentationDefinitionFormat
{
    public string Id { get; set; }
    public string Format { get; set; }
    public string ProofType { get; set; }

    public static JsonObject Serialize(List<PresentationDefinitionFormat> format)
    {
        var json = new JsonObject();
        foreach(var f in format)
        {
            json.Add(f.Format, new JsonObject
            {
                { "proof_type", new JsonArray(f.ProofType) }
            });
        }

        return json;
    }
}
