// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Startup.EthereumDID
{
    public record DIDIdentifier
    {
        public DIDIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public DIDIdentifier(string identifier, string source) : this(identifier)
        {
            Source = source;
        }

        public string Identifier { get; set; } = null!;
        public string? Source { get; set; } = null;
    }
}
