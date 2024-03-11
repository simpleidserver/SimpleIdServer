// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc;

public class CredentialSubjectBuilder
{
    private readonly JsonObject _jsonObj;

    public CredentialSubjectBuilder(JsonObject jsonObj)
    {
        _jsonObj = jsonObj;
    }

    public static CredentialSubjectBuilder New(string id = null)
    {
        var jsonObj = new JsonObject();
        if(!string.IsNullOrWhiteSpace(id)) jsonObj.Add("id", id);
        return new CredentialSubjectBuilder(jsonObj);
    }

    public CredentialSubjectBuilder AddClaim(string name, string value)
    {
        _jsonObj.Add(name, value);
        return this;
    }

    public CredentialSubjectBuilder AddClaim(string name, JsonObject value)
    {
        _jsonObj.Add(name, value);
        return this;
    }

    public JsonObject Build() => _jsonObj;
}
