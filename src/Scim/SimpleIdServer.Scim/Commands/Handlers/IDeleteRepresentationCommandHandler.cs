// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Infrastructure;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public interface IDeleteRepresentationCommandHandler : ISCIMCommandHandler<DeleteRepresentationCommand, bool>
    {
    }
}
