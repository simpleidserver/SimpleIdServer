// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Ethr.Models;

public class DecentralizedIdentifierEthr : DecentralizedIdentifier
{
    public DecentralizedIdentifierEthr(string scheme, string method, string identifier) : base(scheme, method, identifier)
    {
    }

    public string Network { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PublicKey { get; set; } = null;
    public int? Version { get; set; } = null;

    public string Format()
    {
        var result = $"{Scheme}:{Method}";
        if (!string.IsNullOrWhiteSpace(Network)) result = $"{result}:{Network}";
        return $"{result}:{Identifier}";
    }
}
