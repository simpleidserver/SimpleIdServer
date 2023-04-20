// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did
{
    public interface IIdentityDocumentExtractor
    {
        Task<IdentityDocument> Extract(string id, CancellationToken cancellationToken);
        string Type { get; }
    }
}
