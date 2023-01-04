// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.SubjectTypeBuilders
{
    public interface ISubjectTypeBuilder
    {
        string SubjectType { get; }
        Task<string> Build(HandlerContext context, CancellationToken cancellationToken);
    }
}
