// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Infrastructure
{
    public interface ISCIMCommandHandler<TCommand, TResult> where TCommand : ISCIMCommand<TResult>
    {
        Task<TResult> Handle(TCommand request);
    }
}
