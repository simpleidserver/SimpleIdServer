// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleIdServer.Did;

public interface IDidResolver
{
    Task<DidDocument> Resolve(string did, CancellationToken cancellationToken);
    string Method { get; }
}
