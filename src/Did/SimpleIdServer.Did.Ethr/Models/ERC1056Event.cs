// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Numerics;

namespace SimpleIdServer.Did.Ethr.Models;

public record ERC1056Event
{
    public string Identity { get; set; }
    public string Value { get; set; }
    public string Owner { get; set; }
    public string Delegate { get; set; }
    public string DelegateType { get; set; }
    public BigInteger ValidTo { get; set; }
    public BigInteger PreviousChange { get; set; }
    public BigInteger BlockNumber { get; set; }
    public EventTypes Type { get; set; } = EventTypes.DIDAttributeChanged;
}

public enum EventTypes
{
    DIDAttributeChanged = 0,
    DIDOwnerChanged = 1,
    DIDDelegateChanged = 2
}