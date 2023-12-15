// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.Did;

public class ContextBuilder
{
    private JsonObject _jObj;

    internal ContextBuilder()
    {
        _jObj = new JsonObject();
    }

    public ContextBuilder AddPropertyValue(string shortName, string fullPath)
    {
        _jObj.Add(shortName, fullPath);
        return this;
    }

    public ContextBuilder AddPropertyId(string shortName, string fullPath)
    {
        _jObj.Add(shortName, new JsonObject
        {
            { "@id", fullPath },
            { "@type", "@id" }
        });
        return this;
    }

    internal JsonObject Build() => _jObj;
}
