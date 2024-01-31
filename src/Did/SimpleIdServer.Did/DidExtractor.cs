// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using System;
using System.Linq;

namespace SimpleIdServer.Did;

public class DidExtractor
{
    public static DecentralizedIdentifier Extract(string did)
    {
        if (string.IsNullOrWhiteSpace(did)) throw new ArgumentNullException(nameof(did));
        var splitted = did.Split(':');
        if (splitted.Count() != 3) throw new ArgumentException("did doesn't have the correct format");
        if (splitted.First() != Constants.Scheme) throw new ArgumentException($"scheme must be equal {Constants.Scheme}");
        var identifier = splitted[2];
        var fragment = string.Empty;
        if(identifier.Contains("#"))
        {
            var parts = identifier.Split("#");
            identifier = parts[0];
            fragment = parts[1];
        }
        return new DecentralizedIdentifier(Constants.Scheme, splitted[1], identifier, fragment);
    }
}
