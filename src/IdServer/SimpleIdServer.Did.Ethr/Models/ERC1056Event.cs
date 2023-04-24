// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Numerics;

namespace SimpleIdServer.Did.Ethr
{
    public record ERC1056Event
    {
        public string Identity { get; set; }
        public string Value { get; set; }
        public BigInteger ValidTo { get; set; }
        public BigInteger PreviousChange { get; set; }
        public BigInteger BlockNumber { get; set; }
        public string EventName { get; set; } = "DIDAttributeChanged";
    }
}