// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Models
{
    public record IdentityDocumentIdentifier
    {
        public IdentityDocumentIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public IdentityDocumentIdentifier(string identifier, string source) : this(identifier)
        {
            Source = source;
        }

        public string Identifier { get; set; } = null!;
        public string Source { get; set; } = null;
        public string Address { get; set; } = null!;
        public string PublicKey { get; set; } = null;
    }
}
