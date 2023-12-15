// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did
{
    public interface IIdentityDocumentIdentifierParser
    {
        string Type { get; }
        IdentityDocumentIdentifier Parse(string id);
    }
}
