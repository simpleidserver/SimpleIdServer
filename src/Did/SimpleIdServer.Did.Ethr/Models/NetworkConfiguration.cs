// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Did.Ethr.Models;

public class NetworkConfiguration
{
    public string Name { get; set; } = null!;
    public string RpcUrl { get; set; } = null!;
    public string ContractAdr { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
}
