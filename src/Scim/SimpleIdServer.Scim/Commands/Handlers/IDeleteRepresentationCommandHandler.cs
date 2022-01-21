// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public interface IDeleteRepresentationCommandHandler : ISCIMCommandHandler<DeleteRepresentationCommand, SCIMRepresentation>
    {
    }
}
