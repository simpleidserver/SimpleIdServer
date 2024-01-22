// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Ethr;

/// <summary>
/// Format is defined here : https://github.com/ChainAgnostic/CAIPs/blob/main/CAIPs/caip-10.md
/// </summary>
public class CAIP10BlockChainAccount
{
    public string Namespace { get; set; }
    public string? Reference { get; set; }
    public string AccountAddress { get; set; }

    public static CAIP10BlockChainAccount BuildEthereumMainet(string accountAddress)
    {
        return new CAIP10BlockChainAccount
        {
            Namespace = "eip155",
            Reference = "1",
            AccountAddress = accountAddress
        };
    }

    public override string ToString()
    {
        var result = Namespace;
        if (!string.IsNullOrWhiteSpace(Reference)) result = $"{Namespace}:{Reference}";
        return $"{result}:{AccountAddress}";
    }
}
