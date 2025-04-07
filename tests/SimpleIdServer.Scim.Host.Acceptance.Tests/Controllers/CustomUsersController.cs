// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Api;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Queries;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests.Controllers
{
    public class CustomUsersController : BaseApiController
    {
        public CustomUsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISearchRepresentationsQueryHandler searchRepresentationsQueryHandler, IGetRepresentationQueryHandler getRepresentationQueryHandler, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<ScimHostOptions> options, ILogger<CustomUsersController> logger, IBusHelper busControl, IResourceTypeResolver resourceTypeResolver, IUriProvider uriProvider, IRealmRepository realmRepository) : base("CustomUser", addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, searchRepresentationsQueryHandler, getRepresentationQueryHandler, attributeReferenceEnricher, options, logger, busControl, resourceTypeResolver, uriProvider, realmRepository)
        {
        }
    }
}
