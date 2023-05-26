// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Queries;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMEndpoints.User)]
    public class UsersController : BaseApiController
    {
        public UsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISearchRepresentationsQueryHandler searchRepresentationsQueryHandler, IGetRepresentationQueryHandler getRepresentationQueryHandler, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<SCIMHostOptions> options, ILogger<UsersController> logger, IBusControl busControl, IResourceTypeResolver resourceTypeResolver, IUriProvider uriProvider) : base(SCIMResourceTypes.User, addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, searchRepresentationsQueryHandler, getRepresentationQueryHandler, attributeReferenceEnricher, options, logger, busControl, resourceTypeResolver, uriProvider)
        {
        }

        protected override bool IsPublishEvtsEnabled => Options.IsUserPublishEvtsEnabled;
    }
}