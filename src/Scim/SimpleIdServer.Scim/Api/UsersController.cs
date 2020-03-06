// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Persistence;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Users)]
    public class UsersController : BaseApiController
    {
        public UsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMSchemaQueryRepository scimSchemaQueryRepository, IOptionsMonitor<SCIMHostOptions> options, ILogger<UsersController> logger) : base(SCIMConstants.SCIMEndpoints.Users, addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, scimRepresentationQueryRepository, scimSchemaQueryRepository, options, logger)
        {
        }
    }
}