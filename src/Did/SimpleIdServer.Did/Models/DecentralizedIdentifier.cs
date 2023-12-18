// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Did.Models;

public class DecentralizedIdentifier
{
    public DecentralizedIdentifier(string scheme, string method, string identifier)
    {
        Scheme = scheme;
        Method = method;
        Identifier = identifier;
    }

    public string Scheme { get; set; }
    public string Method { get; set; }
    public string Identifier { get; set; }
}