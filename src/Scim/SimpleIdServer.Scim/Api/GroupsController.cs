// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Queries;

namespace SimpleIdServer.Scim.Api
{
    public class GroupsController : BaseApiController
    {
        public GroupsController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISearchRepresentationsQueryHandler searchRepresentationsQueryHandler, IGetRepresentationQueryHandler getRepresentationQueryHandler, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<SCIMHostOptions> options, ILogger<GroupsController> logger, IBusControl busControl, IResourceTypeResolver resourceTypeResolver, IUriProvider uriProvider, IRealmRepository realmRepository) : base(SCIMResourceTypes.Group, addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, searchRepresentationsQueryHandler, getRepresentationQueryHandler, attributeReferenceEnricher, options, logger, busControl, resourceTypeResolver, uriProvider, realmRepository)
        {
        }

        protected override bool IsPublishEvtsEnabled => Options.IsGroupPublishEvtsEnabled;
    }
}
